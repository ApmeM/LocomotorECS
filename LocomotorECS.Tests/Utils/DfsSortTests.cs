namespace LocomotorECS.Tests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LocomotorECS.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class DfsSortTests
    {
        [Test]
        public void PushTags_NoParameters_TextEmpty()
        {
            var list = new List<int> { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            var dependency = new[]
            {
                new Tuple<int, int>(1, 4), new Tuple<int, int>(2, 4), new Tuple<int, int>(2, 5),
                new Tuple<int, int>(3, 5), new Tuple<int, int>(4, 6), new Tuple<int, int>(4, 7),
                new Tuple<int, int>(5, 7), new Tuple<int, int>(5, 8), new Tuple<int, int>(6, 9),
                new Tuple<int, int>(7, 9), new Tuple<int, int>(7, 10), new Tuple<int, int>(8, 10),
            }.GroupBy(a => a.Item1, a => a.Item2).ToDictionary(a => a.Key, a => a.ToList());

            var result = DfsSort.Sort(list, dependency).SelectMany(a => a).ToArray();
            Assert.AreEqual(10, result.Length);
            Assert.AreEqual(3, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(1, result[2]);
            Assert.AreEqual(5, result[3]);
            Assert.AreEqual(4, result[4]);
            Assert.AreEqual(8, result[5]);
            Assert.AreEqual(7, result[6]);
            Assert.AreEqual(6, result[7]);
            Assert.AreEqual(10, result[8]);
            Assert.AreEqual(9, result[9]);
        }
    }
}