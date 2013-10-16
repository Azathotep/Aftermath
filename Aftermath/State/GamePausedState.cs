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
        //protected override void OnSwitchTo()
        //{
        //    PauseMenu
        //    Engine.Instance.UIManager.ShowDialog("PauseMenu");
        //}

        //protected override void OnSwitchAway()
        //{
        //    Engine.Instance.UIManager.HideDialog("PauseMenu");
        //}

        public override void ProcessKey(InputKey key)
        {
            throw new NotImplementedException();
        }
    }
}
