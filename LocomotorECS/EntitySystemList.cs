namespace LocomotorECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class EntitySystemList : IEnumerable<EntitySystem>
    {
        private readonly List<EntitySystem> processors = new List<EntitySystem>();

        public EntitySystemList(EntityList entityList)
        {
            entityList.EntityRemoved += this.NotifyEntityRemoved;
            entityList.EntityAdded += this.NotifyEntityAdded;
            entityList.EntityChanged += this.NotifyEntityChanged;
        }

        public void Add( EntitySystem processor )
        {
            this.processors.Add( processor );
        }

        public void Remove( EntitySystem processor )
        {
            this.processors.Remove( processor );
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
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].Begin();
            }
        }

        public void NotifyDoAction(TimeSpan gameTime)
        {
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].DoAction(gameTime);
            }
        }

        public void NotifyEnd()
        {
            for (var i = 0; i < this.processors.Count; i++)
            {
                this.processors[i].End();
            }
        }

        public IEnumerator<EntitySystem> GetEnumerator()
        {
            return this.processors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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

