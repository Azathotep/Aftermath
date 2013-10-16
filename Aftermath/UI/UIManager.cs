using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Input;

namespace Aftermath.UI
{
    class UIManager
    {
        public UIManager()
        {
        }

        List<Dialog> _shownDialogs = new List<Dialog>();

        public void Show(Dialog dialog)
        {
            _shownDialogs.Add(dialog);
        }

        internal void RenderUI(XnaRenderer _renderer)
        {
            foreach (Dialog dialog in _shownDialogs)
                dialog.Render(_renderer);
        }

        internal bool ProcessKey(InputKey key)
        {
            if (_shownDialogs.Count == 0)
                return false;
            foreach (Dialog dialog in _shownDialogs.Reverse<Dialog>())
            {
                if (dialog.KeyboardFocus)
                {
                    dialog.ProcessKey(key);
                    return true;
                }
            }
            return false;
        }

        internal void Hide(Dialog dialog)
        {
            _shownDialogs.Remove(dialog);
        }
    }
}
