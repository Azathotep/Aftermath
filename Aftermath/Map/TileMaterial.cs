using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Aftermath.Core;
using Aftermath.Utils;

namespace Aftermath.Map
{
    public abstract class TileMaterial
    {
        /// <summary>
        /// Returns the texture name for a tile with this tile type
        /// </summary>
        /// <param name="tile">tile to return texture for</param>
        /// <param name="rotation">output parameter, returns the angle the texture should be rotated to draw</param>
        /// <returns></returns>
        public abstract string GetTexture(Tile tile, out float rotation);

        /// <summary>
        /// Returns true if the tile blocks visible light, eg obstructs vision
        /// </summary>
        public virtual bool IsOpaque 
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether the tile type is solid, eg obstructs movement
        /// </summary>
        public virtual bool IsSolid 
        {
            get
            {
                return false;
            }
        }
    }

    class Wall : TileMaterial
    {
        public override string GetTexture(Tile tile, out float rotation)
        {
            rotation = 0;
            //if the tile to the south of this tile is empty and the player has already seen it then the texture
            //to draw is differnet (thin wall) because the player remembers what is on the otherside of the wall
            bool thinWall = false;
            Tile south = tile.GetNeighbour(CompassDirection.South);
            if (south != null && !south.Material.IsOpaque && south.Material.GetType() != typeof(Door) && Engine.Instance.PlayerSeenTiles.Contains(south))
                thinWall = true;
            if (thinWall)
                return "house.northwall";
            return "house.solidwall";
        }

        public override bool IsSolid
        {
            get
            {
                return true;
            }
        }

        public override bool IsOpaque
        {
            get
            {
                return true;
            }
        }
    }

    class Road : TileMaterial
    {
        RoadType _type;
        public Road(RoadType type)
        {
            _type = type;
        }

        public override string GetTexture(Tile tile, out float rotation)
        {
            rotation = 0;
            switch (_type)
            {
                case RoadType.Concrete:
                    return "road.road1";
                case RoadType.Line:
                default:
                    string ret = "road.roadLinesHz";
                    Tile north = tile.GetNeighbour(CompassDirection.North);
                    Tile south = tile.GetNeighbour(CompassDirection.South);
                    Tile east = tile.GetNeighbour(CompassDirection.North);
                    Tile west = tile.GetNeighbour(CompassDirection.South);
                        
                    bool[] connected = new bool[4];
                    foreach (CompassDirection direction in Compass.CardinalDirections)
                    {
                        Tile neighbour = tile.GetNeighbour(direction);
                        if (neighbour == null)
                            continue;
                        Road nroad = neighbour.Material as Road;
                        if (nroad != null && nroad._type == RoadType.Line)
                            connected[(int)direction] = true;
                    }
                    if (connected[(int)CompassDirection.North] && connected[(int)CompassDirection.East])
                    {
                        ret = "road.corner";
                        rotation = MathHelper.Pi + MathHelper.PiOver2;
                    }
                    else if (connected[(int)CompassDirection.North] && connected[(int)CompassDirection.West])
                    {
                        ret = "road.corner";
                        rotation = MathHelper.Pi;
                    }
                    else if (connected[(int)CompassDirection.South] && connected[(int)CompassDirection.East])
                    {
                        ret = "road.corner";
                        rotation = 0;
                    }
                    else if (connected[(int)CompassDirection.South] && connected[(int)CompassDirection.West])
                    {
                        ret = "road.corner";
                        rotation = MathHelper.PiOver2;
                    }
                    else if (connected[(int)CompassDirection.North] || connected[(int)CompassDirection.South])
                    {
                        ret = "road.roadLinesHz";
                        rotation = MathHelper.PiOver2;
                    }
                    return ret;
            }
        }
    }

    class Floor : TileMaterial
    {
        FloorType _type;
        public Floor(FloorType type)
        {
            _type = type;
        }

        public FloorType Type
        {
            get
            {
                return _type;
            }
        }

        public override string GetTexture(Tile tile, out float rotation)
        {
            rotation = 0;
            switch (_type)
            {
                case FloorType.Carpet:
                    return "house.carpet";
                case FloorType.Pavement:
                default:
                    return "steel.floor";
            }
        }
    }

    class Door : TileMaterial
    {
        bool _isOpen = true;

        public override bool IsOpaque
        {
            get
            {
                return !_isOpen;
            }
        }

        public override bool IsSolid
        {
            get
            {
                return !_isOpen;
            }
        }

        public override string GetTexture(Tile tile, out float rotation)
        {
            rotation = 0;
            if (_isOpen)
                return "house.opendoor";
            else
                return "house.shutdoor";
        }

        public bool IsOpen 
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
            }
        }
    }
}
