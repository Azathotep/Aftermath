using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Map;

namespace Aftermath.Creatures
{
    /// <summary>
    /// base class for all survivors
    /// </summary>
    abstract class Human : Creature
    {
        HashSet<Tile> _zombieViewField = new HashSet<Tile>();
        /// <summary>
        /// Field used by zombies to see if they can see this survivor. Each survivor generates an FOV field using the max
        /// range of the longest sighted zombie. Zombies can then test against this field instead of each zombie
        /// having to generate their own field.
        /// </summary>
        public HashSet<Tile> ZombieViewField
        {
            get
            {
                return _zombieViewField;
            }
        }

        /// <summary>
        /// Regenerates the zombie view field for this human
        /// </summary>
        public void RegenerateZombieViewField()
        {
            int maxZombieViewDistance = 20;
            _zombieViewField = Location.GetVisibleTiles(maxZombieViewDistance);
        }

        public override void PostTurn()
        {
            //regenerate the field after each turn
            //to be perfect it should also regenerate whenever map structure changes, eg a door opens or closes
            //but this approximation is close enough
            RegenerateZombieViewField();
        }
    }
}
