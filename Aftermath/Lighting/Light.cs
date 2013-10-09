using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Aftermath.Lighting
{
    /// <summary>
    /// A colored light. Think about renaming this.
    /// </summary>
    public struct Light
    {
        public Color Color;
        public float Brightness;

        //hmm so we can have a bright black light?
        public Light(float brightness, Color color)
        {
            Brightness = brightness;
            Color = color;
        }

        static Light _pitchBlack = new Light(0, Color.Black);
        public static Light PitchBlack 
        {
            get
            {
                return _pitchBlack;
            }
        }
    }
}
