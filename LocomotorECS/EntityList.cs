namespace LocomotorECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class EntityList : IEnumerable<Entity>
    {
        private readonly List<Entity> entities = new List<Entity>();

        private readonly HashSet<Entity> entitiesToAdd = new HashSet<Entity>();

        private readonly HashSet<Entity> entitiesToRemove = new HashSet<Entity>();

        private readonly Dictionary<int, List<Entity>> entityTagsDict = new Dictionary<int, List<Entity>>();

        internal event Action<Entity> EntityRemoved;
        internal event Action<Entity> EntityAdded;
        internal event Action<Entity> EntityChanged;

        public void Add(Entity entity)
        {
            this.entitiesToAdd.Add(entity);
        }

        public void Remove(Entity entity)
        {
            this.entitiesToAdd.Remove(entity);
            this.entitiesToRemove.Add(entity);
        }
        
        public void CommitChanges()
        {
            if (this.entitiesToRemove.Count > 0)
            {
                foreach (var entity in this.entitiesToRemove)
                {
                    this.RemoveFromTagList(entity);
                    this.entities.Remove(entity);
                    entity.BeforeTagChanged -= this.RemoveFromTagList;
                    entity.AfterTagChanged -= this.AddToTagList;
                }
            }

            if (this.entitiesToAdd.Count > 0)
            {
                foreach (var entity in this.entitiesToAdd)
                {
                    this.AddToTagList(entity);
                    this.entities.Add(entity);
                    entity.BeforeTagChanged += this.RemoveFromTagList;
                    entity.AfterTagChanged += this.AddToTagList;
                }
            }

            for (var i = 0; i < this.entities.Count; i++)
            {
                var entity = this.entities[i];
                var isSomethingChanged = entity.Components.CommitChanges();
                if (this.entitiesToAdd.Contains(entity))
                {
                    this.NotifyProcessorsEntityAdded(entity);
                }
                else if (this.entitiesToRemove.Contains(entity))
                {
                    this.NotifyProcessorsEntityRemoved(entity);
                }
                else if (isSomethingChanged)
                {
                    this.NotifyProcessorsEntityChanged(entity);
                }
            }

            this.entitiesToRemove.Clear();
            this.entitiesToAdd.Clear();
        }

        public Entity FindEntityByName(string name)
        {
            for (var i = 0; i < this.entities.Count; i++)
            {
                if (this.entities[i].Name == name)
                    return this.entities[i];
            }

            foreach (var entity in this.entitiesToAdd)
            {
                if (entity.Name == name)
                    return entity;
            }

            return null;
        }

        public List<Entity> FindEntitiesByTag(int tag)
        {
            List<Entity> list;
            if (!this.entityTagsDict.TryGetValue(tag, out list))
            {
                list = new List<Entity>();
                this.entityTagsDict[tag] = list;
            }

            return this.entityTagsDict[tag];
        }

        public List<Entity> FindEntitiesByType<T>()
            where T : Entity
        {
            var list = new List<Entity>();
            for (var i = 0; i < this.entities.Count; i++)
            {
                if (this.entities[i] is T)
                    list.Add(this.entities[i]);
            }

            foreach (var entity in this.entitiesToAdd)
            {
                if (entity is T)
                {
                    list.Add(entity);
                }
            }

            return list;
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return this.entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void AddToTagList(Entity entity)
        {
            var list = this.FindEntitiesByTag(entity.Tag);
            if (!list.Contains(entity))
            {
                list.Add(entity);
            }
        }

        private void RemoveFromTagList(Entity entity)
        {
            List<Entity> list;
            if (this.entityTagsDict.TryGetValue(entity.Tag, out list))
            {
                list.Remove(entity);
            }
        }
        
        private void NotifyProcessorsEntityRemoved(Entity obj)
        {
            this.EntityRemoved?.Invoke(obj);
        }

        private void NotifyProcessorsEntityAdded(Entity obj)
        {
            this.EntityAdded?.Invoke(obj);
        }

        private void NotifyProcessorsEntityChanged(Entity obj)
        {
            this.EntityChanged?.Invoke(obj);
        }
    }
}
