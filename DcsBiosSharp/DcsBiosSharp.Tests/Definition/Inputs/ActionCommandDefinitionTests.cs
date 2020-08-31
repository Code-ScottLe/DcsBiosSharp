using System;
using System.Collections.Generic;
using System.Text;
using DcsBiosSharp.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DcsBiosSharp.Definition.Inputs.Tests
{
    [TestClass]
    public class ActionCommandDefinitionTests
    {
        [TestMethod]
        public void CreateCommandTest_WithValidArgs_ReturnExpectedCommandInstance()
        {
            // Arrange
            var mockInstrument = new Mock<IModuleInstrumentDefinition>();
            mockInstrument.Setup(m => m.Identifier).Returns("TEST_INS");
            var instance = new ActionCommandDefinition(mockInstrument.Object, "TOGGLE");

            // Act
            IDcsBiosCommand command = instance.CreateCommand();

            // Assert
            Assert.AreEqual(expected: "TOGGLE", actual: command.Arguments);
            Assert.AreEqual(expected: "TEST_INS", actual: command.CommandIdentifier);
        }
    }
}
