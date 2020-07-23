namespace LocomotorECS
{
    using System;

    public abstract class EntitySystem
    {
        protected virtual void OnEntityChanged(Entity entity)
        {
        }

        protected virtual void OnEntityAdded(Entity entity)
        {
        }

        protected virtual void OnEntityRemoved(Entity entity)
        {
        }

        internal void NotifyEntityChanged(Entity entity)
        {
            this.OnEntityChanged(entity);
        }

        internal void NotifyEntityAdded(Entity entity)
        {
            this.OnEntityAdded(entity);
        }

        internal void NotifyEntityRemoved(Entity entity)
        {
            this.OnEntityRemoved(entity);
        }

        public virtual void Begin()
        {
        }

        public virtual void DoAction(TimeSpan gameTime)
        {
        }

        public virtual void End()
        {
        }
    }
}