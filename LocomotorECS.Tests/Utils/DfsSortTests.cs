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
            var list = new List<int> { 1, 2, 3 };
            var dependency = new[] { new Tuple<int, int>(3, 1), new Tuple<int, int>(1, 2) }
                .GroupBy(a => a.Item1, a => a.Item2).ToDictionary(a => a.Key, a => a.ToList());

            var result = DfsSort.Sort(list, dependency);
            Assert.AreEqual(3, result[0]);
            Assert.AreEqual(1, result[1]);
            Assert.AreEqual(2, result[2]);
        }
    }
}