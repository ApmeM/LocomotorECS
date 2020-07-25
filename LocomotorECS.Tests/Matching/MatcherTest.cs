namespace LocomotorECS.Tests.Matching
{
    using LocomotorECS.Matching;

    using NUnit.Framework;

    [TestFixture]
    public class MatcherTest
    {
        private class TestEntity : Entity
        {
        }

        private class Test1Component : Component
        {
        }

        private class Test2Component : Component
        {
        }

        private class Test3Component : Component
        {
        }

        [Test]
        public void IsMatched_SingleComponent_MatchedSingleFilter()
        {
            var target = new Matcher().All(typeof(Test1Component));

            var entity = new TestEntity();
            entity.AddComponent<Test1Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatched_MultipleComponents_MatchedSingleFilter()
        {
            var target = new Matcher().All(typeof(Test1Component));

            var entity = new TestEntity();
            entity.AddComponent<Test1Component>();
            entity.AddComponent<Test2Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatched_SingleComponent_NotMatchedDifferentFilter()
        {
            var target = new Matcher().All(typeof(Test1Component));

            var entity = new TestEntity();
            entity.AddComponent<Test2Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_MultipleComponents_AllMatchedFound()
        {
            var target = new Matcher().All(typeof(Test1Component), typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test1Component>();
            entity.AddComponent<Test2Component>();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatched_MultipleComponents_AllMatchedNotFound()
        {
            var target = new Matcher().All(typeof(Test1Component), typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test1Component>();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_ExcludedComponentNotFound_Matched()
        {
            var target = new Matcher().Exclude(typeof(Test2Component));

            var entity = new TestEntity();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatched_ExcludedComponentFound_NotMatched()
        {
            var target = new Matcher().Exclude(typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test2Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_NoExcludedComponent_Matched()
        {
            var target = new Matcher().Exclude(typeof(Test1Component), typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatched_AnyExcludedComponentFound_NotMatched()
        {
            var target = new Matcher().Exclude(typeof(Test1Component), typeof(Test2Component), typeof(Test3Component));

            var entity = new TestEntity();
            entity.AddComponent<Test2Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_OneComponentNotFound_NotMatched()
        {
            var target = new Matcher().One(typeof(Test1Component));

            var entity = new TestEntity();
            entity.AddComponent<Test2Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_OneComponentFound_Matched()
        {
            var target = new Matcher().One(typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test1Component>();
            entity.AddComponent<Test2Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatched_MultipleComponentsInFilter_AnyMatched()
        {
            var target = new Matcher().One(typeof(Test1Component), typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test1Component>();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatched_MultipleComponentsInFilter_NoMatched()
        {
            var target = new Matcher().One(typeof(Test1Component), typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_ComplexTest1_NoMatched()
        {
            var target = new Matcher().Exclude(typeof(Test3Component)).One(typeof(Test1Component), typeof(Test2Component));

            var entity = new TestEntity();
            entity.AddComponent<Test1Component>();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_ComplexTest2_NotMatched()
        {
            var target = new Matcher().All(typeof(Test3Component), typeof(Test2Component)).One(typeof(Test1Component));

            var entity = new TestEntity();
            entity.AddComponent<Test2Component>();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatched_ComplexTest3_Matched()
        {
            var target = new Matcher().All(typeof(Test3Component), typeof(Test2Component)).One(typeof(Test2Component), typeof(Test1Component));

            var entity = new TestEntity();
            entity.AddComponent<Test2Component>();
            entity.AddComponent<Test3Component>();
            entity.Components.CommitChanges();

            var result = target.IsMatched(entity);

            Assert.IsTrue(result);
        }

    }
}