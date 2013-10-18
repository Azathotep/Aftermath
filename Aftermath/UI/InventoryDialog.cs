using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Input;
using Aftermath.State;
using Aftermath.Core;
using Aftermath.Utils;
using Aftermath.Items;

namespace Aftermath.UI
{
    class InventoryDialog : Dialog
    {
        List<Item> _items;
        int _imageSize = 32;
        int _borderSize = 5;
        RectangleF _bounds;
        public InventoryDialog(List<Item> items)
        {
            _items = items;
            int numItemsX = 8;
            int numItemsY = 6;

            float width = (_imageSize + _borderSize) * numItemsX;
            float height = (_imageSize + _borderSize) * numItemsY;
            _bounds = new RectangleF(400 - width/2, 350, width, height);
        }

        public override void Render(XnaRenderer renderer)
        {
            renderer.Draw(Engine.GetTexture("house.solidwall"), _bounds, 0.5f, 0, new Vector2F(0f, 0f));

            float x = _borderSize;
            float y = _borderSize;
            foreach (Item item in _items)
            {
                renderer.Draw(item.Texture, new RectangleF(_bounds.X + x, _bounds.Y + y, _imageSize, _imageSize), 0.4f, 0, new Vector2F(0f, 0f));
                x += _imageSize + _borderSize;
                if (x > _bounds.Width - _imageSize - _borderSize)
                {
                    x = _borderSize;
                    y += _imageSize + _borderSize;
                }
            }
        }

        public override void ProcessKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.Escape:
                case InputKey.Space:
                    Close();
                    break;
            }
        }
    }
}
