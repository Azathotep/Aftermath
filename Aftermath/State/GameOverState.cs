using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Input;

namespace Aftermath.State
{
    /// <summary>
    /// Implements the state when the game is over
    /// </summary>
    class GameOverState : GameState
    {
        public override void ProcessKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.Escape:
                    Engine.Exit();
                    break;
            }
        }
    }
}
