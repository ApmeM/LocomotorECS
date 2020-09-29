namespace LocomotorECS.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class EntityListTest
    {
        [Test]
        public void FindEntityByName_EntityHasNoName_NotFoundByName()
        {
            var entity = new Entity();
            var list = new EntityList();
            list.Add(entity);

            var found = list.FindEntityByName(entity.Name);

            Assert.AreEqual(null, found);
        }

        [Test]
        public void FindEntityByName_EntityHasName_FoundByName()
        {
            var entity = new Entity("test");
            var list = new EntityList();
            list.Add(entity);

            var found = list.FindEntityByName(entity.Name);

            Assert.AreEqual(entity, found);
        }

        [Test]
        public void FindEntityByName_EntityHasDifferentName_NotFoundByName()
        {
            var entity = new Entity("test");
            var list = new EntityList();
            list.Add(entity);

            var found = list.FindEntityByName("test2");

            Assert.AreEqual(null, found);
        }

        [Test]
        public void Add_AddingWithExistingName_Exception()
        {
            var entity1 = new Entity("test");
            var entity2 = new Entity("test");
            var list = new EntityList();
            list.Add(entity1);
            Assert.Throws<ArgumentException>(() => list.Add(entity2));
        }

        [Test]
        public void FindEntitiesByTag_EntityHasNoTag_NotFoundByTag()
        {
            var entity = new Entity();
            var list = new EntityList();
            list.Add(entity);

            var found = list.FindEntitiesByTag(0);

            Assert.AreEqual(0, found.Count);
        }

        [Test]
        public void FindEntitiesByTag_EntityHasTag_FoundByTag()
        {
            var entity = new Entity { Tag = 1 };
            var list = new EntityList();
            list.Add(entity);

            var found = list.FindEntitiesByTag(1);

            Assert.AreEqual(1, found.Count);
            Assert.AreEqual(entity, found[0]);
        }

        [Test]
        public void FindEntitiesByTag_MultipleEntitiesHasTag_FoundAllByTag()
        {
            var entity1 = new Entity { Tag = 1 };
            var entity2 = new Entity { Tag = 1 };
            var entity3 = new Entity { Tag = 2 };
            var list = new EntityList();
            list.Add(entity1);
            list.Add(entity2);
            list.Add(entity3);

            var found = list.FindEntitiesByTag(1);

            Assert.AreEqual(2, found.Count);
            Assert.Contains(entity1, found);
            Assert.Contains(entity1, found);
            Assert.Contains(entity2, found);
        }
    }
}
