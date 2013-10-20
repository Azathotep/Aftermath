using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Aftermath.Core;
using Aftermath.Utils;
using Aftermath.Animations;

namespace Aftermath.Map
{
    /// <summary>
    /// Provides methods to obtain information about different tile materials
    /// </summary>
    class MaterialInfo
    {
        /// <summary>
        /// Returns the name of the texture to use for a given tile
        /// </summary>
        /// <param name="tile">tile to obtain texture for</param>
        /// <param name="rotation">returns the rotation to apply to the texture before drawing</param>
        /// <returns>name of texture</returns>
        public static string GetTextureName(Tile tile, out float rotation)
        {
            rotation = 0;
            bool thinWall = false;
            if (IsWall(tile.Material))
            {
                //if the tile to the south of this tile is empty and the player has already seen it then the texture
                //to draw is differnet (thin wall) because the player remembers what is on the otherside of the wall
                Tile south = tile.GetNeighbour(CompassDirection.South);
                if (south != null)
                {
                    Door door = south.Structure as Door;
                    //if southern tile does not block light, has been seen and isn't a door
                    if (!south.BlocksLight && south.HasBeenSeen && door == null)
                        thinWall = true;
                }
            }
            switch (tile.Material)
            {
                case MaterialType.WoodWall:
                    if (thinWall)
                        return "house.northwall";
                    return "house.solidwall";
                case MaterialType.Glass:
                    return "overlay.gauze";
                case MaterialType.Pavement:
                    return "steel.floor";
                case MaterialType.Carpet:
                    return "house.carpet";
                case MaterialType.Road:
                    return "road.road1";
            }
            return "";
        }

        /// <summary>
        /// Returns true if the specified tile material type is solid
        /// </summary>
        public static bool IsSolid(MaterialType type)
        {
            if (type == MaterialType.Glass || type == MaterialType.WoodWall)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the specified tile material type blocks light
        /// </summary>
        public static bool BlocksLight(MaterialType type)
        {
            if (type == MaterialType.WoodWall)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if the specified tile material type is a wall type. This is a special case to indicate the
        /// material must support thin walls
        /// </summary>
        public static bool IsWall(MaterialType type)
        {
            if (type == MaterialType.WoodWall)
                return true;
            return false;
        }
    }
}
