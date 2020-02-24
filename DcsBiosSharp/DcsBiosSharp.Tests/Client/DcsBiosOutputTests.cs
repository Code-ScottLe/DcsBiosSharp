using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DcsBiosSharp.Client.Tests
{
    [TestClass]
    public class DcsBiosOutputTests
    {
        [TestMethod]
        public void PropertyChangedTest_WithDuplicateBufferWrite_OnlyNotifyOnce()
        {
            // Arrange
            var module = GetSampleModule();
            var scratchPad1 = module.Instruments.FirstOrDefault(i => i.Identifier == "UFC_OPTION_DISPLAY_1").OutputDefinitions.FirstOrDefault() as IDcsBiosOutputDefinition<string>;
            var mocked = new Mock<IDcsBiosDataBuffer>();

            byte[] controlled = new byte[0x10000];
            mocked.Setup(b => b.Buffer).Returns(controlled);

            int propertyChangedCount = 0;

            // Act
            var instance = new DcsBiosOutput<string>(scratchPad1, mocked.Object);
            instance.PropertyChanged += (s, e) =>
                {
                    propertyChangedCount++;
                };

            mocked.Raise(b => b.BufferUpdated += null, null, new DcsBiosBufferUpdatedEventArgs((int)scratchPad1.Address, (int)scratchPad1.Address + scratchPad1.MaxSize));
            mocked.Raise(b => b.BufferUpdated += null, null, new DcsBiosBufferUpdatedEventArgs((int)scratchPad1.Address, (int)scratchPad1.Address + scratchPad1.MaxSize));

            // Assert
            Assert.AreEqual(expected: 1, actual: propertyChangedCount);
        }

        private IModule GetSampleModule()
        {
            string testModuleJsonPath = "./Assets/FA-18C_hornet.json";

            if (!File.Exists(testModuleJsonPath))
            {
                Assert.Inconclusive($"Test json is missing");
            }

            string moduleJson = File.ReadAllText(testModuleJsonPath);

            // Act
            var parser = new DcsBiosModuleDefinitionJsonParser();
            IModule module = parser.ParseModuleFromJson(Path.GetFileNameWithoutExtension(testModuleJsonPath), moduleJson);

            return module;
        }
    }
}
