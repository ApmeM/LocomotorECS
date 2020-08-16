namespace LocomotorECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using LocomotorECS.Utils;

    public class EntitySystemList
    {
        private List<EntitySystem> processors = new List<EntitySystem>();
        private readonly Dictionary<EntitySystem, List<EntitySystem>> dependencies = new Dictionary<EntitySystem, List<EntitySystem>>();
        private bool needSorting;

        public EntitySystemList(EntityList entityList)
        {
            entityList.EntityRemoved += this.NotifyEntityRemoved;
            entityList.EntityAdded += this.NotifyEntityAdded;
            entityList.EntityChanged += this.NotifyEntityChanged;
        }

        public void AddExecutionOrder<TBefore, TAfter>()
            where TBefore : EntitySystem where TAfter : EntitySystem
        {
            var executedBefore = this.Get<TBefore>();
            var executedAfter = this.Get<TAfter>();

            if (!this.dependencies.ContainsKey(executedBefore))
            {
                this.dependencies[executedBefore] = new List<EntitySystem>();
            }

            this.dependencies[executedBefore].Add(executedAfter);
            this.needSorting = true;
        }

        public void RemoveExecutionOrder<TBefore, TAfter>()
            where TBefore : EntitySystem where TAfter : EntitySystem
        {
            var executedBefore = this.Get<TBefore>();
            var executedAfter = this.Get<TAfter>();

            if (!this.dependencies.ContainsKey(executedBefore))
            {
                return;
            }

            this.dependencies[executedBefore].Remove(executedAfter);
        }

        public void Add(EntitySystem processor)
        {
            this.processors.Add(processor);
        }

        public void Remove(EntitySystem processor)
        {
            this.processors.Remove(processor);
        }

        public T Get<T>() where T : EntitySystem
        {
            for (var i = 0; i < this.processors.Count; i++)
            {
                var processor = this.processors[i];
                if (processor is T)
                    return processor as T;
            }

            return null;
        }

        public void NotifyBegin()
        {
            this.EnsureSorted();
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].Begin();
            }
        }

        public void NotifyDoAction(TimeSpan gameTime)
        {
            this.EnsureSorted();
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].DoAction(gameTime);
            }
        }

        public void NotifyEnd()
        {
            this.EnsureSorted();
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].End();
            }
        }

        private void EnsureSorted()
        {
            if (!this.needSorting)
            {
                return;
            }

            this.needSorting = false;
            this.processors = DfsSort.Sort(this.processors, this.dependencies);
        }

        private void NotifyEntityChanged(Entity entity)
        {
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].NotifyEntityChanged(entity);
            }
        }

        private void NotifyEntityAdded(Entity entity)
        {
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].NotifyEntityAdded(entity);
            }
        }

        private void NotifyEntityRemoved(Entity entity)
        {
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].NotifyEntityRemoved(entity);
            }
        }
    }
}

