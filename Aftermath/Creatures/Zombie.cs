using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.AI;
using Aftermath.Core;

namespace Aftermath.Creatures
{
    class Zombie : Creature
    {
        static int playermap_generatedTime = -1;
        static HomingField playermap=null;

        bool _skipNextTurn = false;
        public override void DoTurn()
        {
            if (Engine.Instance.TurnSystem.TurnNumber != playermap_generatedTime)
            {
                playermap = new HomingField(Engine.Instance.World, 50, 50);
                playermap.CenterOn(Engine.Instance.Player.Tile);
                playermap.SetHomingTarget(Engine.Instance.Player.Tile);
                playermap.Generate();
                playermap_generatedTime = Engine.Instance.TurnSystem.TurnNumber;
            }

            //skip every other turn, makes zombies slow
            if (_skipNextTurn)
            {
                _skipNextTurn = false;
                return;
            }

            Map.Tile next = playermap.GetNext(Tile);
            if (next != null)
                MoveTo(next);
            else
                Move(Compass.GetRandomCompassDirection());
            //zombies somehow know where the player is and chase
            //MoveTowards(Aftermath.Core.Engine.Instance.Player.Tile);
            _skipNextTurn = true;
            //Move(Compass.GetRandomCompassDirection());
        }

        public override GameTexture Texture
        {
            get
            {
                if (IsAlive)
                {
                    if (_health <= 10)
                        return new GameTexture("zombieInjured", new RectangleI(0, 0, 64, 64));
                    return new GameTexture("zombie", new RectangleI(0, 0, 64, 64));
                }
                else
                    return new GameTexture("zombie_dead", new RectangleI(0, 0, 64, 64));
            }
        }
    }
}
