using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Map;
using Aftermath.Animations;
using Aftermath.Utils;

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

        void Face(Tile targetTile)
        {
            if (_tile != null && targetTile.X < _tile.X)
                _facingRight = false;
            else
                _facingRight = true;
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
            //face the attempted direction of movement
            Face(targetTile);

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

        //TODO try to make this private. Add actions to drive this method instead
        /// <summary>
        /// Should be called by any action that ends the turn so that the player's turn is ended
        /// </summary>
        public void EndTurn()
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

        public Tile Location
        {
            get
            {
                return _tile;
            }
        }

        protected bool _isAlive = true;
        public bool IsAlive
        {
            get
            {
                return _isAlive;
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

        protected int _health = 20;
        /// <summary>
        /// Invoke damage on the creature
        /// </summary>
        /// <param name="damageAmount">amount of damage</param>
        internal void PutDamage(int damageAmount)
        {
            //TODO add damage type (eg bullet, fire, etc)
            //TODO find better method name, InduceDamage or CauseDamage or something..
            Engine.Instance.AnimationManager.StartAnimation(new BleedAnimation(this));

            _health -= damageAmount;
            if (_health <= 0)
                Die();
        }

        void Die()
        {
            _isAlive = false;
            _tile.Creature = null;
            _tile.Corpse = this;
        }

        /// <summary>
        /// Calculates the best path to reach the specified tile and takes the first step
        /// </summary>
        public void MoveTowards(Tile tile)
        {
            Tile[] tiles = _tile.GetTraversablePath(tile);
            if (tiles.Length > 1)
                MoveTo(tiles[1]);
        }

        public class Gun
        {
            internal int MaxRange
            {
                get
                {
                    return 5;
                }
            }

            /// <summary>
            /// The current owner of the gun. This method allows the position of the gun to be obtained.
            /// </summary>
            public Creature Owner
            {
                get;
                set;
            }

            public int LoadedAmmo = 6;

            internal bool CanReach(Tile targetTile)
            {
                return Owner.Location.GetManhattenDistanceFrom(targetTile) <= MaxRange;
            }

            internal bool Fire(Creature firer, Tile targetTile)
            {
                if (!CanReach(targetTile))
                    return false;
                LoadedAmmo--;
                Engine.Instance.AnimationManager.StartAnimation(new MuzzleFlashAnimation(firer));
                if (targetTile.Creature != null)
                    targetTile.Creature.PutDamage(10);
                return true;
            }

            internal void Reload()
            {
                LoadedAmmo = 6;
            }
        }

        Gun _selectedGun = new Gun();

        internal Gun SelectedGun
        {
            get
            {
                _selectedGun.Owner = this;
                return _selectedGun;
            }
        }

        internal void FireAt(Tile targetTile)
        {
            if (_selectedGun.LoadedAmmo <= 0)
                _selectedGun.Reload();
            Face(targetTile);
            bool didFire = _selectedGun.Fire(this, targetTile);
            if (!didFire)
                return;
            EndTurn();
        }

        internal void Reload()
        {
            _selectedGun.Reload();
            EndTurn();
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
