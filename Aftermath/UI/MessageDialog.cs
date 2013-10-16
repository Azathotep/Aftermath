using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Input;
using Aftermath.State;
using Aftermath.Core;
using Aftermath.Utils;

namespace Aftermath.UI
{
    class MessageDialog : Dialog
    {
        public override void Render(XnaRenderer renderer)
        {
            renderer.Draw(Engine.Instance.TextureManager.GetTexture("house.solidwall"), Position, 0.8f, 0, new Vector2F(0, 0));
            renderer.DrawStringBox(Text, Position, Microsoft.Xna.Framework.Color.White);
        }

        public RectangleF Position
        {
            get;
            set;
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

        internal void SetText(string text, bool takesFocus)
        {
            _text = text;
            _focus = takesFocus;
        }

        public event Action OnOk;
    }
}
