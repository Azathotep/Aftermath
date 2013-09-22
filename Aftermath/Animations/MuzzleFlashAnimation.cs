using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Creatures;
using Aftermath.Map;
using Aftermath.Rendering;
using Aftermath.Core;
using Aftermath.Utils;

namespace Aftermath.Animations
{
    /// <summary>
    /// Muzzle flash animation to be drawn on gun when gun is fired
    /// </summary>
    class MuzzleFlashAnimation : Animation
    {
        int _lifetime = 5;
        Creature _firer;
        public MuzzleFlashAnimation(Creature firer)
        {
            _firer = firer;
        }

        public override bool Update()
        {
            _lifetime--;
            if (_lifetime <= 0)
                return false;
            return true;
        }

        public override void Render(XnaRenderer renderer)
        {
            float width = 0.5f;
            float x = _firer.Location.X + 0.4f;
            if (!_firer.FacingRight)
                x = _firer.Location.X - 0.4f;
            renderer.Draw(Engine.Instance.TextureManager.GetTexture("overlay.fireball"), new RectangleF(x, _firer.Location.Y - 0.1f, width, width), 0.2f, 0, new Vector2F(0.5f, 0.5f));
        }
    }
}
