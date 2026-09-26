using System;
using System.Collections.Generic;
using KakaoTalkAdBlockPlus.AdBlock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    [TestClass]
    public class CachedProcessIdSourceTests
    {
        private static readonly TimeSpan RefreshPeriod = TimeSpan.FromSeconds(1);

        private readonly CountingProcessIdSource _inner = new CountingProcessIdSource();
        private TimeSpan _now = TimeSpan.FromMinutes(5);

        private CachedProcessIdSource CreateCache() => new CachedProcessIdSource(_inner, RefreshPeriod, () => _now);

        [TestMethod]
        public void ShouldQueryInnerSourceOnFirstCall()
        {
            _inner.ProcessIds = new[] { 10, 20 };

            var ids = CreateCache().GetProcessIds();

            CollectionAssert.AreEquivalent(new[] { 10, 20 }, new List<int>(ids));
            Assert.AreEqual(1, _inner.CallCount);
        }

        [TestMethod]
        public void ShouldReuseIdsWithinRefreshPeriod()
        {
            var cache = CreateCache();
            _inner.ProcessIds = new[] { 10 };
            cache.GetProcessIds();

            _inner.ProcessIds = new[] { 99 };
            _now += TimeSpan.FromMilliseconds(999);
            var ids = cache.GetProcessIds();

            CollectionAssert.AreEquivalent(new[] { 10 }, new List<int>(ids));
            Assert.AreEqual(1, _inner.CallCount);
        }

        private sealed class CountingProcessIdSource : IProcessIdSource
        {
            public int[] ProcessIds { get; set; } = new int[0];

            public int CallCount { get; private set; }

            public IReadOnlyCollection<int> GetProcessIds()
            {
                CallCount++;
                return ProcessIds;
            }
        }
    }
}
