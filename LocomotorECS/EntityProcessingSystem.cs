namespace LocomotorECS
{
    using System;
    using System.Threading.Tasks;

    using LocomotorECS.Matching;

    /// <summary>
    /// Basic entity processing system. Use this as the base for processing many entities with specific components
    /// </summary>
    public abstract class EntityProcessingSystem : MatcherEntitySystem
    {
        protected bool UseParallelism = false;

        protected EntityProcessingSystem( Matcher matcher ) : base( matcher )
        {
        }

        public override void DoAction(TimeSpan gameTime)
        {
            base.DoAction(gameTime);
            if (this.UseParallelism)
            {
                Parallel.ForEach(this.MatchedEntities, a => this.DoAction(a, gameTime));
            }
            else
            {
                for (var i = 0; i < this.MatchedEntities.Count; i++)
                {
                    this.DoAction(this.MatchedEntities[i], gameTime);
                }
            }
        }

        protected virtual void DoAction(Entity entity, TimeSpan gameTime)
        {

        }
    }
}

