using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;

namespace Aftermath.Creatures
{
    class Zombie : Creature
    {
        public override void DoTurn()
        {
            Move(Compass.GetRandomCompassDirection());
        }

        public override GameTexture Texture
        {
            get
            {
                return new GameTexture("zombie", new RectangleI(0, 0, 64, 64));
            }
        }
    }
}
