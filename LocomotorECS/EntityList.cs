namespace LocomotorECS
{
    using System;
    using System.Collections.Generic;

    public class EntityList
    {
        private readonly List<Entity> entities = new List<Entity>();

        private readonly HashSet<Entity> entitiesToAdd = new HashSet<Entity>();

        private readonly HashSet<Entity> entitiesToRemove = new HashSet<Entity>();

        private readonly Dictionary<int, List<Entity>> entityTagsDict = new Dictionary<int, List<Entity>>();
        private readonly Dictionary<string, Entity> entityNamesDict = new Dictionary<string, Entity>();

        internal event Action<Entity> EntityRemoved;
        internal event Action<Entity> EntityAdded;
        internal event Action<Entity> EntityChanged;

        public void Add(Entity entity)
        {
            this.entitiesToAdd.Add(entity);
            if (entity.Name != null)
            {
                this.entityNamesDict.Add(entity.Name, entity);
            }

            AddToTagList(entity);
        }

        public void Remove(Entity entity)
        {
            this.entitiesToAdd.Remove(entity);
            this.entitiesToRemove.Add(entity);
            RemoveFromTagList(entity);
        }
        
        public void CommitChanges()
        {
            if (this.entitiesToRemove.Count > 0)
            {
                foreach (var entity in this.entitiesToRemove)
                {
                    this.RemoveFromTagList(entity);
                    this.entities.Remove(entity);
                    if (entity.Name != null)
                    {
                        this.entityNamesDict.Remove(entity.Name);
                    }
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
            if (name == null)
            {
                return null;
            }

            if (!this.entityNamesDict.ContainsKey(name))
            {
                return null;
            }

            return this.entityNamesDict[name];
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

        private void AddToTagList(Entity entity)
        {
            if (entity.Tag == 0)
            {
                return;
            }

            var list = this.FindEntitiesByTag(entity.Tag);
            if (!list.Contains(entity))
            {
                list.Add(entity);
            }
        }

        private void RemoveFromTagList(Entity entity)
        {
            if (entity.Tag == 0)
            {
                return;
            }

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
