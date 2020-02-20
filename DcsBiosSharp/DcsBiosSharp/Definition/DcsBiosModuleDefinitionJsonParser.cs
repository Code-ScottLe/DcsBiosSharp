using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DcsBiosSharp.Definition
{
    public class DcsBiosModuleDefinitionJsonParser
    {
        private IModule _commonsModule;

        public DcsBiosModuleDefinitionJsonParser(string commonDataJson = default)
        {
            _commonsModule = string.IsNullOrWhiteSpace(commonDataJson) ? null : ParseModuleFromJson("CommonData", commonDataJson);
        }

        public IModule ParseModuleFromJson(string moduleId, string json)
        {

            List<IModuleInstrument> instruments = new List<IModuleInstrument>();

            // Add common data if we have them.
            foreach (var metaInstrument in _commonsModule?.Instruments)
            {
                instruments.Add(metaInstrument);
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                JObject jsonModule = JObject.Parse(json);

                foreach (JObject category in jsonModule.Properties().Select(p => p.Value).Cast<JObject>())
                {
                    foreach (JObject command in category.Properties().Select(p => p.Value).Cast<JObject>())
                    {
                        string commandIdentifier = (string)command["identifier"];
                        string commandDescription = (string)command["description"];
                        string commandCategory = (string)command["category"];
                        string commandControlType = (string)command["control_type"];

                        JArray inputs = (JArray)command["inputs"];

                        List<IDcsBiosInputDefinition> inputList = new List<IDcsBiosInputDefinition>();
                        foreach (JObject inputJsonObject in inputs)
                        {
                            IDcsBiosInputDefinition input = CreateInputDefinitionFromJson(inputJsonObject);
                            inputList.Add(input);
                        }

                        JArray outputs = (JArray)command["outputs"];
                        List<IDcsBiosOutputDefinition> outputDefinitionList = new List<IDcsBiosOutputDefinition>();
                        foreach (JObject outputJsonObject in outputs)
                        {
                            IDcsBiosOutputDefinition outputDef = CreateOutputDefinitionFromJson(outputJsonObject);
                            outputDefinitionList.Add(outputDef);
                        }

                        var commandInstance = new DcsBiosModuleInstrument(commandCategory, commandControlType, commandDescription, commandIdentifier, inputList, outputDefinitionList);

                        instruments.Add(commandInstance);
                    }
                }
            }

            var instance = new DcsBiosModule(moduleId, instruments);

            return instance;
        }

        public IDcsBiosOutputDefinition CreateOutputDefinitionFromJson(JObject outputJsonObject)
        {
            // try get the address.
            if (!outputJsonObject.TryGetValue("address", out JToken addressToken))
            {
                throw new ArgumentException("can't find the address");
            }

            long address = (long)(addressToken as JValue).Value;

            string description = outputJsonObject.TryGetValue("description", out JToken descriptionToken) ? (descriptionToken as JValue).Value as string : string.Empty;

            string suffix = outputJsonObject.TryGetValue("suffix", out JToken suffixToken) ? (suffixToken as JValue).Value as string : string.Empty;

            // try and get the type.
            string outputType = outputJsonObject.TryGetValue("type", out JToken outputTypeToken) ? (outputTypeToken as JValue).Value as string : throw new ArgumentException("can't find the output type.");

            if (outputType == "integer")
            {
                long mask = outputJsonObject.TryGetValue("mask", out JToken maskToken) ? (long)(maskToken as JValue).Value : throw new ArgumentException("can't find the integer mask value");

                long maxValue = outputJsonObject.TryGetValue("max_value", out JToken maxvalueToken) ? (long)(maxvalueToken as JValue).Value : throw new ArgumentException("can't find the integer maxValue");

                long shiftBy = outputJsonObject.TryGetValue("shift_by", out JToken shiftByToken) ? (long)(shiftByToken as JValue).Value : throw new ArgumentException("can't find the integer shift_by");

                return new IntegerOutputDefinition((uint)address, (int)shiftBy, (int)mask, (int)maxValue, description, suffix);
            }
            else if (outputType == "string")
            {
                long maxLength = outputJsonObject.TryGetValue("max_length", out JToken maxLengthToken) ? (long)(maxLengthToken as JValue).Value : throw new ArgumentException("can't find the string max length");

                return new StringOutputDefinition((uint)address, (int)maxLength, description, suffix);
            }
            else
            {
                return null;
            }
        }

        public IDcsBiosInputDefinition CreateInputDefinitionFromJson(JObject inputJsonObject)
        {
            // Try get the "interface" section
            if (!inputJsonObject.TryGetValue("interface", out JToken inputInterface))
            {
                throw new ArgumentException("Unexpected json blob given. No input interface was detected");
            }

            string inputInterfaceString = (inputInterface as JValue).Value as string;
            string def = inputJsonObject.TryGetValue("description", out JToken description) ? (description as JValue).Value as string : string.Empty;

            if (inputInterfaceString == FixedStepCommandDefinition.DEFAULT_COMMAND_INTERFACE_NAME)
            {
                return string.IsNullOrEmpty(def) ? new FixedStepCommandDefinition() : new FixedStepCommandDefinition(def);
            }
            else if (inputInterfaceString == SetStateCommandDefinition.DEFAULT_COMMAND_INTERFACE_NAME)
            {
                if (inputJsonObject.TryGetValue("max_value", out JToken max) && max is JValue value)
                {
                    switch (max.Type)
                    {
                        case JTokenType.Integer:
                            return new SetState<long>((long)value.Value, def);
                        case JTokenType.Float:
                            return new SetState<double>((double)value.Value, def);
                        default:
                            System.Diagnostics.Debug.WriteLine($"Dropping a command. Type: {max.Type} | Raw: {max.ToString()}");
                            return null;
                    }
                }
                else
                {
                    return new SetState<uint>(uint.MaxValue, def);
                }
            }
            else if (inputInterfaceString == ActionCommandDefinition.DEFAULT_COMMAND_INTERFACE_NAME)
            {
                if (!inputJsonObject.TryGetValue("argument", out JToken arg))
                {
                    throw new ArgumentException("Action Command needs argument!");
                }

                string argValue = (arg as JValue).Value as string;

                return new ActionCommandDefinition(argValue, def);
            }
            else if (inputInterfaceString == VariableStepCommandDefinition.DEFAULT_COMMAND_INTERFACE_NAME)
            {
                if (inputJsonObject.TryGetValue("max_value", out JToken max) && inputJsonObject.TryGetValue("suggested_step", out JToken suggest))
                {
                    var maxValue = max as JValue;
                    var suggestValue = suggest as JValue;

                    switch (max.Type)
                    {
                        case JTokenType.Integer:
                        case JTokenType.Float:
                            return new VariableStepCommandDefinition((double)maxValue.Value, (double)suggestValue.Value);
                        default:
                            System.Diagnostics.Debug.WriteLine($"Dropping a command. Type: {max.Type} | Raw: {max.ToString()}");
                            return null;
                    }
                }
                else
                {
                    throw new ArgumentNullException("Variable Steps needs both max value and suggested step.");
                }
            }
            else
            {
                // can't find the type.
                System.Diagnostics.Debug.WriteLine($"Dropping a command. Can't find interface type. Type: {inputInterfaceString} | Raw: {inputInterface.ToString()}");
                return null;
            }

        }
    }
}
