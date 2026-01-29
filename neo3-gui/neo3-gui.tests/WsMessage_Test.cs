using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Models;

namespace neo3_gui.tests
{
    [TestClass]
    public class WsMessage_Test
    {
        [TestMethod]
        public void WsMessage_SetId_ReturnsCorrectValue()
        {
            var msg = new WsMessage { Id = "msg-001" };
            Assert.AreEqual("msg-001", msg.Id);
        }

        [TestMethod]
        public void WsMessage_SetMsgType_ReturnsCorrectValue()
        {
            var msg = new WsMessage { MsgType = WsMessageType.Response };
            Assert.AreEqual(WsMessageType.Response, msg.MsgType);
        }

        [TestMethod]
        public void WsMessage_SetMethod_ReturnsCorrectValue()
        {
            var msg = new WsMessage { Method = "getblock" };
            Assert.AreEqual("getblock", msg.Method);
        }

        [TestMethod]
        public void WsMessage_SetResult_ReturnsCorrectValue()
        {
            var msg = new WsMessage { Result = "success" };
            Assert.AreEqual("success", msg.Result);
        }

        [TestMethod]
        public void WsMessage_SetError_ReturnsCorrectValue()
        {
            var error = new WsError { Code = 100 };
            var msg = new WsMessage { Error = error };
            Assert.AreEqual(100, msg.Error?.Code);
        }

        [TestMethod]
        public void WsMessage_DefaultValues_AreDefault()
        {
            var msg = new WsMessage();
            Assert.IsNull(msg.Id);
            Assert.IsNull(msg.Method);
            Assert.IsNull(msg.Result);
            Assert.IsNull(msg.Error);
        }
    }
}
