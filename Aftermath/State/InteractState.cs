using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Input;
using Aftermath.Animations;
using Aftermath.Utils;
using Aftermath.Map;
using Aftermath.Lighting;

namespace Aftermath.State
{
    /// <summary>
    /// Implements the state where the player interacts with adjacent objects
    /// </summary>
    class InteractState : GameState
    {
        public override void ProcessKey(InputKey key)
        {
            //if the user presses a cursor key then interact with any object in that direction
            CompassDirection? direction = Compass.DirectionFromKey(key);
            if (direction.HasValue)
            {
                Tile target = Engine.Player.Location.GetNeighbour(direction.Value);
                //depending what is on the target perform some action
                //if the target is a door then the interaction is to open or close it
                Door door = target.Material as Door;
                if (door != null)
                {
                    if (door.IsOpen)
                        door.Close();
                    else
                        door.Open();

                    //if a door opens and closes need to update nearby light fields
                    //TODO redesign this, no need to update every light on the map
                    foreach (PointLight light in Core.Engine.Instance.World.Lights)
                        light.RecalculateLightfield();
                    Engine.Player.EndTurn();
                }
                GameState.CurrentState = GameState.MovementState;
                return;
            }

            switch (key)
            {
                case InputKey.Escape:
                    GameState.CurrentState = GameState.MovementState;
                    break;
            }
        }
    }
}
