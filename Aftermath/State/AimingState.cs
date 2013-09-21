using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Input;
using Aftermath.Animations;

namespace Aftermath.State
{
    /// <summary>
    /// Implements the state where the player aims using the crosshair
    /// </summary>
    class AimingState : GameState
    {
        public override void ProcessKey(InputKey key)
        {
            TargetingModule targetingModule = Engine._targetingModule;
            switch (key)
            {
                case InputKey.Left:
                    targetingModule.MoveBy(-1, 0);
                    break;
                case InputKey.Right:
                    targetingModule.MoveBy(1, 0);
                    break;
                case InputKey.Up:
                    targetingModule.MoveBy(0, -1);
                    break;
                case InputKey.Down:
                    targetingModule.MoveBy(0, 1);
                    break;
                case InputKey.Escape:
                    targetingModule.ClearTarget();
                    GameState.CurrentState = GameState.MovementState;
                    break;
                case InputKey.F:
                    //fire at target
                    if (targetingModule.Tile == Engine.Player.Tile)
                        break;
                    Engine.Player.FireAt(targetingModule.Tile);
                    
                    //GameState.CurrentState = GameState.MovementState;
                    break;
                case InputKey.R:
                    //fire at target
                    Engine.Player.Reload();
                    break;
            }
        }
    }
}
