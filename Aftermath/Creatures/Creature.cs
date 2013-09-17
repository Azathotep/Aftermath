﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Map;

namespace Aftermath.Creatures
{
    /// <summary>
    /// Base class for all creatures in the game. Provides common methods used across all creatures. Specific
    /// creatures should be implemented by extending this class and overriding methods.
    /// </summary>
    public abstract class Creature
    {
        Tile _tile;
        bool _facingRight=false;

        public ActionResult Move(CompassDirection moveDirection)
        {
            Tile targetTile = _tile.GetNeighbour(moveDirection);
            return MoveTo(targetTile);
        }

        /// <summary>
        /// Returns whether the creature is facing left or right, depending on their last movement. This has no meaning
        /// for gameplay but is used for visuals as it is trivial to simply flip the spites horizontally.
        /// </summary>
        public bool FacingRight
        {
            get
            {
                return _facingRight;
            }
        }

        /// <summary>
        /// Moves the creature from its current tile to a new tile. The operation can fail if
        /// the tile is blocked or already occupied.
        /// </summary>
        /// <returns>Indicates whether the action succeeded and if not, why not</returns>
        public virtual ActionResult MoveTo(Tile targetTile)
        {
            if (targetTile == null)
                return ActionResult.TileBlocked;
            //update the facing direction
            if (_tile != null && targetTile.X < _tile.X)
                _facingRight = false;
            else
                _facingRight = true;

            //check whether the tile allows the creature to enter.
            ActionResult res = targetTile.CanEnter(this);
            if (res == ActionResult.TileOccupied)
            {
                //TODO tile is occupied by another creature, so convert movement into a bumb melee attack
                //res = CanAttack(targetTile);
                //if (res == ActionResult.Ok)
                //    MeleeAttack(targetTile);
                return res;
            }
            //if tile itself blocks (eg it's solid)
            if (res == ActionResult.TileBlocked)
            {
                //bump into the target tile. Tiles can have effects when bumped into (eg pulling a lever or
                //putting out a light, opening a door) which may use up the turn.
                //if (targetTile.OnBump(this))
                //{
                //    res = ActionResult.Ok;
                //    EndTurn();
                //}
                //else
                {
                    //TODO if the player is moving into a wall put out a message? eg "the wall is unmovable"
                }
                return res;
            }

            //Tile.OnExit(this);
            //do the move and update references
            InternalMove(targetTile);
            //this action is a move so mark the current turn as ended
            EndTurn();
            //Tile.OnEnter(this);
            return ActionResult.Ok;
        }

        /// <summary>
        /// Should be called by any action that ends the turn so that the player's turn is ended
        /// </summary>
        private void EndTurn()
        {
            Engine.Instance.TurnSystem.TurnComplete();
        }

        /// <summary>
        /// Moves the creature to the specified tile. Caller must ensure the movement is legal.
        /// </summary>
        protected void InternalMove(Tile targetTile)
        {
            if (_tile != null)
                _tile.Creature = null;
            if (targetTile.Creature != null)
                throw new Exception("InternalMove attempt to tile already occupied by another creature");
            targetTile.Creature = this;
            _tile = targetTile;
        }

        /// <summary>
        /// Returns whether the creature is player controlled
        /// </summary>
        public virtual bool IsPlayerControlled
        {
            get
            {
                return false;
            }
        }

        public virtual string Description
        {
            get
            {
                return "";
            }
        }

        public Tile Tile
        {
            get
            {
                return _tile;
            }
        }

        public int X
        {
            get
            {
                return _tile.X;
            }
        }

        public int Y
        {
            get
            {
                return _tile.Y;
            }
        }

        protected int _sightRadius = 15;

        /// <summary>
        /// Returns the set of tiles currently visible to the creature
        /// </summary>
        public HashSet<Tile> GetVisibleTiles()
        {
            return Engine.Instance.GetFov(_tile.X, _tile.Y, _sightRadius);
        }

        /// <summary>
        /// Returns the set of other creatures currently visible to the creature
        /// </summary>
        public HashSet<Creature> GetVisibleCreatures()
        {
            HashSet<Creature> ret = new HashSet<Creature>();
            foreach (Tile tile in GetVisibleTiles())
                if (tile.Creature != null && tile.Creature != this)
                    ret.Add(tile.Creature);
            return ret;
        }

        //TODO a draw offset would allow creatures to be drawn offcenter in the tile?
        //public Vector2 DrawOffset;
        //protected Vector2 DrawPosition
        //{
        //    get
        //    {
        //        return new Vector2(_tile.X + DrawOffset.X, _tile.Y + DrawOffset.Y);
        //    }
        //}

        /// <summary>
        /// Creature implementations should override this method to provide AI turn logic
        /// </summary>
        public virtual void DoTurn()
        {
            
        }

        /// <summary>
        /// Places the creature on the specified tile
        /// </summary>
        /// <param name="targetTile"></param>
        public void Place(Tile targetTile)
        {
            InternalMove(targetTile);
            //targetTile.Map.RegisterCreature(this);
        }

        public abstract Rendering.GameTexture Texture 
        { 
            get;
        }
    }

    public enum ActionResult
    {
        Ok,
        No,
        TileBlocked,
        TileOccupied
    }
}