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
        World _world;
        int[,] _map;

        int _x, _y;
        int _width, _height;
        public HomingField(World world, int width, int height)
        {
            _world = world;
            _width = width;
            _height = height;
            //initialize field with high values
            _map = new int[width, height];
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    _map[x, y] = 9999;
        }

        /// <summary>
        /// Sets a tile as a homing target. After generating the field can be used to home on these targets
        /// </summary>
        /// <param name="tile"></param>
        public void SetHomingTarget(Tile tile)
        {
            SetValue(tile.X - _x, tile.Y - _y, 0);
        }

        /// <summary>
        /// Generate the field
        /// </summary>
        public void Generate()
        {
            bool change = true;
            while (change)
            {
                change = false;
                for (int y = 0; y < _height; y++)
                    for (int x = 0; x < _width; x++)
                    {
                        Tile tile = _world.GetTile(_x + x, _y + y);
                        if (tile == null)
                            continue;
                        if (!tile.IsPassable)
                            continue;
                        int val = _map[x, y];
                        int lowestNeighbour = Math.Min(GetValue(x + 1, y), GetValue(x - 1, y));
                        lowestNeighbour = Math.Min(lowestNeighbour, GetValue(x, y - 1));
                        lowestNeighbour = Math.Min(lowestNeighbour, GetValue(x, y + 1));
                        lowestNeighbour = Math.Min(lowestNeighbour, GetValue(x - 1, y - 1));
                        lowestNeighbour = Math.Min(lowestNeighbour, GetValue(x - 1, y + 1));
                        lowestNeighbour = Math.Min(lowestNeighbour, GetValue(x + 1, y - 1));
                        lowestNeighbour = Math.Min(lowestNeighbour, GetValue(x + 1, y + 1));

                        if (val > lowestNeighbour + 1)
                        {
                            _map[x, y] = lowestNeighbour + 1;
                            change = true;
                        }
                    }
            }
        }

        /// <summary>
        /// Returns the value of a cell in the field
        /// </summary>
        int GetValue(int x, int y)
        {
            if (x < 0 || y < 0)
                return 9999;
            if (x >= _width || y >= _height)
                return 9999;
            return _map[x, y];
        }

        /// <summary>
        /// Sets the value of a cell in the field
        /// </summary>
        void SetValue(int x, int y, int value)
        {
            if (x < 0 || y < 0)
                return;
            if (x >= _width || y >= _height)
                return;
            _map[x, y] = value;
        }

        /// <summary>
        /// Returns the distance of the specified tile from the nearest homing target. Returns 9999 if there is no path
        /// from the tile to any homing target or the tile is outside the field.
        /// </summary>
        /// <param name="tile">tile to obtain field value for</param>
        int GetDistanceToNearestTarget(Tile tile)
        {
            return GetValue(tile.X - _x, tile.Y - _y);
        }

        /// <summary>
        /// Returns the neighbour of the specified tile which is closest to a homing target
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public Tile GetNext(Tile tile)
        {
            //find a neighbour that has a lower field numnber that the specified tile
            int currentValue = GetDistanceToNearestTarget(tile);
            return (from n in tile.GetNeighbours() where n.Creature == null && GetDistanceToNearestTarget(n) < currentValue select n).FirstOrDefault();
        }

        /// <summary>
        /// Centers the field on a tile on the map before generation
        /// </summary>
        /// <param name="tile"></param>
        internal void CenterOn(Tile tile)
        {
            _x = tile.X - _width / 2;
            _y = tile.Y - _height / 2;
        }
    }
}
