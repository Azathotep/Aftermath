using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Creatures;
using Aftermath.Map;
using Aftermath.Animations;

namespace Aftermath.Weapons
{
    public class Pistol9mm : Gun
    {
        public override bool Fire(Creatures.Creature firer, Map.Tile targetTile)
        {
            if (LoadedAmmo < 0)
                return false;
            LoadedAmmo--;
            Engine.Instance.AnimationManager.StartAnimation(new MuzzleFlashAnimation(firer));
            Sound.Emit(Core.Engine.Instance.Player.Location, 20);
            if (targetTile.Creature != null)
                targetTile.Creature.PutDamage(10);
            return true;
        }
    }
}
