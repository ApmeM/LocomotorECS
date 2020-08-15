namespace LocomotorECS.Utils
{
    using System.Collections.Generic;

    internal class DfsSort
    {
        private static void Dfs<T>(T current, Dictionary<T, List<T>> edges, HashSet<T> used, List<T> result)
        {
            used.Add(current);

            if (edges.ContainsKey(current))
            {
                foreach (var dependent in edges[current])
                {
                    if (used.Contains(dependent))
                    {
                        continue;
                    }

                    Dfs(dependent, edges, used, result);
                }
            }

            result.Add(current);
        }

        public static List<T> Sort<T>(List<T> items, Dictionary<T, List<T>> edges)
        {
            var used = new HashSet<T>();
            var result = new List<T>();

            foreach (var current in items)
            {
                if (used.Contains(current))
                {
                    continue;
                }

                Dfs(current, edges, used, result);
            }

            result.Reverse();

            return result;
        }
    }
}