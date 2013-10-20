using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using System.IO;

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

        /// <summary>
        /// Location of the structure on the map
        /// </summary>
        public Tile Location
        {
            get;
            set;
        }

        /// <summary>
        /// Applies damage to the structure.
        /// </summary>
        /// <param name="damageAmount">amount to damage</param>
        public virtual void Damage(short damageAmount)
        {

        }

        /// <summary>
        /// Returns a unique Id for this stucture for serialization
        /// </summary>
        public abstract StructureType Type
        {
            get;
        }

        protected short _health = 10;

        /// <summary>
        /// Method called to serialize the structure
        /// </summary>
        public virtual void Serialize(BinaryWriter bw)
        {
            bw.Write((short)_health);
        }

        public virtual void Deserialize(BinaryReader br)
        {
            _health = br.ReadInt16();
        }
    }
}
