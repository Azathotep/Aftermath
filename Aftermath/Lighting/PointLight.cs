using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Map;
using Microsoft.Xna.Framework;

namespace Aftermath.Lighting
{
    /// <summary>
    /// Encapsulates a light that emits from a point on the map
    /// </summary>
    public class PointLight
    {
        public PointLight(Tile location, int brightness, Color color)
        {
            Location = location;
            Brightness = brightness;
            Color = color;
        }

        bool _on;
        public bool On
        {
            get
            {
                return _on;
            }
            set
            {
                _on = value;
            }
        }

        HashSet<Tile> _lightField = new HashSet<Tile>();
        /// <summary>
        /// Returns the tiles that are currently visible to the light (tiles that the light is shining on)
        /// </summary>
        public HashSet<Tile> LightField
        {
            get
            {
                return _lightField;
            }
        }

        public Tile Location;
        public int Brightness;
        public Color Color;

        /// <summary>
        /// Recalculates the light field of this light. This method should be called when a nearby door is opened or closed
        /// or a light changes position.
        /// </summary>
        public void RecalculateLightfield()
        {
            //calcualte the new light field
            HashSet<Tile> newField = Engine.Instance.GetFov(Location, Brightness);

            //generate the set of tiles that either used to be the in lightfield or now are the lightfield
            //these tiles need to recalculate their point lighting
            HashSet<Tile> toRecalculate = LightField;
            toRecalculate.UnionWith(newField);

            //before recalculating at each tile set the lightfield to new
            _lightField = newField;

            foreach (Tile tile in toRecalculate)
                tile.RecalculatePointLighting();
        }

        /// <summary>
        /// Returns the brightness of a tile caused by this light alone (from 0 to 1)
        /// </summary>
        internal float GetBrightnessAt(Tile tile)
        {
            if (!On)
                return 0;
            if (!LightField.Contains(tile))
                return 0;
            //return 0.4f;
            float ret = Math.Max(0, 0.4f * (float)(Brightness * Brightness) / (tile.GetDistanceSquared(Location) + 1));
            if (ret < 0.2f)
                ret = 0;
            return ret;
        }
    }
}
