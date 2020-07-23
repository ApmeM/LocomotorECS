namespace LocomotorECS.Matching
{
    using System;
    using System.Text;

    public class Matcher
    {
        private readonly BitSet allSet = new BitSet();
        private readonly BitSet exclusionSet = new BitSet();
        private readonly BitSet oneSet = new BitSet();

        public Matcher All( params Type[] types )
        {
            foreach (var type in types)
            {
                this.allSet.Set( ComponentTypeManager.GetIndexFor( type ) );
            }

            return this;
        }

        public Matcher Exclude( params Type[] types )
        {
            foreach (var type in types)
            {
                this.exclusionSet.Set( ComponentTypeManager.GetIndexFor( type ) );
            }

            return this;
        }

        public Matcher One( params Type[] types )
        {
            foreach (var type in types)
            {
                this.oneSet.Set( ComponentTypeManager.GetIndexFor( type ) );
            }

            return this;
        }

        public static Matcher Empty()
        {
            return new Matcher();
        }

        public bool IsMatched(Entity entity)
        {
            // Check if the entity possesses ALL of the components defined in the aspect.
            if (!this.allSet.IsEmpty())
            {
                for (var i = this.allSet.NextSetBit(0); i >= 0; i = this.allSet.NextSetBit(i + 1))
                {
                    if (!entity.Components.Bits.Get(i))
                    {
                        return false;
                    }
                }
            }

            // If we are STILL interested,
            // Check if the entity possesses ANY of the exclusion components, if it does then the system is not interested.
            if (!this.exclusionSet.IsEmpty() && this.exclusionSet.Intersects(entity.Components.Bits))
            {
                return false;
            }

            // If we are STILL interested,
            // Check if the entity possesses ANY of the components in the oneSet. If so, the system is interested.
            if (!this.oneSet.IsEmpty() && !this.oneSet.Intersects(entity.Components.Bits))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder( 1024 );

            builder.AppendLine( "Matcher:" );
            AppendTypes( builder, " -  Requires the components: ", this.allSet );
            AppendTypes( builder, " -  Has none of the components: ", this.exclusionSet );
            AppendTypes( builder, " -  Has at least one of the components: ", this.oneSet );

            return builder.ToString();
        }

        private static void AppendTypes( StringBuilder builder, string headerMessage, BitSet typeBits )
        {
            var firstType = true;
            builder.Append( headerMessage );
            foreach( var type in ComponentTypeManager.GetTypesFromBits( typeBits ) )
            {
                if( !firstType )
                    builder.Append( ", " );
                builder.Append( type.Name );

                firstType = false;
            }

            builder.AppendLine();
        }
    }
}
