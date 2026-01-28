using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Common;

namespace neo3_gui.tests
{
    [TestClass]
    public class ByteArrayValueComparer_Test
    {
        private ByteArrayValueComparer _comparer;

        [TestInitialize]
        public void Setup()
        {
            _comparer = ByteArrayValueComparer.Default;
        }

        [TestMethod]
        public void Default_Instance_IsNotNull()
        {
            Assert.IsNotNull(ByteArrayValueComparer.Default);
        }

        [TestMethod]
        public void Equals_SameContent_ReturnsTrue()
        {
            byte[] a = new byte[] { 1, 2, 3, 4, 5 };
            byte[] b = new byte[] { 1, 2, 3, 4, 5 };
            Assert.IsTrue(_comparer.Equals(a, b));
        }

        [TestMethod]
        public void Equals_DifferentContent_ReturnsFalse()
        {
            byte[] a = new byte[] { 1, 2, 3, 4, 5 };
            byte[] b = new byte[] { 1, 2, 3, 4, 6 };
            Assert.IsFalse(_comparer.Equals(a, b));
        }

        [TestMethod]
        public void Equals_DifferentLength_ReturnsFalse()
        {
            byte[] a = new byte[] { 1, 2, 3 };
            byte[] b = new byte[] { 1, 2, 3, 4 };
            Assert.IsFalse(_comparer.Equals(a, b));
        }

        [TestMethod]
        public void Equals_BothEmpty_ReturnsTrue()
        {
            byte[] a = Array.Empty<byte>();
            byte[] b = Array.Empty<byte>();
            Assert.IsTrue(_comparer.Equals(a, b));
        }

        [TestMethod]
        public void Equals_SameReference_ReturnsTrue()
        {
            byte[] a = new byte[] { 1, 2, 3 };
            Assert.IsTrue(_comparer.Equals(a, a));
        }

        [TestMethod]
        public void GetHashCode_SameContent_ReturnsSameHash()
        {
            byte[] a = new byte[] { 1, 2, 3, 4, 5 };
            byte[] b = new byte[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(_comparer.GetHashCode(a), _comparer.GetHashCode(b));
        }

        [TestMethod]
        public void GetHashCode_DifferentContent_ReturnsDifferentHash()
        {
            byte[] a = new byte[] { 1, 2, 3, 4, 5 };
            byte[] b = new byte[] { 5, 4, 3, 2, 1 };
            Assert.AreNotEqual(_comparer.GetHashCode(a), _comparer.GetHashCode(b));
        }

        [TestMethod]
        public void GetHashCode_EmptyArray_ReturnsConsistentHash()
        {
            byte[] a = Array.Empty<byte>();
            byte[] b = Array.Empty<byte>();
            Assert.AreEqual(_comparer.GetHashCode(a), _comparer.GetHashCode(b));
        }

        [TestMethod]
        public void GetHashCode_SingleByte_ReturnsExpectedHash()
        {
            byte[] a = new byte[] { 42 };
            int hash = _comparer.GetHashCode(a);
            // hash = 17 * 31 + 42 = 527 + 42 = 569
            Assert.AreEqual(569, hash);
        }
    }
}
