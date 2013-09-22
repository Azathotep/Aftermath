using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Aftermath.Creatures;
using Aftermath.Map;
using Aftermath.Rendering;
using Aftermath.Core;

namespace Aftermath.Animations
{
    /// <summary>
    /// Bleed animation to indicate a creature has taken damage
    /// </summary>
    class BleedAnimation : Animation
    {
        int _lifetime = 10;
        Creature _creature;
        public BleedAnimation(Creature creature)
        {
            _creature = creature;
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
            float width = (float)_lifetime / 15;
            renderer.Draw(Engine.Instance.TextureManager.GetTexture("overlay.blood"), new RectangleF(_creature.Location.X, _creature.Location.Y, width, width), 0.2f, 0, new Vector2F(0.5f, 0.5f), new Color(1, 1, 1, 0.8f));
        }
    }
}
