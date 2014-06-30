using Microsoft.VisualStudio.TestTools.UnitTesting;
using PB.Frameworks.Common.Extensions;
using PB.Frameworks.Types.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PB.UnitTests
{
	class TestObject : DbEntity
	{
		public TestObject TestSimple { get; set; }
		public TestObject[][][] TestArray { get; set; }
		public Dictionary<int, Dictionary<int, TestObject>> TestDictionary { get; set; }
		public List<List<TestObject>> TestList { get; set; }
	}

	[TestClass]
	public class GenericExtensionsTests
	{
		[TestMethod]
		public void TestDeepClone()
		{
			var to = new TestObject
			{
				Id = 1,
				TestSimple = new TestObject { Id = 2 },
				TestArray = new TestObject[][][] { new TestObject[][] { new TestObject[] { new TestObject { Id = 3 } } } },
				TestDictionary = new Dictionary<int, Dictionary<int, TestObject>> { { 4, new Dictionary<int, TestObject> { { 5, new TestObject { Id = 6 } } } } },
				TestList = new List<List<TestObject>> { new List<TestObject> { new TestObject { Id = 7 } } }
			};
			TestObject clone = to.DeepClone();
			Assert.AreEqual(to.Id, clone.Id);
			Assert.AreEqual(to.TestSimple.Id, clone.TestSimple.Id);
			Assert.AreEqual(to.TestArray[0][0][0].Id, clone.TestArray[0][0][0].Id);
			Assert.AreEqual(to.TestDictionary[4][5].Id, clone.TestDictionary[4][5].Id);
			Assert.AreEqual(to.TestList[0][0].Id, clone.TestList[0][0].Id);
			Assert.AreNotEqual(to, clone);
			Assert.AreNotEqual(to.TestSimple, clone.TestSimple);
			Assert.AreNotEqual(to.TestArray[0][0][0], clone.TestArray[0][0][0]);
			Assert.AreNotEqual(to.TestDictionary[4][5], clone.TestDictionary[4][5]);
			Assert.AreNotEqual(to.TestList[0][0], clone.TestList[0][0]);
		}

		[TestMethod]
		public void TestDeepCloneLoadTest()
		{
			var to = new TestObject
			{
				Id = 1,
				TestSimple = new TestObject { Id = 2 },
				TestArray = new TestObject[][][] { new TestObject[][] { new TestObject[] { new TestObject { Id = 3 } } } },
				TestDictionary = new Dictionary<int, Dictionary<int, TestObject>> { { 4, new Dictionary<int, TestObject> { { 5, new TestObject { Id = 6 } } } } },
				TestList = new List<List<TestObject>> { new List<TestObject> { new TestObject { Id = 7 } } }
			};
			to.DeepClone();
			var sw = new Stopwatch();
			sw.Restart();
			for (int i = 0; i < 5000; i++)
			{
				to.DeepClone();
			}
			Console.WriteLine(sw.ElapsedMilliseconds);
			Assert.IsTrue(sw.ElapsedMilliseconds < 100, "Test is expected to be run under 100 ms but got {0} ms.".FormatStr(sw.ElapsedMilliseconds));
		}
	}
}
