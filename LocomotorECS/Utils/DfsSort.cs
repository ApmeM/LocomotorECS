namespace LocomotorECS.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class DfsSort
    {
        private static int Dfs<T>(T current, Dictionary<T, List<T>> edges, Dictionary<T, int> result)
        {
            result[current] = 0;

            if (edges.ContainsKey(current))
            {
                foreach (var dependent in edges[current])
                {
                    if (result.ContainsKey(dependent))
                    {
                        result[current] = Math.Max(result[current], result[dependent] + 1);
                        continue;
                    }

                    result[current] = Math.Max(result[current], Dfs(dependent, edges, result) + 1);
                }
            }

            return result[current];
        }

        public static ILookup<int, T> Sort<T>(List<T> items, Dictionary<T, List<T>> edges)
        {
            var result = new Dictionary<T, int>(items.Count);

            foreach (var current in items)
            {
                if (result.ContainsKey(current))
                {
                    continue;
                }

                Dfs(current, edges, result);
            }

            return result.OrderByDescending(a => a.Value).ToLookup(a => a.Value, a => a.Key);
        }
    }
}