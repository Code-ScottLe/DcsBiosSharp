using System;
using System.Collections.Generic;
using System.Text;
using DcsBiosSharp.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DcsBiosSharp.Definition.Inputs.Tests
{
    [TestClass]
    public class FixedStepCommandDefinitionTests
    {
        [DataTestMethod]
        [DataRow("INC")]
        [DataRow("DEC")]
        public void CreateCommandTest_WithValidArgs_ReturnExpectedCommandInstance(string args)
        {
            // Arrange
            var mockInstrument = new Mock<IModuleInstrument>();
            mockInstrument.Setup(m => m.Identifier).Returns("TEST_INS");
            var instance = new FixedStepCommandDefinition(mockInstrument.Object);

            // Act
            IDcsBiosCommand command = instance.CreateCommand(args);

            // Assert
            Assert.AreEqual(expected: args, actual:command.Arguments);
            Assert.AreEqual(expected: "TEST_INS", actual: command.CommandIdentifier);
        }
    }
}
