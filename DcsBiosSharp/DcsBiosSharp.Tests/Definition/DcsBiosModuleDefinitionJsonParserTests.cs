using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DcsBiosSharp.Tests.Definition
{
    [TestClass]
    public class DcsBiosModuleDefinitionJsonParserTests
    {

        [DataTestMethod()]
        [DataRow("./Assets/CommonData.json","./Assets/FA-18C_hornet.json")]
        public void ParseModuleFromJsonTest_WithCommonDataAndValidModuleJson_ReturnsExpectedModule(string commonJsonPath, string testModuleJsonPath)
        {
            // Arrange
            if (!File.Exists(testModuleJsonPath) || !File.Exists(commonJsonPath))
            {
                Assert.Inconclusive($"Test json is missing");
            }

            string commonJson = File.ReadAllText(commonJsonPath);
            string moduleJson = File.ReadAllText(testModuleJsonPath);

            // Act
            var parser = new DcsBiosModuleDefinitionJsonParser(commonJson);
            IModule module = parser.ParseModuleFromJson("FA-18C_hornet", moduleJson);

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
