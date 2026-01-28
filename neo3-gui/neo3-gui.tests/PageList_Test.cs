using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Models;

namespace neo3_gui.tests
{
    [TestClass]
    public class PageList_Test
    {
        [TestMethod]
        public void PageList_DefaultList_IsEmpty()
        {
            var pageList = new PageList<int>();
            Assert.IsNotNull(pageList.List);
            Assert.AreEqual(0, pageList.List.Count);
        }

        [TestMethod]
        public void PageList_SetProperties_ReturnsCorrectValues()
        {
            var pageList = new PageList<string>
            {
                PageIndex = 2,
                PageSize = 10,
                TotalCount = 100
            };

            Assert.AreEqual(2, pageList.PageIndex);
            Assert.AreEqual(10, pageList.PageSize);
            Assert.AreEqual(100, pageList.TotalCount);
        }

        [TestMethod]
        public void Project_IntToString_TransformsCorrectly()
        {
            var pageList = new PageList<int>
            {
                PageIndex = 1,
                PageSize = 5,
                TotalCount = 3,
                List = new List<int> { 1, 2, 3 }
            };

            var projected = pageList.Project(x => x.ToString());

            Assert.AreEqual("1", projected.List[0]);
            Assert.AreEqual("2", projected.List[1]);
            Assert.AreEqual("3", projected.List[2]);
        }

        [TestMethod]
        public void Project_PreservesPageInfo()
        {
            var pageList = new PageList<int>
            {
                PageIndex = 3,
                PageSize = 20,
                TotalCount = 150,
                List = new List<int> { 10, 20 }
            };

            var projected = pageList.Project(x => x * 2);

            Assert.AreEqual(3, projected.PageIndex);
            Assert.AreEqual(20, projected.PageSize);
            Assert.AreEqual(150, projected.TotalCount);
        }

        [TestMethod]
        public void Project_EmptyList_ReturnsEmptyList()
        {
            var pageList = new PageList<int>
            {
                PageIndex = 1,
                PageSize = 10,
                TotalCount = 0,
                List = new List<int>()
            };

            var projected = pageList.Project(x => x.ToString());

            Assert.AreEqual(0, projected.List.Count);
        }
    }
}
