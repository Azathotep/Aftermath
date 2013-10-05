using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Creatures;
using Aftermath.State;

namespace Aftermath.Core
{
    /// <summary>
    /// Manages turn state and provides methods to advance and query whose turn it currently is
    /// </summary>
    public class TurnSystem
    {
        Queue<Creature> _turns = new Queue<Creature>();

        public void RegisterCreature(Creature creature)
        {
            _turns.Enqueue(creature);
        }

        int _turnNumber;
        /// <summary>
        /// Turn number is advanced by one just prior to the player's turn
        /// </summary>
        public int TurnNumber
        {
            get
            {
                return _turnNumber;
            }
            set
            {
                _turnNumber = value;
            }
        }

        int _minorTurnNumber;
        /// <summary>
        /// The minor turn number is advanced by one on each creature's turn
        /// </summary>
        public int MinorTurnNumber
        {
            get
            {
                return _minorTurnNumber;
            }
        }

        Creature _currentActor;
        /// <summary>
        /// Returns the creature whose turn it current is 
        /// </summary>
        public Creature CurrentActor
        {
            get
            {
                return _currentActor;
            }
        }

        /// <summary>
        /// Ends the turn of the current actor
        /// </summary>
        public void TurnComplete()
        {
            if (_currentActor != null)
            {
                _currentActor.PostTurn();
                _currentActor = null;
            }
        }

        int _playerCountdown = 0;

        List<ITurnInihibitor> _turnInhibitors = new List<ITurnInihibitor>();

        /// <summary>
        /// Advances the game to the next player turn and then exits. Returns immediately if the game is still waiting for
        /// the player to move. This method is called every frame and usually exits immediately waiting on player input.
        /// </summary>
        public void Update()
        {
            if (_currentActor != null && _currentActor.IsPlayerControlled && _playerCountdown > 0)
            {
                _playerCountdown--;
                if (_playerCountdown <= 0)
                    _currentActor.EndTurn();
            }

            while (_turns.Count > 0)
            {
                //wait for current actor to end their turn (almost always the player, as AI controlled actors move immediately)
                if (_currentActor != null)
                    return;

                //wait for any registered inhibitors such as running animations
                foreach (ITurnInihibitor inhibitor in _turnInhibitors)
                    if (inhibitor.IsBlocking)
                        return;

                //TODO move this
                if (Engine.Instance.Player.IsAlive == false)
                {
                    GameState.CurrentState = GameState.GameOverState;
                }

                //increment the minor turn number each time an actor has moved
                _minorTurnNumber++;
                _currentActor = _turns.Dequeue();

                //TODO handle the case where the actor might be "dead" or "paralyzed". In that case
                //should DoTurn() be called? Should PreTurn() and PostTurn() actions be performed?

                //TODO perform any actions just before the actor makes their turn here
                //_currentActor.PreTurn();

                //TODO even dead creatures should have turns?
                //if (!_currentActor.IsActive)
                //{
                //    _currentActor.PostTurn();
                //    _currentActor = null;
                //    continue;
                //}

                //move the actor to the back of the turn list ready for their next turn
                _turns.Enqueue(_currentActor);

                if (_currentActor.IsPlayerControlled)
                {
                    //TODO fix this
                    Engine.Instance._targetingModule.ReaquireTarget(Engine.Instance.Player);


                    //_playerCountdown = 50;
                    //The turn number is incremented just prior to the player's turn
                    TurnNumber++;
                    //TODO raise TurnAdvanced event?
                    //Do nothing else, the player's keyboard input determines when and what the player's turn action is
                }
                else
                {
                    //when the actor is not player controlled they are made to move immediately
                    if (_currentActor.IsAlive)
                    {
                        Creature actor = _currentActor;
                        _currentActor.DoTurn();
                        actor.PostTurn();
                    }
                    _currentActor = null;
                }
            }
        }

        public void RegisterTurnInhibitor(ITurnInihibitor inhibitor)
        {
            _turnInhibitors.Add(inhibitor);
        }
    }

    public interface ITurnInihibitor
    {
        bool IsBlocking
        {
            get;
        }
    }
}
