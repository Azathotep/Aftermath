﻿using System;
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
        const int ZombieDefaultSightDistance = 8;
        const int ZombieEnragedSightDistance = 16;

        byte _rageLevel = 0;
        ZombieState _state = ZombieState.Idle;
        Tile _targetTile;

        public Zombie()
        {
            _health = 20;
        }
        
        /// <summary>
        /// Returns the nearest visible human
        /// </summary>
        Human GetNearestVisibleHuman()
        {
            var targets = GetVisibleHumans();
            if (targets.Count > 0)
                return targets[0];
            return null;
        }

        /// <summary>
        /// Returns a list of all visible humans
        /// </summary>
        List<Human> GetVisibleHumans()
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

        void BecomeAlert()
        {
            _state = ZombieState.Alert;
            _rageLevel = 4;
        }

        void BecomeIdle()
        {
            _state = ZombieState.Idle;
        }

        public void BecomeEnraged()
        {
            _state = ZombieState.Enraged;
            _rageLevel = 50;
        }

        /// <summary>
        /// Called when the zombie bumps into another creature while trying to move
        /// </summary>
        /// <param name="creature">the creature bumped into</param>
        /// <returns>true if the bump was handled, false otherwise</returns>
        protected override bool OnBump(Creature creature)
        {
            if (IsFood(creature))
            {
                Bite(creature.Location);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called when the zombie bumps into a solid tile while moving
        /// </summary>
        /// <param name="tile">the tile bumped into</param>
        /// <returns>true if this event is handled</returns>
        protected override bool OnBump(Tile tile)
        {
            if (tile.Structure != null)
            {
                tile.Structure.Damage(2);
            }
            return false;
        }

        Tile _lastTile=null;

        /// <summary>
        /// AI logic when the zombie is enraged
        /// </summary>
        void OnEnraged()
        {
            Human target = GetNearestVisibleHuman();
            //if a human is visible then recharge rage
            if (target != null)
            {
                _rageLevel = 50;
                _targetTile = target.Location;
                _lastTile = null;
            }
            else
            {
                //reduce rage if target is not visible
                _rageLevel--;
                if (_rageLevel == 0)
                {
                    _targetTile = null;
                    BecomeIdle();
                    return;
                }
            }

            //if zombie has a target then move towards it
            if (_targetTile != null)
            {
                //if already at the target tile then drop target
                if (Location == _targetTile)
                    _targetTile = null;
                else
                {
                    //when zombies get blocked they typically step back and forth between two tiles
                    //detect this and if it happens clear the target tile so a new target is aquired
                    Tile location = Location;

                    //move towards target tile
                    MoveTowards(_targetTile);
                    if (Location == _lastTile)
                        _targetTile = null;
                    _lastTile = location;
                    return;
                }
            }

            if (_heardSound != null && Engine.Instance.TurnSystem.TurnNumber - _heardSound.TurnEmitted > 30)
                _heardSound = null;

            //move towards heard sound
            if (_heardSound != null)
            {
                Tile next = _heardSound.GetNextTileTowardsSource(Location);
                if (next != null)
                {
                    MoveTowards(next);
                    return;
                }
            }

            Tile scent = GetNextTileInScentTrail();
            if (scent != null)
                MoveTo(scent);

            //no target, head in a random direction
            _targetTile = Location.GetRelativeTile(new Vector2I(Dice.Next(16)-8, Dice.Next(16)-8));
            _lastTile = null;
        }

        /// <summary>
        /// AI logic when the zombie is in idle state
        /// </summary>
        void OnIdle()
        {
            //keep a look out for humans
            Human target = GetNearestVisibleHuman();
            if (target != null)
            {
                //something spotted, go into alert state
                _targetTile = target.Location;
                BecomeAlert();
                return;
            }

            //no humans visible

            if (_heardSound != null && Engine.Instance.TurnSystem.TurnNumber - _heardSound.TurnEmitted > 30)
                _heardSound = null;

            //move towards last heard sound
            if (_heardSound != null)
            {
                Tile next = _heardSound.GetNextTileTowardsSource(Location);
                if (next != null)
                {
                    MoveTowards(next);
                    return;
                }
            }

            //check smell
            Tile scentTarget = GetNextTileInScentTrail();
            if (scentTarget != null)
            {
                //slowly follow scent trail
                if (Dice.Next(2) == 0)
                    MoveTo(scentTarget);
                return;
            }

            //if have a previously remembered target tile then move towards that
            if (_targetTile != null)
            {
                MoveTowards(_targetTile);
                return;
            }

            //wander randomly
            if (Dice.Next(3) == 0)
                Move(Compass.GetRandomCompassDirection());
        }

        /// <summary>
        /// AI logic when zombie is in alert state
        /// </summary>
        void OnAlert()
        {
            //zombie has spotted a human but it takes a while to realize
            Human target = GetNearestVisibleHuman();
            if (target == null)
            {
                //if cannot see anyone then rage down and eventually switch to idle
                _rageLevel--;
                if (_rageLevel == 0)
                {
                    BecomeIdle();
                    return;
                }
            }
            else
            {
                //if a target is near enough then enrage instantly
                if (Location.GetChebyshevDistanceFrom(target.Location) < 4)
                    _rageLevel += 10;
                else
                    _rageLevel += 2;
                if (_rageLevel > 10)
                {
                    _targetTile = target.Location;
                    BecomeEnraged();
                    return;
                }
            }
        }

        public override void Hear(Sound sound)
        {
            _heardSound = sound;
            BecomeEnraged();
        }

        /// <summary>
        /// Called once per zombie turn for the zombie to perform a turn action
        /// </summary>
        public override void DoTurn()
        {
            //action depends on state
            switch (_state)
            {
                //when zombie is idle it slowly shuffles around
                //if a player gets to close it rages
                case ZombieState.Idle:
                    OnIdle();
                    break;
                case ZombieState.Alert:
                    OnAlert();
                    break;
                case ZombieState.Enraged:
                    OnEnraged();
                    break;
            }
        }

        /// <summary>
        /// Returns true if the specified creature is food, ie attackable
        /// </summary>
        /// <param name="other">creature to test</param>
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

        /// <summary>
        /// Returns the texture to draw this zombie
        /// </summary>
        public override GameTexture Texture
        {
            get
            {
                if (IsAlive)
                {
                    //TODO get textures from manager and don't heap allocate every frame
                    if (_health <= 10)
                        return new GameTexture("zombieInjured", new RectangleI(0, 0, 64, 64));
                    return new GameTexture("zombie", new RectangleI(0, 0, 64, 64));
                }
                else
                    return new GameTexture("zombie_dead", new RectangleI(0, 0, 64, 64));
            }
        }

        public ZombieState State
        {
            get
            {
                return _state;
            }
        }

        public override CreatureType Type
        {
            get { return CreatureType.Zombie; }
        }

        public override void Serialize(System.IO.BinaryWriter bw)
        {
            base.Serialize(bw);
            bw.Write((byte)_rageLevel);
            bw.Write((byte)_state);
            bool hasTarget = _targetTile != null;
            bw.Write((bool)hasTarget);
            if (hasTarget)
            {
                bw.Write((ushort)_targetTile.X);
                bw.Write((ushort)_targetTile.Y);
            }
        }

        public override void Deserialize(System.IO.BinaryReader br, World world)
        {
            base.Deserialize(br, world);
            _rageLevel = br.ReadByte();
            _state = (ZombieState)br.ReadByte();
            bool hasTarget = br.ReadBoolean();
            if (hasTarget)
            {
                ushort x = br.ReadUInt16();
                ushort y = br.ReadUInt16();
                _targetTile = world.GetTile(x, y);
            }
        }
    }

    enum ZombieState : byte
    {
        Idle,
        Alert,
        Enraged
    }
}
