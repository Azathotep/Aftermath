using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;

namespace Aftermath.Creatures
{
    class Player : Creature
    {
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
                return new GameTexture("character", new RectangleI(0, 0, 64, 64));
            }
        }
    }
}
