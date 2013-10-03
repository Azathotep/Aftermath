using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aftermath.Map
{
    /// <summary>
    /// Represents the game world, a grid of tiles and provides methods to query them
    /// </summary>
    public class World
    {
        Tile[,] _tiles;
        int _width, _height;
        public World(int width, int height)
        {
            _width = width;
            _height = height;
            _tiles = new Tile[width, height];
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    _tiles[x, y] = new Tile(this, x, y);
        }

        /// <summary>
        /// Returns the tile at the specified coordinates in the world
        /// </summary>
        /// <returns>null if no tile exists at the specified coordinates (off the map)</returns>
        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return null;
            return _tiles[x, y];
        }

        /// <summary>
        /// Returns a random empty tile on the map
        /// </summary>
        public Tile GetRandomEmptyTile()
        {
            //todo make this a bit more robust..
            Random r = new Random();
            for (int i = 0; i < 1000; i++)
            {
                Tile tile = GetTile(r.Next(_width), r.Next(_height));
                if (tile.Material.IsSolid || tile.Creature != null)
                    continue;
                return tile;
            }
            return null;
        }

        internal bool IsTileOpaque(int x, int y)
        {
            Tile tile = GetTile(x, y);
            if (tile == null)
                return true;
            return tile.Material.IsOpaque;
        }
    }
}
