using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;

namespace neo3_gui.tests
{
    [TestClass]
    public class Helpers_Test
    {
        #region IsValidUTF8ByteArray Tests

        [TestMethod]
        public void IsValidUTF8ByteArray_ValidAscii_ReturnsTrue()
        {
            // ASCII characters (0x00-0x7F) are valid 1-byte UTF-8
            byte[] ascii = Encoding.UTF8.GetBytes("Hello World");
            Assert.IsTrue(ascii.IsValidUTF8ByteArray());
        }

        [TestMethod]
        public void IsValidUTF8ByteArray_ValidChinese_ReturnsTrue()
        {
            // Chinese characters are valid 3-byte UTF-8
            byte[] chinese = Encoding.UTF8.GetBytes("你好世界");
            Assert.IsTrue(chinese.IsValidUTF8ByteArray());
        }

        [TestMethod]
        public void IsValidUTF8ByteArray_ValidEmoji_ReturnsTrue()
        {
            // Emoji are valid 4-byte UTF-8
            byte[] emoji = Encoding.UTF8.GetBytes("😀🎉");
            Assert.IsTrue(emoji.IsValidUTF8ByteArray());
        }

        [TestMethod]
        public void IsValidUTF8ByteArray_InvalidSequence_ReturnsFalse()
        {
            // Invalid: continuation byte without start byte
            byte[] invalid = new byte[] { 0x80, 0x80 };
            Assert.IsFalse(invalid.IsValidUTF8ByteArray());
        }

        [TestMethod]
        public void IsValidUTF8ByteArray_IncompleteSequence_ReturnsFalse()
        {
            // Invalid: 2-byte sequence start without continuation
            byte[] incomplete = new byte[] { 0xC2 };
            Assert.IsFalse(incomplete.IsValidUTF8ByteArray());
        }

        [TestMethod]
        public void IsValidUTF8ByteArray_EmptyArray_ReturnsTrue()
        {
            byte[] empty = Array.Empty<byte>();
            Assert.IsTrue(empty.IsValidUTF8ByteArray());
        }

        #endregion

        #region String Extension Tests

        [TestMethod]
        public void IsNull_NullString_ReturnsTrue()
        {
            string nullStr = null;
            Assert.IsTrue(nullStr.IsNull());
        }

        [TestMethod]
        public void IsNull_EmptyString_ReturnsTrue()
        {
            string empty = "";
            Assert.IsTrue(empty.IsNull());
        }

        [TestMethod]
        public void IsNull_WhitespaceString_ReturnsTrue()
        {
            string whitespace = "   ";
            Assert.IsTrue(whitespace.IsNull());
        }

        [TestMethod]
        public void IsNull_ValidString_ReturnsFalse()
        {
            string valid = "hello";
            Assert.IsFalse(valid.IsNull());
        }

        [TestMethod]
        public void NotNull_ValidString_ReturnsTrue()
        {
            string valid = "hello";
            Assert.IsTrue(valid.NotNull());
        }

        [TestMethod]
        public void NotNull_NullString_ReturnsFalse()
        {
            string nullStr = null;
            Assert.IsFalse(nullStr.NotNull());
        }

        #endregion

        #region ToBool Tests

        [TestMethod]
        public void ToBool_True_ReturnsTrue()
        {
            Assert.IsTrue("true".ToBool());
            Assert.IsTrue("TRUE".ToBool());
            Assert.IsTrue("True".ToBool());
        }

        [TestMethod]
        public void ToBool_Yes_ReturnsTrue()
        {
            Assert.IsTrue("yes".ToBool());
            Assert.IsTrue("YES".ToBool());
        }

        [TestMethod]
        public void ToBool_One_ReturnsTrue()
        {
            Assert.IsTrue("1".ToBool());
        }

        [TestMethod]
        public void ToBool_False_ReturnsFalse()
        {
            Assert.IsFalse("false".ToBool());
            Assert.IsFalse("no".ToBool());
            Assert.IsFalse("0".ToBool());
        }

        [TestMethod]
        public void ToBool_Null_ReturnsFalse()
        {
            string nullStr = null;
            Assert.IsFalse(nullStr.ToBool());
        }

        #endregion

        #region IsYes Tests

        [TestMethod]
        public void IsYes_Yes_ReturnsTrue()
        {
            Assert.IsTrue("yes".IsYes());
            Assert.IsTrue("YES".IsYes());
            Assert.IsTrue("y".IsYes());
            Assert.IsTrue("Y".IsYes());
        }

        [TestMethod]
        public void IsYes_No_ReturnsFalse()
        {
            Assert.IsFalse("no".IsYes());
            Assert.IsFalse("n".IsYes());
            Assert.IsFalse("true".IsYes());
        }

        [TestMethod]
        public void IsYes_Null_ReturnsFalse()
        {
            string nullStr = null;
            Assert.IsFalse(nullStr.IsYes());
        }

        #endregion

        #region Collection Extension Tests

        [TestMethod]
        public void IsEmpty_NullCollection_ReturnsTrue()
        {
            List<int> nullList = null;
            Assert.IsTrue(nullList.IsEmpty());
        }

        [TestMethod]
        public void IsEmpty_EmptyCollection_ReturnsTrue()
        {
            var emptyList = new List<int>();
            Assert.IsTrue(emptyList.IsEmpty());
        }

        [TestMethod]
        public void IsEmpty_NonEmptyCollection_ReturnsFalse()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.IsFalse(list.IsEmpty());
        }

        [TestMethod]
        public void NotEmpty_NonEmptyCollection_ReturnsTrue()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.IsTrue(list.NotEmpty());
        }

        [TestMethod]
        public void NotEmpty_EmptyCollection_ReturnsFalse()
        {
            var emptyList = new List<int>();
            Assert.IsFalse(emptyList.NotEmpty());
        }

        #endregion

        #region ToUInt160 Tests

        [TestMethod]
        public void ToUInt160_Valid20Bytes_ReturnsUInt160()
        {
            byte[] bytes = new byte[20];
            for (int i = 0; i < 20; i++) bytes[i] = (byte)i;
            
            var result = bytes.ToUInt160();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ToUInt160_InvalidLength_ReturnsNull()
        {
            byte[] bytes = new byte[10];
            var result = bytes.ToUInt160();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToUInt160_NullBytes_ReturnsNull()
        {
            byte[] bytes = null;
            var result = bytes.ToUInt160();
            Assert.IsNull(result);
        }

        #endregion

        #region Enum Extension Tests

        [TestMethod]
        public void ToEnum_ValidValue_ReturnsEnum()
        {
            var result = "Monday".ToEnum<DayOfWeek>();
            Assert.AreEqual(DayOfWeek.Monday, result);
        }

        [TestMethod]
        public void ToEnum_ValidValueIgnoreCase_ReturnsEnum()
        {
            var result = "monday".ToEnum<DayOfWeek>(ignoreCase: true);
            Assert.AreEqual(DayOfWeek.Monday, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToEnum_InvalidValue_ThrowsException()
        {
            "InvalidDay".ToEnum<DayOfWeek>();
        }

        [TestMethod]
        public void AsEnum_ValidValue_ReturnsEnum()
        {
            var result = "Tuesday".AsEnum<DayOfWeek>();
            Assert.AreEqual(DayOfWeek.Tuesday, result);
        }

        [TestMethod]
        public void AsEnum_InvalidValue_ReturnsDefault()
        {
            var result = "InvalidDay".AsEnum(DayOfWeek.Sunday);
            Assert.AreEqual(DayOfWeek.Sunday, result);
        }

        #endregion

        #region DateTime Extension Tests

        [TestMethod]
        public void AsUtcTime_LocalTime_ReturnsUtcKind()
        {
            var localTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Local);
            var utcTime = localTime.AsUtcTime();
            
            Assert.AreEqual(DateTimeKind.Utc, utcTime.Kind);
            Assert.AreEqual(localTime.Ticks, utcTime.Ticks);
        }

        [TestMethod]
        public void FromTimestampMS_ValidTimestamp_ReturnsCorrectDate()
        {
            // 2024-01-01 00:00:00 UTC in milliseconds
            ulong timestamp = 1704067200000;
            var result = timestamp.FromTimestampMS();
            
            Assert.AreEqual(2024, result.Year);
            Assert.AreEqual(1, result.Month);
            Assert.AreEqual(1, result.Day);
        }

        #endregion

        #region Byte Array Extension Tests

        [TestMethod]
        public void Append_MultipleArrays_ConcatenatesCorrectly()
        {
            byte[] source = new byte[] { 1, 2 };
            byte[] arr1 = new byte[] { 3, 4 };
            byte[] arr2 = new byte[] { 5, 6 };
            
            var result = source.Append(arr1, arr2);
            
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5, 6 }, result);
        }

        [TestMethod]
        public void ToBase64String_ValidBytes_ReturnsBase64()
        {
            byte[] data = new byte[] { 72, 101, 108, 108, 111 }; // "Hello"
            var result = data.ToBase64String();
            Assert.AreEqual("SGVsbG8=", result);
        }

        #endregion

        #region JSON Serialization Tests

        [TestMethod]
        public void SerializeJson_SimpleObject_ReturnsJson()
        {
            var obj = new { Name = "Test", Value = 123 };
            var json = obj.SerializeJson();
            
            Assert.IsTrue(json.Contains("name"));
            Assert.IsTrue(json.Contains("Test"));
            Assert.IsTrue(json.Contains("123"));
        }

        [TestMethod]
        public void DeserializeJson_ValidJson_ReturnsObject()
        {
            var json = "{\"name\":\"Test\",\"value\":123}";
            var result = json.DeserializeJson<TestModel>();
            
            Assert.AreEqual("Test", result.Name);
            Assert.AreEqual(123, result.Value);
        }

        [TestMethod]
        public void DeserializeJson_NullJson_ReturnsDefault()
        {
            string json = null;
            var result = json.DeserializeJson<TestModel>();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeserializeJson_EmptyJson_ReturnsDefault()
        {
            string json = "   ";
            var result = json.DeserializeJson<TestModel>();
            Assert.IsNull(result);
        }

        #endregion

        #region GetDefaultValue Tests

        [TestMethod]
        public void GetDefaultValue_IntType_ReturnsZero()
        {
            var result = typeof(int).GetDefaultValue();
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetDefaultValue_BoolType_ReturnsFalse()
        {
            var result = typeof(bool).GetDefaultValue();
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void GetDefaultValue_ReferenceType_ReturnsNull()
        {
            var result = typeof(string).GetDefaultValue();
            Assert.IsNull(result);
        }

        #endregion

        // Helper class for JSON tests
        private class TestModel
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
