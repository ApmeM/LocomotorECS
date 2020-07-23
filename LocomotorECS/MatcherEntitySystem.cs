namespace LocomotorECS
{
    using System.Collections.Generic;

    using LocomotorECS.Matching;

    public abstract class MatcherEntitySystem : EntitySystem
    {
        public Matcher Matcher { get; }
        
        protected List<Entity> MatchedEntities { get; set; } = new List<Entity>();

        protected MatcherEntitySystem()
        {
            this.Matcher = Matcher.Empty();
        }

        protected MatcherEntitySystem( Matcher matcher )
        {
            this.Matcher = matcher;
        }

        protected sealed override void OnEntityChanged(Entity entity)
        {
            var contains = this.MatchedEntities.Contains(entity);
            var interest = this.Matcher.IsMatched(entity);

            if (interest)
            {
                if (contains)
                {
                    this.OnMatchedEntityChanged(entity);
                }else
                {
                    this.MatchedEntities.Add(entity);
                    this.OnMatchedEntityAdded(entity);
                }
            }
            else if (contains)
            {
                this.MatchedEntities.Remove(entity);
                this.OnMatchedEntityRemoved(entity);
            }
        }

        protected sealed override void OnEntityAdded(Entity entity)
        {
            var interest = this.Matcher.IsMatched(entity);
            if (interest)
            {
                this.MatchedEntities.Add(entity);
                this.OnMatchedEntityAdded(entity);
            }
        }

        protected sealed override void OnEntityRemoved( Entity entity )
        {
            var interest = this.Matcher.IsMatched(entity);
            if (interest)
            {
                this.MatchedEntities.Remove(entity);
                this.OnMatchedEntityRemoved(entity);
            }
        }

        protected virtual void OnMatchedEntityAdded(Entity entity)
        {
        }

        protected virtual void OnMatchedEntityChanged(Entity entity)
        {
        }

        protected virtual void OnMatchedEntityRemoved( Entity entity )
        {
        }
    }
}

