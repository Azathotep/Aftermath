using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Input;
using Aftermath.Utils;
using Aftermath.Lighting;
using Aftermath.UI;
using Microsoft.Xna.Framework;

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
                    Color color = new Color(1f, 0f, 0f);
                    if (Dice.Next(3) == 0)
                        color = new Color(0f, 1f, 0f);
                    else if (Dice.Next(3) == 0)
                        color = new Color(0f, 0f, 1f);

                    Engine.SaveAndRestore();

                    //PointLight l = new PointLight(Core.Engine.Instance.Player.Location, 6, color);
                    //Core.Engine.Instance.World.Lights.Add(l);
                    //l.RecalculateLightfield();
                    //Engine.TurnSystem.CurrentActor.EndTurn();
                    break;
                case InputKey.I:
                    Engine.Player.Interact();
                    break;
                case InputKey.Escape:
                    PauseMenu menu = new PauseMenu();
                    menu.Show();
                    break;
                case InputKey.F:
                    //activate aim mode
                    GameState.CurrentState = GameState.AimingState;
                    Engine._targetingModule.ReaquireTarget(Engine.Player);
                    break;
                case InputKey.L:
                    Engine.Player.ToggleFlashlight();
                    break;
                case InputKey.R:
                    Engine.Player.Reload();
                    break;
                case InputKey.OemComma:
                    Engine.Player.PickupItem();
                    break;
                case InputKey.Space:
                    InventoryDialog inventoryDialog = new InventoryDialog(Engine.Player.Inventory);
                    inventoryDialog.Show();
                    break;
            }
        }
    }
}
