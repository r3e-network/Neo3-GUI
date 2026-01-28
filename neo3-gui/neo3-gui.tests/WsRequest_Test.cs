using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Models;

namespace neo3_gui.tests
{
    [TestClass]
    public class WsRequest_Test
    {
        [TestMethod]
        public void WsRequest_SetId_ReturnsCorrectValue()
        {
            var request = new WsRequest { Id = "req-123" };
            Assert.AreEqual("req-123", request.Id);
        }

        [TestMethod]
        public void WsRequest_SetMethod_ReturnsCorrectValue()
        {
            var request = new WsRequest { Method = "getblock" };
            Assert.AreEqual("getblock", request.Method);
        }

        [TestMethod]
        public void WsRequest_SetParams_ReturnsCorrectValue()
        {
            var json = JsonDocument.Parse("{\"height\": 100}");
            var request = new WsRequest { Params = json.RootElement };
            
            Assert.AreEqual(100, request.Params.GetProperty("height").GetInt32());
        }

        [TestMethod]
        public void WsRequest_DefaultValues_AreDefault()
        {
            var request = new WsRequest();
            Assert.IsNull(request.Id);
            Assert.IsNull(request.Method);
        }
    }
}
