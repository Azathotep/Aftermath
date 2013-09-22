using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Input;

namespace Aftermath.State
{
    /// <summary>
    /// Implements the state where the player can move around
    /// </summary>
    class MovementState : GameState
    {
        public override void ProcessKey(InputKey key)
        {
            //TODO if the player holds down two keys then the first key ends the turn and currentactor becomes null
            //the second key handler then fires but currentactor is now null.
            //for now put a check that the currentactor isn't null but review this later
            if (Engine.TurnSystem.CurrentActor == null || !Engine.TurnSystem.CurrentActor.IsPlayerControlled)
                return;

            switch (key)
            {
                case InputKey.Left:
                    Engine.TurnSystem.CurrentActor.Move(CompassDirection.West);
                    break;
                case InputKey.Right:
                    Engine.TurnSystem.CurrentActor.Move(CompassDirection.East);
                    break;
                case InputKey.Up:
                    Engine.TurnSystem.CurrentActor.Move(CompassDirection.North);
                    break;
                case InputKey.Down:
                    Engine.TurnSystem.CurrentActor.Move(CompassDirection.South);
                    break;
                case InputKey.OemPeriod:
                    Engine.TurnSystem.CurrentActor.EndTurn();
                    break;
                case InputKey.Escape:
                    GameState.CurrentState = GameState.GamePausedState;
                    //Engine.Exit();
                    break;
                case InputKey.F:
                    //activate aim mode
                    GameState.CurrentState = GameState.AimingState;
                    Engine._targetingModule.ReaquireTarget(Engine.Player);
                    break;
                case InputKey.R:
                    Engine.Player.Reload();
                    break;
            }
        }
    }
}
