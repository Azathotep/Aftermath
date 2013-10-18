using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Lighting;

namespace Aftermath.Map
{
    /// <summary>
    /// Represents the game world, a grid of tiles and provides methods to query them
    /// </summary>
    public class World
    {
        FovRecursiveShadowcast _fov;

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
            _fov = new FovRecursiveShadowcast(this);
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public FovRecursiveShadowcast Fov
        {
            get
            {
                return _fov;
            }
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
                if (tile.IsSolid || tile.Creature != null)
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
            return tile.BlocksLight;
        }

        int _timeOfDay;
        public int TimeOfDay
        {
            get
            {
                return _timeOfDay;
            }
            set
            {
                _timeOfDay = value;
                if (_timeOfDay >= 1440)
                    _timeOfDay -= 1440;
            }
        }

        /// <summary>
        /// Returns the current level of sunlight in the world which depends on the time of day
        /// </summary>
        public float Sunlight
        {
            get
            {
                //TODO this is calculated several times on every frame for every tile drawn. Maybe cache this.
                if (_timeOfDay >= 600) //10am
                {
                    if (_timeOfDay <= 960) //4pm 
                        return 1;
                    if (_timeOfDay <= 1200) //8pm
                        return 1 - ((float)_timeOfDay - 960) / (1200 - 960);
                    else
                        return 0;
                }
                else
                {
                    if (_timeOfDay <= 420) //7am
                        return 0;
                    else
                        return ((float)_timeOfDay - 420) / 200;
                }
            }
        }

        List<PointLight> _lights = new List<PointLight>();
        /// <summary>
        /// Returns the list of point light in the world
        /// </summary>
        public List<PointLight> Lights
        {
            get
            {
                return _lights;
            }
        }
    }
}
