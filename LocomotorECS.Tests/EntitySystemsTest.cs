namespace LocomotorECS.Tests
{
    using System;

    using LocomotorECS.Matching;

    using NUnit.Framework;

    [TestFixture]
    public class EntitySystemsTest
    {
        private class EntitySystems : EntityProcessingSystem
        {
            public int DoActionCount { get; set; }
            public int DoActionPerEntityCount { get; set; }

            public EntitySystems() : base(Matcher.Empty())
            {
            }

            public override void DoAction(TimeSpan gameTime)
            {
                base.DoAction(gameTime);
                DoActionCount++;
            }

            protected override void DoAction(Entity entity, TimeSpan gameTime)
            {
                base.DoAction(entity, gameTime);
                DoActionPerEntityCount++;
            }
        }

        [Test]
        public void DoAction_MultipleEntitiesMatched_CalledForAllEntities()
        {
            var target = new EntitySystems();
            target.NotifyEntityAdded(new Entity());
            target.NotifyEntityAdded(new Entity());
            target.NotifyEntityAdded(new Entity());

            target.DoAction(TimeSpan.Zero);

            Assert.AreEqual(1, target.DoActionCount);
            Assert.AreEqual(3, target.DoActionPerEntityCount);
        }
    }
}