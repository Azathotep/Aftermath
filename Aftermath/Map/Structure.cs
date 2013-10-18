using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;

namespace Aftermath.Map
{
    /// <summary>
    /// Represents a structure on a tile such as a door or generator
    /// </summary>
    public abstract class Structure
    {
        /// <summary>
        /// Returns true if the structure blocks visible light, eg obstructs vision
        /// </summary>
        public virtual bool BlocksLight
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the texture name for drawing this structure
        /// </summary>
        public abstract GameTexture Texture
        {
            get;
        }

        /// <summary>
        /// Returns whether the structure type is solid, eg obstructs movement
        /// </summary>
        public virtual bool IsSolid
        {
            get
            {
                return false;
            }
        }

        public Tile Tile
        {
            get;
            set;
        }

        /// <summary>
        /// Applies damage to the structure.
        /// </summary>
        /// <param name="damageAmount">amount to damage</param>
        public virtual void Damage(int damageAmount)
        {

        }
    }
}
