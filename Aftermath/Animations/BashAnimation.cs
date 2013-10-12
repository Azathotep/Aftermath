using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Utils;
using Microsoft.Xna.Framework;
using Aftermath.Creatures;
using Aftermath.Map;
using Aftermath.Rendering;
using Aftermath.Core;

namespace Aftermath.Animations
{
    /// <summary>
    /// Animation to indicate the sound of a tile being bashed (eg door)
    /// </summary>
    class BashAnimation : Animation
    {
        int _lifetime = 10;
        Tile _tile;
        public BashAnimation(Tile tile)
        {
            _tile = tile;
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
            renderer.Draw(Engine.Instance.TextureManager.GetTexture("overlay.blood"), new RectangleF(_tile.X, _tile.Y, width, width), 0.2f, 0, new Vector2F(0.5f, 0.5f), new Color(0, 0, 0, 0.8f));
        }
    }
}
