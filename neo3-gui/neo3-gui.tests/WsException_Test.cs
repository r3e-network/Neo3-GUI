using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Models;

namespace neo3_gui.tests
{
    [TestClass]
    public class WsException_Test
    {
        [TestMethod]
        public void WsException_FromErrorCode_HasCorrectCode()
        {
            var ex = new WsException(ErrorCode.MethodNotFound);
            Assert.AreEqual((int)ErrorCode.MethodNotFound, ex.Code);
        }

        [TestMethod]
        public void WsException_WithMessage_HasCorrectMessage()
        {
            var ex = new WsException(ErrorCode.InvalidParams, "Custom msg");
            Assert.AreEqual("Custom msg", ex.Message);
        }

        [TestMethod]
        public void WsException_ToString_ContainsCodeAndMessage()
        {
            var ex = new WsException(ErrorCode.InvalidParams, "Test");
            var str = ex.ToString();
            Assert.IsTrue(str.Contains("WsException"));
            Assert.IsTrue(str.Contains("Test"));
        }
    }
}
