using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.AI;
using Aftermath.Core;
using Aftermath.Map;
using Aftermath.Utils;
using Aftermath.Lighting;

namespace Aftermath.Creatures
{
    class Zombie : Creature
    {
        static int playermap_generatedTime = -1;
        static HomingField playermap=null;

        public Zombie()
        {
            _health = 20;
        }

        const int ZombieIdleSightDistance = 8;
        const int ZombieEnragedSightDistance = 16;

        List<Human> VisibleHumans()
        {
            List<Human> ret = new List<Human>();
            List<Human> nearbyHumans = new List<Human>();
            nearbyHumans.Add(Engine.Instance.Player);
            foreach (var human in nearbyHumans)
            {
                HashSet<Tile> tiles = Engine.Instance.Player.ZombieViewField;
                if (!tiles.Contains(Location))
                    continue;
                int sightRange = ZombieEnragedSightDistance;
                //idle zombies have lower sight range
                if (_state == ZombieState.Idle)
                    sightRange = ZombieIdleSightDistance;
                if (Location.GetChebyshevDistanceFrom(human.Location) > sightRange)
                    continue;
                ret.Add(human);
            }
            return ret;
        }

        enum ZombieState
        {
            Idle,
            Enraged
        }

        byte _rageLevel = 0;

        ZombieState _state = ZombieState.Idle;

        Human _target;
        Tile _targetTile;

        bool _skipNextTurn = false;
        public override void DoTurn()
        {
            switch (_state)
            {
                //when zombie is idle it slowly shuffles around
                //if a player gets to close it rages
                case ZombieState.Idle:
                    //look for visible targets
                    List<Human> targets = VisibleHumans();
                    //if there are no targets then move randomly
                    if (targets.Count == 0)
                    {
                        _rageLevel = 0;
                        //shamble about randomly
                        if (Dice.Next(3) == 0)
                            Move(Compass.GetRandomCompassDirection());
                        return;
                    }
                    //increment rage count until enraged. Probably want to rage faster when really close
                    _rageLevel++;
                    if (_rageLevel > 3)
                    {
                        _rageLevel = 20;
                        //TODO change to pick nearest target?
                        Human target = targets[0];
                        //become enraged and run at the target
                        _state = ZombieState.Enraged;
                        _target = target;
                        _targetTile = target.Location;
                    }
                    break;
                case ZombieState.Enraged:
                    List<Human> visibleHumans = VisibleHumans();
                    //maximum rage while humans are visible else range falls
                    if (visibleHumans.Count > 0)
                        _rageLevel = 20;
                    else
                        _rageLevel--;

                    //chase target
                    if (_target != null)
                    {
                        if (visibleHumans.Contains(_target))
                            _targetTile = _target.Location;
                        else
                        {
                            //if already at the last seen tile and can't see target then target has been lost
                            if (Location == _targetTile)
                            {
                                _target = null;
                                _targetTile = null;
                            }
                        }
                    }

                    if (_target == null && visibleHumans.Count > 0)
                    {
                        _target = visibleHumans[0];
                        _targetTile = _target.Location;
                    }

                    //if no target then just stand there angry until a new target is aquired
                    //move towards target
                    if (_targetTile != null)
                    {
                        Tile tile = GetNextTileTowards(_targetTile);
                        if (tile == null)
                            return;
                        if (tile.Creature != null && IsFood(tile.Creature))
                            Bite(tile);
                        else
                        {
                            Door door = tile.Material as Door;
                            if (door != null && !door.IsOpen)
                            {
                                door.IsOpen = true;
                                
                            }
                            else
                                MoveTo(tile);
                        }
                    }
                    
                    //if rage level drops then switch back to idle state
                    if (_rageLevel == 0)
                    {
                        _target = null;
                        _targetTile = null;
                        _state = ZombieState.Idle;
                    }
                    break;
            }
            return;

            if (Engine.Instance.TurnSystem.TurnNumber != playermap_generatedTime)
            {
                playermap = new HomingField(Engine.Instance.World, 30, 30); //50, 50);
                playermap.CenterOn(Engine.Instance.Player.Location);
                playermap.SetHomingTarget(Engine.Instance.Player.Location);
                playermap.Generate();
                playermap_generatedTime = Engine.Instance.TurnSystem.TurnNumber;
            }

            //skip every other turn, makes zombies slow
            if (_skipNextTurn)
            {
                _skipNextTurn = false;
                return;
            }

            Tile next = playermap.GetNext(Location);
            if (next == null)
            {
                //if there is no path to follow then move randomly
                Move(Compass.GetRandomCompassDirection());
            }
            else
            {
                //there is a path to follow and the tile is unblocked move there
                if (next.Creature == null)
                {
                    MoveTo(next);
                }
                else
                {
                    //a creature blocks the path. If it's food then bite it, otherwise move randomly
                    if (IsFood(next.Creature))
                        Bite(next);
                    else
                        Move(Compass.GetRandomCompassDirection());
                }                 
            }
                
            //zombies somehow know where the player is and chase
            //MoveTowards(Aftermath.Core.Engine.Instance.Player.Tile);
            //_skipNextTurn = true;
            //Move(Compass.GetRandomCompassDirection());
        }

        //hello  //hello BACK!!!

        bool IsFood(Creature other)
        {
            if (other == Engine.Instance.Player)
                return true;
            return false;
        }

        /// <summary>
        /// Bite whatever is in the tile
        /// </summary>
        void Bite(Tile tile)
        {
            if (tile.Creature == null)
                return;
            tile.Creature.PutDamage(10);
            EndTurn();
        }

        public override GameTexture Texture
        {
            get
            {
                if (IsAlive)
                {
                    if (_health <= 10)
                        return new GameTexture("zombieInjured", new RectangleI(0, 0, 64, 64));
                    return new GameTexture("zombie", new RectangleI(0, 0, 64, 64));
                }
                else
                    return new GameTexture("zombie_dead", new RectangleI(0, 0, 64, 64));
            }
        }
    }
}
