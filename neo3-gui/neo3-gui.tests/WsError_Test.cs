using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Models;

namespace neo3_gui.tests
{
    [TestClass]
    public class WsError_Test
    {
        [TestMethod]
        public void WsError_SetCode_ReturnsCorrectValue()
        {
            var error = new WsError { Code = 100 };
            Assert.AreEqual(100, error.Code);
        }

        [TestMethod]
        public void WsError_SetMessage_ReturnsCorrectValue()
        {
            var error = new WsError { Message = "Test error" };
            Assert.AreEqual("Test error", error.Message);
        }

        [TestMethod]
        public void WsError_SetBothProperties_ReturnsCorrectValues()
        {
            var error = new WsError 
            { 
                Code = 500, 
                Message = "Internal error" 
            };
            
            Assert.AreEqual(500, error.Code);
            Assert.AreEqual("Internal error", error.Message);
        }

        [TestMethod]
        public void WsError_DefaultValues_AreDefault()
        {
            var error = new WsError();
            Assert.AreEqual(0, error.Code);
            Assert.IsNull(error.Message);
        }

        [TestMethod]
        public void WsError_NegativeCode_IsValid()
        {
            var error = new WsError { Code = -1 };
            Assert.AreEqual(-1, error.Code);
        }

        [TestMethod]
        public void WsError_EmptyMessage_IsValid()
        {
            var error = new WsError { Message = "" };
            Assert.AreEqual("", error.Message);
        }

        [TestMethod]
        public void WsError_FromCode_CreatesError()
        {
            var error = WsError.FromCode(ErrorCode.MethodNotFound);
            Assert.IsNotNull(error);
            Assert.AreEqual((int)ErrorCode.MethodNotFound, error.Code);
        }
    }
}
