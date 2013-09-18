﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Map;
using Aftermath.Creatures;

namespace Aftermath.Input
{
    /// <summary>
    /// Helper targetting AI that locks onto hostile targets and uses some intelligence to decide
    /// whether to keep locked on or to aquire a new target. Used to simplify the player's aiming (so that the
    /// gun locks onto zombies)
    /// </summary>
    class TargetingModule
    {
        //module has a concept of targetting a creature or targetting a tile
        //when targetting a creature the module will continue to track that creature while it is in sight,
        //even if it moves. When targetting a tile the module will stick on the tile.
        Creature _targetedCreature;
        Tile _targetedTile;

        /// <summary>
        /// Tile currently being targetted
        /// </summary>
        public Tile Tile
        {
            get
            {
                if (_targetedCreature != null)
                    return _targetedCreature.Tile;
                return _targetedTile;
            }
        }

        /// <summary>
        /// Manually retarget a different tile
        /// </summary>
        public void MoveBy(int x, int y)
        {
            Tile newTarget = Tile.GetRelativeTile(new Vector2I(x, y));
            if (newTarget != null)
            {
                _targetedTile = newTarget;
                _targetedCreature = _targetedTile.Creature;
            }
        }

        /// <summary>
        /// Reaquires the existing target or finds a new target
        /// </summary>
        /// <param name="targeter">the creature doing the targetting</param>
        public void ReaquireTarget(Creature targeter)
        {
            HashSet<Creature> visibleCreatures = targeter.GetVisibleCreatures();
            //if a currently targetted creature is still visible (and alive) then keep that as the target
            if (_targetedCreature != null && _targetedCreature.IsAlive && visibleCreatures.Contains(_targetedCreature))
                return;
            
            HashSet<Tile> visibleTiles = targeter.GetVisibleTiles();
            //if targeting a tile and that tile is still visible then continue targetting that tile
            if (_targetedCreature == null && visibleTiles.Contains(_targetedTile))
                return;

            //module has no target or an invalid target so aquire a new target
            //go for the nearest living creature
            Creature nearestTarget = (from c in visibleCreatures where c.IsAlive orderby c.Tile.GetManhattenDistanceFrom(targeter.Tile) select c).FirstOrDefault();
            if (nearestTarget == null)
            {
                //no available target so reset crosshair to owner's tile
                _targetedTile = targeter.Tile;
                _targetedCreature = null;
            }
            else
            {
                _targetedCreature = nearestTarget;
            }
        }

        /// <summary>
        /// Resets targetting
        /// </summary>
        internal void ClearTarget()
        {
            _targetedCreature = null;
            _targetedTile = null;
        }

        /// <summary>
        /// Keys for controlling target module. TODO move this.
        /// </summary>
        internal void ProcessKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.Left:
                    MoveBy(-1, 0);
                    break;
                case InputKey.Right:
                    MoveBy(1, 0);
                    break;
                case InputKey.Up:
                    MoveBy(0, -1);
                    break;
                case InputKey.Down:
                    MoveBy(0, 1);
                    break;
                case InputKey.Escape:
                    ClearTarget();
                    break;
            }
        }
    }
}
