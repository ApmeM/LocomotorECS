namespace LocomotorECS.Matching
{
    using System;
    using System.Collections.Generic;

    internal static class ComponentTypeManager
    {
        private static readonly Dictionary<Type, int> ComponentTypesMask = new Dictionary<Type, int>();
        
        public static int GetIndexFor( Type type )
        {
            if( !ComponentTypesMask.ContainsKey(type))
            {
                ComponentTypesMask[type] = ComponentTypesMask.Count;
            }

            return ComponentTypesMask[type];
        }

        public static IEnumerable<Type> GetTypesFromBits( BitSet bits )
        {
            foreach( var keyValuePair in ComponentTypesMask )
            {
                if (bits.Get(keyValuePair.Value))
                {
                    yield return keyValuePair.Key;
                }
            }   
        }
    }
}

