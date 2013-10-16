using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Core;
using Aftermath.Input;

namespace Aftermath.UI
{
    abstract class Dialog
    {
        public abstract void Render(XnaRenderer renderer);

        public bool IsVisible;

        public virtual void ProcessKey(InputKey key)
        {
        }

        public virtual bool KeyboardFocus
        {
            get
            {
                return true;
            }
        }

        public virtual void Show()
        {
            Engine.Instance.UIManager.Show(this);
        }

        public void Close()
        {
            Engine.Instance.UIManager.Hide(this);
        }
    }
}
