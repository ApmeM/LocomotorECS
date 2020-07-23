namespace LocomotorECS
{
    using System;

    using LocomotorECS.Matching;

    /// <summary>
    /// Basic entity processing system. Use this as the base for processing many entities with specific components
    /// </summary>
    public abstract class EntityProcessingSystem : MatcherEntitySystem
    {
        protected EntityProcessingSystem( Matcher matcher ) : base( matcher )
        {
        }

        public override void DoAction(TimeSpan gameTime)
        {
            base.DoAction(gameTime);
            for (var i = 0; i < this.MatchedEntities.Count; i++)
            {
                this.DoAction(this.MatchedEntities[i], gameTime);
            }
        }

        protected virtual void DoAction(Entity entity, TimeSpan gameTime)
        {

        }
    }
}

