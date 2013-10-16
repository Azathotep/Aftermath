using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Creatures;
using Aftermath.Map;
using Aftermath.Items;

namespace Aftermath.Weapons
{
    public abstract class Gun : Item
    {
        public int LoadedAmmo = 6;

        /// <summary>
        /// Fires the gun at a target
        /// </summary>
        /// <param name="firer">who the gun should be fired from</param>
        /// <param name="targetTile">the target</param>
        /// <returns></returns>
        public abstract bool Fire(Creature firer, Tile targetTile);

        internal void Reload()
        {
            LoadedAmmo = 6;
        }
    }
}
