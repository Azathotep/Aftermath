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

        const int ZombieDefaultSightDistance = 8;
        const int ZombieEnragedSightDistance = 16;

        Human GetNearestVisibleHuman()
        {
            var targets = VisibleHumans();
            if (targets.Count > 0)
                return targets[0];
            return null;
        }

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
                int sightRange = ZombieDefaultSightDistance;
                //idle zombies have lower sight range
                if (_state == ZombieState.Enraged)
                    sightRange = ZombieEnragedSightDistance;
                if (Location.GetChebyshevDistanceFrom(human.Location) > sightRange)
                    continue;
                ret.Add(human);
            }
            return ret;
        }

        enum ZombieState
        {
            Idle,
            Stare,
            Enraged
        }

        byte _rageLevel = 0;

        ZombieState _state = ZombieState.Idle;

        Human _target;
        Tile _targetTile;

        void BeginStare()
        {
            _state = ZombieState.Stare;
            _rageLevel = 4;
        }

        void BeginIdle()
        {
            _state = ZombieState.Idle;
        }

        void BeginEnraged()
        {
            _state = ZombieState.Enraged;
            _rageLevel = 50;
        }

        bool _skipNextTurn = false;
        public override void DoTurn()
        {
            Human target;
            switch (_state)
            {
                //when zombie is idle it slowly shuffles around
                //if a player gets to close it rages
                case ZombieState.Idle:

                    target = GetNearestVisibleHuman();
                    if (target != null)
                    {
                        _targetTile = target.Location;
                        BeginStare();
                        break;
                    }

                    //check sound

                    //check smell
                    Tile scentTarget = GetNextTileInScentTrail();
                    if (scentTarget != null)
                    {
                        if (Dice.Next(2) == 0)
                            MoveTowards(scentTarget);
                        return;
                    }

                    if (_targetTile != null)
                    {
                        MoveTowards(_targetTile);
                        return;
                    }

                    if (Dice.Next(3) == 0)
                        Move(Compass.GetRandomCompassDirection());
                    break;
                case ZombieState.Stare:
                    //zombie has spotted a human but it takes a while to realize
                    target = GetNearestVisibleHuman();
                    if (target == null)
                    {
                        _rageLevel--;
                        if (_rageLevel == 0)
                        {
                            BeginIdle();
                            return;
                        }
                    }
                    else
                    {
                        if (Location.GetChebyshevDistanceFrom(target.Location) < 4)
                            _rageLevel += 10;
                        else
                            _rageLevel += 2;
                        if (_rageLevel > 10)
                        {
                            _targetTile = target.Location;
                            BeginEnraged();
                            return;
                        }
                    }
                    break;
                case ZombieState.Enraged:
                    target = GetNearestVisibleHuman();
                    if (target != null)
                    {
                        _rageLevel = 50;
                        _targetTile = target.Location;
                    }
                    else
                    {
                        _rageLevel--;
                        if (_rageLevel == 0)
                        {
                            BeginIdle();
                            return;
                        }
                    }

                    if (_targetTile != null)
                    {
                        //if already at the last seen tile and can't see target then target has been lost
                        if (Location == _targetTile)
                        {
                            _targetTile = null;
                        }
                        else
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
                            return;
                        }
                    }

                    Tile scent = GetNextTileInScentTrail();
                    if (scent != null)
                        MoveTowards(scent);
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

        public bool IsAlerted 
        {
            get
            {
                return _state == ZombieState.Stare;
            }
        }
    }
}
