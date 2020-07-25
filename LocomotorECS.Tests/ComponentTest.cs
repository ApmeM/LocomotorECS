namespace LocomotorECS.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ComponentTest
    {
        [Test]
        public void Enabled_Changed_TriggerEvent()
        {
            var target = new Component();
            target.Enabled = false;
            var counter = 0;
            target.ComponentEnabled += component => counter++;
            target.ComponentDisabled += component => counter=100;

            target.Enabled = true;

            Assert.AreEqual(1, counter);
        }

        [Test]
        public void Enabled_NotChanged_EventNotTriggered()
        {
            var target = new Component();
            target.Enabled = true;
            var counter = 0;
            target.ComponentEnabled += component => counter++;
            target.ComponentDisabled += component => counter=100;

            target.Enabled = true;

            Assert.AreEqual(0, counter);
        }

        [Test]
        public void Disabled_NotChanged_EventNotTriggered()
        {
            var target = new Component();
            target.Enabled = false;
            var counter = 0;
            target.ComponentEnabled += component => counter = 100;
            target.ComponentDisabled += component => counter++;

            target.Enabled = false;

            Assert.AreEqual(0, counter);
        }

        [Test]
        public void Disabled_Changed_TriggerEvent()
        {
            var target = new Component();
            target.Enabled = true;
            var counter = 0;
            target.ComponentEnabled += component => counter = 100;
            target.ComponentDisabled += component => counter++;

            target.Enabled = false;

            Assert.AreEqual(1, counter);
        }
    }
}