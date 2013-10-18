using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Input;
using Aftermath.State;
using Aftermath.Core;
using Aftermath.Utils;
using Microsoft.Xna.Framework;

namespace Aftermath.UI
{
    class MessageDialog : Dialog
    {
        float x = 0;
        RectangleF _bounds;
        public MessageDialog(RectangleF bounds)
        {
            _bounds = bounds;
        }

        public override void Show()
        {
            x = -_bounds.Width;
            base.Show();
        }

        public override void Render(XnaRenderer renderer)
        {
            x += 5f;
            if (x > _bounds.X)
                x = _bounds.X;
            renderer.Draw(Engine.Instance.TextureManager.GetTexture("overlay.white"), new RectangleF(x, _bounds.Y, _bounds.Width, _bounds.Height), 0.8f, 0, new Vector2F(0, 0), new Color(0.1f,0.1f,0.1f,0.2f));
            renderer.DrawStringBox(Text, new RectangleF(x + 5, _bounds.Y + 5, _bounds.Width - 10, _bounds.Height - 10), Microsoft.Xna.Framework.Color.White);
        }

        public override bool KeyboardFocus
        {
            get
            {
                return _focus;
            }
        }

        public override void ProcessKey(InputKey key)
        {
            if (key == InputKey.Enter)
            {
                _focus = false;
                if (OnOk != null)
                    OnOk();
            }
        }

        bool _focus = false;
        string _text;
        public string Text 
        {
            get
            {
                return _text;
            }
        }

        internal void SetText(string text, bool takesFocus=false)
        {
            _text = text;
            _focus = takesFocus;
        }

        public event Action OnOk;
    }
}
