using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DcsBiosSharp.Definition.Tests
{
    [TestClass]
    public class ModuleDefinitionManagerTests
    {
        [TestMethod]
        public async Task RefreshModuleAsyncTest_WithDefaultFolder_HasNonZeroModules()
        {
            // Arrange
            var manager = new ModuleDefinitionManager();

            // Act
            await manager.RefreshModuleAsync();

            // Assert
            Assert.AreNotEqual(notExpected: 0, actual: manager.Modules.Count);
        }
    }
}
