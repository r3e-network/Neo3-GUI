using System.Net.WebSockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Common;

namespace neo3_gui.tests
{
    [TestClass]
    public class WebSocketStringResult_Test
    {
        [TestMethod]
        public void Constructor_BasicParams_SetsProperties()
        {
            var result = new WebSocketStringResult(100, WebSocketMessageType.Text, true);
            
            Assert.AreEqual(100, result.Count);
            Assert.AreEqual(WebSocketMessageType.Text, result.MessageType);
            Assert.IsTrue(result.EndOfMessage);
        }

        [TestMethod]
        public void Constructor_BinaryMessage_SetsCorrectType()
        {
            var result = new WebSocketStringResult(50, WebSocketMessageType.Binary, false);
            
            Assert.AreEqual(WebSocketMessageType.Binary, result.MessageType);
            Assert.IsFalse(result.EndOfMessage);
        }

        [TestMethod]
        public void Constructor_WithCloseStatus_SetsAllProperties()
        {
            var result = new WebSocketStringResult(
                0, 
                WebSocketMessageType.Close, 
                true, 
                WebSocketCloseStatus.NormalClosure, 
                "Connection closed normally");
            
            Assert.AreEqual(WebSocketCloseStatus.NormalClosure, result.CloseStatus);
            Assert.AreEqual("Connection closed normally", result.CloseStatusDescription);
        }

        [TestMethod]
        public void Message_SetAndGet_ReturnsCorrectValue()
        {
            var result = new WebSocketStringResult(10, WebSocketMessageType.Text, true);
            result.Message = "Hello World";
            
            Assert.AreEqual("Hello World", result.Message);
        }

        [TestMethod]
        public void Message_DefaultValue_IsNull()
        {
            var result = new WebSocketStringResult(10, WebSocketMessageType.Text, true);
            
            Assert.IsNull(result.Message);
        }
    }
}
