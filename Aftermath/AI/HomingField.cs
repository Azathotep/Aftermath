using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Map;

namespace Aftermath.AI
{
    /// <summary>
    /// Generates a field that can be used by zombies to home in on targets. Zombies can share the same field which allows the number
    /// of zombies to be increased. The city map is too big to generate a field for the entire map. A field is generated around the 
    /// player.
    /// (full description of method http://roguebasin.roguelikedevelopment.org/index.php?title=The_Incredible_Power_of_Dijkstra_Maps)
    /// </summary>
    public class HomingField
    {
        /// <summary>
        /// Sets a tile as a homing target. After generating the field can be used to home on these targets
        /// </summary>
        /// <param name="tile"></param>
        public void AddHomingTarget(Tile tile)
        {
            _distance[tile] = 0;
        }

        Dictionary<Tile, int> _distance = new Dictionary<Tile, int>();

        /// <summary>
        /// Generate the field
        /// </summary>
        public void Generate(int maxDistance, OnTileReachedHandler onTileReachedHandler=null)
        {
            Queue<Tile> toProcess = new Queue<Tile>();
            foreach (Tile tile in _distance.Keys)
                toProcess.Enqueue(tile);

            while (toProcess.Count > 0)
            {
                Tile tile = toProcess.Dequeue();

                int distance = _distance[tile];
                if (distance >= maxDistance)
                    continue;

                foreach (Tile n in tile.GetNeighbours())
                {
                    if (_distance.ContainsKey(n))
                        continue;
                    if (n.IsSolid && !n.ContainsDestructableObstacle)
                        continue;
                    _distance[n] = distance + 1;
                    if (onTileReachedHandler != null)
                        onTileReachedHandler(n, distance + 1);
                    toProcess.Enqueue(n);
                }
            }
        }

        public delegate void OnTileReachedHandler(Tile tile, int distance);

        /// <summary>
        /// Returns the distance of the specified tile from the nearest homing target
        /// </summary>
        public int GetDistanceFromTarget(Tile tile)
        {
            int ret;
            if (_distance.TryGetValue(tile, out ret))
                return ret;
            return 9999;
        }

        /// <summary>
        /// Returns the neighbour of the specified tile which is closest to a homing target
        /// </summary>
        public Tile GetNextTileTowardsTarget(Tile tile)
        {
            foreach (Tile t in tile.GetNeighbours())
            {
                int d = GetDistanceFromTarget(t);
            }

            Tile nearestTile = (from n in tile.GetNeighbours() orderby GetDistanceFromTarget(n) ascending select n).FirstOrDefault();
            if (GetDistanceFromTarget(nearestTile) < GetDistanceFromTarget(tile))
                return nearestTile;
            return null;
        }
    }
}
