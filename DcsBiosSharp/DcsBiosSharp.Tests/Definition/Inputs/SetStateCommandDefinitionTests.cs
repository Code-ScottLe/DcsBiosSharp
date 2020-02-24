using System;
using System.Collections.Generic;
using System.Text;
using DcsBiosSharp.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DcsBiosSharp.Definition.Inputs.Tests
{
    [TestClass]
    public class SetStateCommandDefinitionTests
    {
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(0)]
        public void CreateCommandTest_WithValidArgs_ReturnExpectedCommandInstance(int args)
        {
            // Arrange
            var mockInstrument = new Mock<IModuleInstrument>();
            mockInstrument.Setup(m => m.Identifier).Returns("TEST_INS");
            var instance = new SetStateCommandDefinition<int>(mockInstrument.Object, 1, string.Empty);

            // Act
            IDcsBiosCommand command = instance.CreateCommand(args);

            // Assert
            Assert.AreEqual(expected: args.ToString(), actual: command.Arguments);
            Assert.AreEqual(expected: "TEST_INS", actual: command.CommandIdentifier);
        }
    }
}
