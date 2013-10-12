using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Utils;
using Aftermath.Lighting;

namespace Aftermath.Creatures
{
    class Player : Human
    {
        public Player()
        {
            _health = 50;
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

        public override void PostTurn()
        {
            //get the torch to follow the player
            //refactor this
            Flashlight.Location = Location;
            Flashlight.RecalculateLightfield();

            Location.DropScent(500);

            base.PostTurn();
        }

        public PointLight Flashlight;
    }
}
