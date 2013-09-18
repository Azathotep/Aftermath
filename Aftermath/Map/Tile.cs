using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Creatures;

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
        /// Whether this tile is opaque (blocks vision)
        /// </summary>
        public bool IsOpaque
        {
            get
            {
                if (Wall == WallType.Door)
                    return false;
                return Wall != WallType.None;
            }
        }

        /// <summary>
        /// Whether the tile is passable (creatures can walk into it, eg walls are not passable)
        /// </summary>
        public bool IsPassable
        {
            get
            {
                if (Wall == WallType.Door)
                    return true;
                return Wall == WallType.None;
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
            if (creature.Tile != null)
                throw new Exception("Creature cannot be placed as it is already placed on a map");
            creature.Place(this);
        }

        /// <summary>
        /// Returns the type of wall on this tile. If no wall is present then the tile is floor.
        /// </summary>
        public WallType Wall = WallType.None;

        public FloorType Floor = FloorType.Tile;

        /// <summary>
        /// Returns whether a specified creature can enter this tile
        /// </summary>
        /// <returns>Whether a creature can enter this tile and if not, why not</returns>
        internal ActionResult CanEnter(Creature creature)
        {
            if (Creature != null)
                return ActionResult.TileOccupied;
            if (!IsPassable)
                return ActionResult.TileBlocked;
            return ActionResult.Ok;
        }

        internal int GetManhattenDistanceFrom(Tile other)
        {
            return Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
        }
    }
}
