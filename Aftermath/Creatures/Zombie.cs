using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;

namespace Aftermath.Creatures
{
    class Zombie : Creature
    {
        bool _skipNextTurn = false;
        public override void DoTurn()
        {
            //skip every other turn, makes zombies slow
            if (_skipNextTurn)
            {
                _skipNextTurn = false;
                return;
            }
            //zombies somehow know where the player is and chase
            MoveTowards(Aftermath.Core.Engine.Instance.Player.Tile);
            _skipNextTurn = true;
            //Move(Compass.GetRandomCompassDirection());
        }

        public override GameTexture Texture
        {
            get
            {
                if (IsAlive)
                    return new GameTexture("zombie", new RectangleI(0, 0, 64, 64));
                else
                    return new GameTexture("zombie_dead", new RectangleI(0, 0, 64, 64));
            }
        }
    }
}
