using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Utils;

namespace Aftermath.Creatures
{
    class Player : Creature
    {
        public Player()
        {
            _health = 200;
        }

        public override bool IsPlayerControlled
        {
            get
            {
                return true;
            }
        }

        public override GameTexture Texture
        {
            get 
            {
                if (IsAlive)
                    return new GameTexture("character", new RectangleI(0, 0, 64, 64));
                return new GameTexture("zombie_dead", new RectangleI(0, 0, 64, 64));
            }
        }


    }
}
