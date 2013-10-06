using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;

namespace Aftermath.Map
{
    /// <summary>
    /// Represents a light on the map
    /// </summary>
    public class Light
    {
        public HashSet<Tile> LightField = new HashSet<Tile>();

        public Tile Location;
        public int Brightness = 5;

        /// <summary>
        /// Recalculates the light field of this light. This method is called when a nearby door is opened or closed.
        /// </summary>
        public void RecalculateLightfield()
        {
            LightField = Engine.Instance.GetFov(Location, Brightness);
        }

        /// <summary>
        /// Returns the brightness of a tile from this light from 0 to 1
        /// </summary>
        internal float GetBrightnessAt(Tile tile)
        {
            if (!LightField.Contains(tile))
                return 0;
            return Math.Max(0, (float)Brightness / (tile.GetDistanceSquared(Location) + 1));
            //return Math.Max(((float)Brightness - (float)tile.GetManhattenDistanceFrom(Location)) / Brightness, 0);
        }
    }
}
