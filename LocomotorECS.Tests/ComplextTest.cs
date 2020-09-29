namespace LocomotorECS.Tests
{
    using System;

    using LocomotorECS.Matching;

    using NUnit.Framework;

    [TestFixture]
    public class ComplextTest
    {
        private class EntitySystems : EntityProcessingSystem
        {
            public int Counter { get; set; }

            public EntitySystems() : base(new Matcher().All(typeof(Component)))
            {
            }

            protected override void OnMatchedEntityAdded(Entity entity)
            {
                base.OnMatchedEntityAdded(entity);
                Counter += 10;
            }

            protected override void OnMatchedEntityRemoved(Entity entity)
            {
                base.OnMatchedEntityRemoved(entity);
                Counter += 100;
            }

            protected override void OnMatchedEntityChanged(Entity entity)
            {
                base.OnMatchedEntityChanged(entity);
                Counter += 1000;
            }

            protected override void DoAction(Entity entity, TimeSpan gameTime)
            {
                base.DoAction(entity, gameTime);
                Counter += 1;
            }
        }

        private class DisablerSystem : EntityProcessingSystem
        {
            public DisablerSystem() : base(new Matcher().All(typeof(Component)))
            {
            }

            protected override void DoAction(Entity entity, TimeSpan gameTime)
            {
                base.DoAction(entity, gameTime);
                entity.Enabled = false;
            }
        }

        [Test]
        public void BeforeFirstCommit_NoEntitiesHandled()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(system);
            entity.AddComponent(component);

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(0, system.Counter);
        }

        [Test]
        public void FirstCommit_EntityAddedAndHandled()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(system);
            entity.AddComponent(component);

            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(11, system.Counter);
        }

        [Test]
        public void DisableComponent_EntityRemovedFromSystem()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(system);
            entity.AddComponent(component);
            entities.CommitChanges();

            component.Enabled = false;
            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(110, system.Counter);
        }

        [Test]
        public void EnableComponent_EntityAddedToSystem()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(system);
            entity.AddComponent(component);
            component.Enabled = false;
            entities.CommitChanges();

            component.Enabled = true;
            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(11, system.Counter);
        }

        [Test]
        public void DisableEntity_EntityRemovedFromSystem()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(system);
            entity.AddComponent(component);
            entities.CommitChanges();

            entity.Enabled = false;
            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(110, system.Counter);
        }

        [Test]
        public void EnableEntity_EntityAddedToSystem()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(system);
            entity.AddComponent(component);
            entity.Enabled = false;
            entities.CommitChanges();

            entity.Enabled = true;
            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(11, system.Counter);
        }

        [Test]
        public void ComponentEnabled_WhenEntityDisabled_NoNotification()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(system);
            entity.AddComponent(component);
            entity.Enabled = false;
            component.Enabled = false;
            entities.CommitChanges();

            component.Enabled = true;
            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(0, system.Counter);
        }

        [Test]
        public void NotifyDoAction_SubsystemDisableEntity_OtherSubsystemsShouldReceiveNotification()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var disabler = new DisablerSystem();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(disabler);
            entitySystems.Add(system);
            entity.AddComponent(component);
            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(11, system.Counter);
        }

        [Test]
        public void NotifyDoAction_SubsystemDisableEntity_ChangesTakesEffectOnlyAfterCommitChanges()
        {
            var entity = new Entity();
            var component = new Component();
            var system = new EntitySystems();
            var disabler = new DisablerSystem();
            var entities = new EntityList();
            var entitySystems = new EntitySystemList(entities);
            entities.Add(entity);
            entitySystems.Add(disabler);
            entitySystems.Add(system);
            entity.AddComponent(component);
            entities.CommitChanges();

            entitySystems.NotifyDoAction(TimeSpan.Zero);
            entities.CommitChanges();
            entitySystems.NotifyDoAction(TimeSpan.Zero);

            Assert.AreEqual(111, system.Counter);
        }
    }
}