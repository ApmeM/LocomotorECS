namespace LocomotorECS.Tests
{
    using LocomotorECS.Matching;

    using NUnit.Framework;

    [TestFixture]
    public class ComponentListTest
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
        public void Add_ByType_ComponentAddedAsPending()
        {
            var target = new ComponentList(new TestEntity());
            target.Add<Test1Component>();

            var withPending = target.Get<Test1Component>();
            var withoutPending = target.Get<Test1Component>(false);

            Assert.IsNotNull(withPending);
            Assert.IsNull(withoutPending);
        }

        [Test]
        public void CommitChanges_WithPendingComponent_ComponentAddedToRealList()
        {
            var target = new ComponentList(new TestEntity());
            target.Add<Test1Component>();
            target.CommitChanges();

            var withPending = target.Get<Test1Component>();
            var withoutPending = target.Get<Test1Component>(false);

            Assert.IsNotNull(withPending);
            Assert.IsNotNull(withoutPending);
        }

        [Test]
        public void Remove_NoCommit_ComponentStillAccessible()
        {
            var target = new ComponentList(new TestEntity());
            target.Add<Test1Component>();
            target.CommitChanges();
            target.Remove<Test1Component>();

            var result = target.Get<Test1Component>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void Remove_WithCommit_ComponentNotAccessible()
        {
            var target = new ComponentList(new TestEntity());
            target.Add<Test1Component>();
            target.CommitChanges();
            target.Remove<Test1Component>();
            target.CommitChanges();

            var result = target.Get<Test1Component>();

            Assert.IsNull(result);
        }
    }
}