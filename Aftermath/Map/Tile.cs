using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Creatures;
using Aftermath.AI.Navigation;
using Aftermath.Utils;
using Aftermath.Lighting;
using Microsoft.Xna.Framework;

namespace Aftermath.Map
{
    /// <summary>
    /// Represents a single tile in the world
    /// </summary>
    public class Tile
    {
        World _world;
        public Tile(World world, int x, int y)
        {
            X = x;
            Y = y;
            _world = world;
        }

        public int X;
        public int Y;

        Creature _creature=null;
        Creature _corpse = null;

        protected TileMaterial _type;
        internal TileMaterial Material
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Creature occupying this tile
        /// </summary>
        public Creature Creature
        {
            get
            {
                return _creature;
            }
            set
            {
                _creature = value;
            }
        }

        /// <summary>
        /// Returns the set of tiles visible from this tile
        /// </summary>
        /// <param name="sightRange">maximum vision range</param>
        /// <param name="lightThreshold">minimum level of light a tile must have to be visible</param>
        /// <returns></returns>
        public HashSet<Tile> GetVisibleTiles(int sightRange, float lightThreshold = 0)
        {
            HashSet<Tile> ret = Engine.Instance.GetFov(this, sightRange);
            if (lightThreshold > 0)
            {
                foreach (Tile tile in ret.ToArray())
                    if (tile.GetLighting(this).Brightness < lightThreshold)
                        ret.Remove(tile);
            }
            return ret;
        }

        //TODO only one corpse saved?
        /// <summary>
        /// Corpse occupying this tile
        /// </summary>
        public Creature Corpse
        {
            get
            {
                return _corpse;
            }
            set
            {
                _corpse = value;
            }
        }

        /// <summary>
        /// Returns the neighbour tile of this tile in the specified direction
        /// </summary>
        /// <returns>null if no tile exists in the direction specified (edge of map)</returns>
        public Tile GetNeighbour(CompassDirection direction)
        {
            Vector2I offset = Compass.DirectionToVector(direction);
            return GetRelativeTile(offset);
        }

        public List<Tile> GetNeighbours()
        {
            List<Tile> ret = new List<Tile>();
            foreach (CompassDirection direction in Compass.CompassDirections)
            {
                Tile n = GetNeighbour(direction);
                if (n == null)
                    continue;
                ret.Add(n);
            }
            return ret;
        }

        /// <summary>
        /// Returns the tile at the given offset from this tile
        /// </summary>
        /// <returns>null if no tile exists in the direction specified (edge of map)</returns>
        public Tile GetRelativeTile(Vector2I offset)
        {
            return _world.GetTile(X + offset.X, Y + offset.Y);
        }

        /// <summary>
        /// Places a creature on this tile. The creature should not be part of the map already.
        /// </summary>
        public void PlaceCreature(Creature creature)
        {
            if (creature.Location != null)
                throw new Exception("Creature cannot be placed as it is already placed on a map");
            creature.Place(this);
        }

        /// <summary>
        /// Returns whether a specified creature can enter this tile
        /// </summary>
        /// <returns>Whether a creature can enter this tile and if not, why not</returns>
        internal ActionResult CanEnter(Creature creature)
        {
            if (Creature != null)
                return ActionResult.TileOccupied;
            if (Material.IsSolid)
                return ActionResult.TileBlocked;
            return ActionResult.Ok;
        }

        internal int GetManhattenDistanceFrom(Tile other)
        {
            return Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
        }

        internal int GetChebyshevDistanceFrom(Tile other)
        {
            return Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y));
        }

        /// <summary>
        /// Returns the current level of sunlight at this tile
        /// </summary>
        public float Sunlight
        {
            get
            {
                //TODO inside tiles should always have 0 sunlight
                return _world.Sunlight;
            }
        }

        /// <summary>
        /// Returns the ambient lighting (ambient + sunlight) at this tile. Ambient lighting changes a lot for an outsid tile
        /// </summary>
        Light AmbientLighting
        {
            get
            {
                return new Light(Sunlight + 0.2f, new Color(Sunlight + 0.2f, Sunlight + 0.2f, Sunlight + 0.2f));
            }
        }

        Light _pointLighting = Light.PitchBlack;
        /// <summary>
        /// Returns the point lighting at this tile. Eg flashlights, bulbs. Point lighting is expensive to compute so is
        /// cached.
        /// </summary>
        Light PointLighting
        {
            get
            {
                return _pointLighting;
            }
        }

        /// <summary>
        /// Returns the total lighting (ambient + point) of this tile. Only relevant for floor tiles. Wall tiles
        /// share the light of a neighbouring floor tile
        /// </summary>
        Light TotalLighting
        {
            get
            {
                Color totalColor = new Color(AmbientLighting.Color.R + PointLighting.Color.R,
                    AmbientLighting.Color.G + PointLighting.Color.G,
                    AmbientLighting.Color.B + PointLighting.Color.B);
                return new Light(AmbientLighting.Brightness + PointLighting.Brightness, totalColor);
            }
        }

        /// <summary>
        /// Recalculates the point lighting of the tile from all nearby point lights
        /// </summary>
        public void RecalculatePointLighting()
        {
            _pointLighting = Light.PitchBlack;
            float totalBr = 0;
            foreach (PointLight light in _world.Lights)
            {
                totalBr = light.GetBrightnessAt(this);
                Color lightColor = light.Color;
                lightColor *= totalBr * 0.5f;
                if (totalBr > 0)
                {
                    _pointLighting.Color.R = (byte)Math.Min(_pointLighting.Color.R + lightColor.R, 255);
                    _pointLighting.Color.G = (byte)Math.Min(_pointLighting.Color.G + lightColor.G, 255);
                    _pointLighting.Color.B = (byte)Math.Min(_pointLighting.Color.B + lightColor.B, 255);
                    _pointLighting.Brightness += totalBr;
                }
            }
        }

        /// <summary>
        /// Returns a path of tiles from this tile to a target tile
        /// </summary>
        public Tile[] GetTraversablePath(Tile target)
        {
            AStarAlgorithm astar = new AStarAlgorithm();
            var path = astar.FindPath(new NavigatableNode(this), new NavigatableNode(target));
            return (from p in path select ((NavigatableNode)p).Tile).ToArray();
        }

        internal int GetDistanceSquared(Tile Location)
        {
            return (Location.X - X)*(Location.X - X) + (Location.Y - Y)*(Location.Y - Y);
        }

        /// <summary>
        /// Returns the light level/brightness of this tile. The light level depends on the eye position 
        /// if the tile is opaque.
        /// </summary>
        /// <param name="eyePosition"></param>
        /// <returns></returns>
        internal Light GetLighting(Tile eyePosition)
        {
            //if tile is floor then brightness is the comination of sunlight and point lights on this tile
            if (!Material.IsOpaque)
                return TotalLighting;
            //tile blocks light (eg a wall), so the brightness depends on which side of the wall the eye position is
            //from inside a well lit room the walls appear bright, from outside in the night the walls appear dark
            //use the brightness of a floor neighbour
            //TODO refactor
            Light light;
            if (eyePosition.X < X)
            {
                light = GetNeighbourLighting(CompassDirection.West);
                if (light.Brightness > 0)
                    return light;

                if (eyePosition.Y < Y)
                {
                    light = GetNeighbourLighting(CompassDirection.NorthWest);
                    if (light.Brightness > 0)
                        return light;
                }
                if (eyePosition.Y > Y)
                {
                    light = GetNeighbourLighting(CompassDirection.SouthWest);
                    if (light.Brightness > 0)
                        return light;
                }
            }

            if (eyePosition.X > X)
            {
                light = GetNeighbourLighting(CompassDirection.East);
                if (light.Brightness > 0)
                    return light;
                if (eyePosition.Y < Y)
                {
                    light = GetNeighbourLighting(CompassDirection.NorthEast);
                    if (light.Brightness > 0)
                        return light;
                }
                if (eyePosition.Y > Y)
                {
                    light = GetNeighbourLighting(CompassDirection.SouthEast);
                    if (light.Brightness > 0)
                        return light;
                }
            }
            if (eyePosition.Y < Y)
            {
                light = GetNeighbourLighting(CompassDirection.North);
                if (light.Brightness > 0)
                    return light;
            }
            if (eyePosition.Y > Y)
            {
                light = GetNeighbourLighting(CompassDirection.South);
                if (light.Brightness > 0)
                    return light;
            }
            return Light.PitchBlack;
        }

        /// <summary>
        /// Returns the light level of a neighbouring tile IF the tile is transparent to light
        /// </summary>
        Light GetNeighbourLighting(CompassDirection direction)
        {
            Tile n = GetNeighbour(direction);
            if (n == null || n.Material.IsOpaque)
                return Light.PitchBlack;
            return n.TotalLighting;
        }
    }
}
