using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Utils;
using Aftermath.Lighting;
using Aftermath.Items;

namespace Aftermath.Creatures
{
    public class Player : Human
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
            //update the flashlight to the player's new position
            if (Flashlight != null)
                Flashlight.SetPosition(Location);

            Location.DropScent(500);

            //if (Dice.Next(50) == 0)
            //    Map.Sound.Emit(Location, 100);

            base.PostTurn();
        }

        Flashlight _flashlight;
        public Flashlight Flashlight
        {
            get
            {
                return _flashlight;
            }
            set
            {
                _flashlight = value;
                _flashlight.SetPosition(Location);  
            }
        }

        internal void ToggleFlashlight()
        {
            if (Flashlight == null)
                return;
            Flashlight.On = !Flashlight.On;
            EndTurn();
        }
    }
}
