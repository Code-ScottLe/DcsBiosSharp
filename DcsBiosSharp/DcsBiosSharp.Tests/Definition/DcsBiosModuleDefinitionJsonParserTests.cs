using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DcsBiosSharp.Definition.Tests
{
    [TestClass]
    public class DcsBiosModuleDefinitionJsonParserTests
    {
        private string commonJsonLocation = "./Assets/CommonData.json";
        private string commonJson;

        [TestInitialize]
        public void TestInit()
        {
            if (File.Exists(commonJsonLocation) && string.IsNullOrWhiteSpace(commonJson))
            {
                commonJson = File.ReadAllText(commonJsonLocation);
            }
        }

        [TestMethod]
        public void ParseModuleFromJsonTest_WithCommonDataAndValidModuleJson_ReturnsExpectedModule()
        {
            // Arrange
            string testModuleJsonPath = "./Assets/FA-18C_hornet.json";

            if (!File.Exists(testModuleJsonPath))
            {
                Assert.Inconclusive($"Test json is missing");
            }

            string moduleJson = File.ReadAllText(testModuleJsonPath);

            // Act
            var parser = new DcsBiosModuleDefinitionJsonParser(commonJson);
            IModule module = parser.ParseModuleFromJson(Path.GetFileNameWithoutExtension(testModuleJsonPath), moduleJson);

            // Assert.
            Assert.IsNotNull(module);
            Assert.AreNotEqual(notExpected: 0, actual: module.Instruments.Count);

            IModuleInstrument leftDDIContCtl = module.Instruments.FirstOrDefault(i => i.Identifier == "LEFT_DDI_CONT_CTL");
            Assert.IsNotNull(leftDDIContCtl);

            Assert.AreEqual(expected: 2, actual: leftDDIContCtl.InputDefinitions.Count);
            Assert.IsTrue(leftDDIContCtl.InputDefinitions.Any(i => i is SetStateCommandDefinition));
            Assert.IsTrue(leftDDIContCtl.InputDefinitions.Any(i => i is VariableStepCommandDefinition));

            Assert.AreEqual(expected: 1, actual: leftDDIContCtl.OutputDefinitions.Count);
            Assert.IsInstanceOfType(leftDDIContCtl.OutputDefinitions.First(), typeof(IntegerOutputDefinition));
        }

        [TestMethod]
        public void ParseModuleFromJsonCtorTest_WithNoArgs_CreateNewInstanceOk()
        {
            var parser = new DcsBiosModuleDefinitionJsonParser();

            Assert.IsNotNull(parser);
        }
    }
}
