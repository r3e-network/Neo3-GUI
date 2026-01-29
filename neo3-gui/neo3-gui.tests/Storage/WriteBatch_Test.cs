using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Common.Storage.LevelDBModules;

namespace neo3_gui.tests.Storage
{
    [TestClass]
    public class WriteBatch_Test
    {
        [TestMethod]
        public void WriteBatch_Implements_IDisposable()
        {
            var batch = new WriteBatch();
            Assert.IsTrue(batch is IDisposable, "WriteBatch should implement IDisposable for deterministic cleanup.");
        }
    }
}
