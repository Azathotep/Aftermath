using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Input;
using Aftermath.Core;
using Aftermath.UI;

namespace Aftermath.State
{
    class GamePausedState : GameState
    {
        protected override void OnSwitchTo()
        {
            Engine.Instance.UIManager.ShowMenu("PauseMenu");
        }

        protected override void OnSwitchAway()
        {
            Engine.Instance.UIManager.HideMenu("PauseMenu");
        }

        public override void ProcessKey(InputKey key)
        {
            Menu menu = Engine.Instance.UIManager.GetMenuByName("PauseMenu");
            menu.ProcessKey(key);
        }
    }
}
