using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Lighting;
using Aftermath.Map;
using Microsoft.Xna.Framework;

namespace Aftermath.Items
{
    /// <summary>
    /// Flashlight item which the player can carry
    /// </summary>
    public class Flashlight
    {
        PointLight _light; 
        public Flashlight(World world)
        {
            _light = new PointLight(null, 4, new Color(0.5f, 0.5f, 0.3f, 0.5f)); //0.8f, 0.8f, 0.2f));
            world.Lights.Add(_light);
        }

        public bool On
        {
            get
            {
                return _light.On;
            }
            set
            {
                _light.On = value;
            }
        }

        /// <summary>
        /// Sets the position of the flashlight on the map
        /// </summary>
        internal void SetPosition(Tile Location)
        {
            _light.Location = Location;
            _light.RecalculateLightfield();
        }
    }
}
