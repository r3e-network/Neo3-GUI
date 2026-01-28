using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.Common;
using Neo.SmartContract.Native;

namespace neo3_gui.tests
{
    [TestClass]
    public class AssetComparer_Test
    {
        private AssetComparer _comparer;

        [TestInitialize]
        public void Setup()
        {
            _comparer = new AssetComparer();
        }

        [TestMethod]
        public void Compare_SameAsset_ReturnsZero()
        {
            var asset = NativeContract.NEO.Hash;
            Assert.AreEqual(0, _comparer.Compare(asset, asset));
        }

        [TestMethod]
        public void Compare_NEOFirst_ReturnsPositive()
        {
            var neo = NativeContract.NEO.Hash;
            var gas = NativeContract.GAS.Hash;
            Assert.IsTrue(_comparer.Compare(neo, gas) > 0);
        }

        [TestMethod]
        public void Compare_GASFirst_ReturnsNegative()
        {
            var neo = NativeContract.NEO.Hash;
            var gas = NativeContract.GAS.Hash;
            Assert.IsTrue(_comparer.Compare(gas, neo) < 0);
        }

        [TestMethod]
        public void Compare_NEOWithOther_NEOWins()
        {
            var neo = NativeContract.NEO.Hash;
            var other = UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.IsTrue(_comparer.Compare(neo, other) > 0);
        }

        [TestMethod]
        public void Compare_OtherWithNEO_NEOWins()
        {
            var neo = NativeContract.NEO.Hash;
            var other = UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.IsTrue(_comparer.Compare(other, neo) < 0);
        }

        [TestMethod]
        public void Compare_GASWithOther_GASWins()
        {
            var gas = NativeContract.GAS.Hash;
            var other = UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.IsTrue(_comparer.Compare(gas, other) > 0);
        }
    }
}
