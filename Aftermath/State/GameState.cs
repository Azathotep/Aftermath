using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Input;
using Aftermath.Core;

namespace Aftermath.State
{
    /// <summary>
    /// The game uses a state machine for a more modular design. The game is divided into states.
    /// Only one state is active at any one time. Keyboard input is routed to the currently active state which 
    /// is responsible for handling the command in the context of that state. Individual states are implemented 
    /// by extending this abstract base class. 
    /// </summary>
    abstract class GameState
    {
        public static GameState MovementState = new MovementState();
        public static GameState AimingState = new AimingState();

        /// <summary>
        /// Returns or sets the current game state. Should be set to one of the static instances above.
        /// </summary>
        public static GameState CurrentState = MovementState;

        /// <summary>
        /// Tells the game state to handle a keypress
        /// </summary>
        public abstract void ProcessKey(InputKey key);

        protected Engine Engine
        {
            get
            {
                return Engine.Instance;
            }
        }

    }
}
