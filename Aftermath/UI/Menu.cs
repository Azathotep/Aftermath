using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Input;

namespace Aftermath.UI
{
    abstract class Menu
    {
        public abstract void Render(XnaRenderer renderer);

        public bool IsVisible;

        public virtual void ProcessKey(InputKey key)
        {
        }
    }
}
