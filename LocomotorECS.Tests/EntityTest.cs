namespace LocomotorECS.Tests
{
    using System.Diagnostics.Tracing;

    using NUnit.Framework;

    [TestFixture]
    public class EntityTest
    {
        private class Test1Component : Component
        {
        }

        [Test]
        public void SetTag_TagChanged_FireEvent()
        {
            var counter = 0;
            var target = new Entity();
            target.Tag = 1;
            target.BeforeTagChanged += (e) => counter += e.Tag;
            target.AfterTagChanged += (e) => counter += e.Tag;
            target.Tag = 10;

            Assert.AreEqual(11, counter);
        }

        [Test]
        public void GetEnabled_Default_True()
        {
            var target = new Entity();
            target.AddComponent<Test1Component>();

            Assert.AreEqual(true, target.Enabled);
            Assert.AreEqual(true, target.GetComponent<Test1Component>().Enabled);
        }

        [Test]
        public void SetEnabled_ToEnabled_EntityEnabledComponentsAreNot()
        {
            var target = new Entity();
            target.AddComponent<Test1Component>();
            target.Enabled = false;

            Assert.AreEqual(false, target.Enabled);
            Assert.AreEqual(true, target.GetComponent<Test1Component>().Enabled);
        }

        [Test]
        public void AddComponent_AlreadyExists_Failed()
        {
            var target = new Entity();
            target.AddComponent<Test1Component>();
            Assert.Catch(() => target.AddComponent<Test1Component>());
        }
    }
}