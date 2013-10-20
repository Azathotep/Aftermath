using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Aftermath.Core;
using Aftermath.Map;
using Aftermath.Animations;
using Aftermath.Utils;
using Aftermath.Weapons;
using Aftermath.Items;
using Aftermath.Lighting;

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
        /// Called when the creature bumps into a solid tile when moving. Can be overriden to eg bash the obstacle.
        /// If the response is not handled then it is assumed the tile blocked the crature.
        /// </summary>
        /// <param name="tile">tile bumped into</param>
        /// <returns>true if the creature handled the bump, false if it did not.</returns>
        protected virtual bool OnBump(Tile tile)
        {
            return false;
        }

        /// <summary>
        /// Called when the creature bumps into another creature when moving. Can be overriden to eg bite the creature
        /// or swap places with it.
        /// </summary>
        /// <param name="creature">Creature bumped into</param>
        /// <returns>true if the bump is handled, false if it isn't</returns>
        protected virtual bool OnBump(Creature creature)
        {
            return false;
        }

        /// <summary>
        /// Moves the creature towards the specified tile
        /// </summary>
        /// <param name="target">target tile</param>
        protected ActionResult MoveTowards(Tile target)
        {
            var candidates = (from n in Location.GetNeighbours() orderby n.GetDistanceSquared(target) select n);
            foreach (var tile in candidates)
            {
                ActionResult result = MoveTo(tile);
                if (result == ActionResult.Ok)
                    return ActionResult.Ok;
            }
            return ActionResult.No;
        }

        /// <summary>
        /// Moves the creature from its current tile to a new tile. The operation can fail if
        /// the tile is blocked or already occupied.
        /// </summary>
        /// <returns>Indicates whether the action succeeded and if not, why not</returns>
        protected ActionResult MoveTo(Tile targetTile)
        {
            if (targetTile == null)
                return ActionResult.TileBlocked;
            //face the attempted direction of movement
            Face(targetTile);

            if (targetTile.IsSolid)
            {
                bool handled = OnBump(targetTile);
                if (handled)
                    return ActionResult.Ok;
                return ActionResult.TileBlocked;
            }

            if (targetTile.Creature != null)
            {
                bool handled = OnBump(targetTile.Creature);
                if (handled)
                    return ActionResult.Ok;
                return ActionResult.TileOccupied;
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

        public bool IsAlive
        {
            get
            {
                return _health >= 0;
            }
        }

        protected int _sightRadius = 15;


        /// <summary>
        /// Returns the set of tiles currently visible to the creature
        /// </summary>
        /// <param name="lightThreshold">minimum level of light for tile to be visible</param>
        /// <returns>set of visible tiles</returns>
        public HashSet<Tile> GetVisibleTiles(float lightThreshold = 0)
        {
            HashSet<Tile> ret = _tile.GetVisibleTiles(_sightRadius, lightThreshold);
            //add neighbour tiles whether they are visible or not. Creatures can always see these tiles
            foreach (CompassDirection direction in Compass.CompassDirections)
            {
                Tile neighbour = Location.GetNeighbour(direction);
                if (neighbour != null)
                    ret.Add(neighbour);
            }
            ret.Add(Location);
            return ret;
        }

        /// <summary>
        /// Returns the set of other creatures currently visible to the creature
        /// </summary>
        public HashSet<Creature> GetVisibleCreatures()
        {
            HashSet<Creature> ret = new HashSet<Creature>();
            foreach (Tile tile in GetVisibleTiles(0.4f))
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
        }

        public abstract Rendering.GameTexture Texture 
        { 
            get;
        }

        List<Item> _inventory = null;

        /// <summary>
        /// Returns the inventory of this creature. Items should not be added directly to this collection.
        /// </summary>
        public List<Item> Inventory
        {
            get
            {
                //inventory is only created when the first item is added
                if (_inventory == null)
                    return new List<Item>();
                return _inventory;
            }
        }

        /// <summary>
        /// Pick up item from the floor
        /// </summary>
        internal void PickupItem()
        {
            Item item = Location.Item;
            if (item == null)
                return;
            item.RemoveFromTile();
            AddItemToInventory(item);
            EndTurn();
        }

        /// <summary>
        /// Adds an item to the creature's inventory
        /// </summary>
        /// <param name="item"></param>
        public void AddItemToInventory(Item item)
        {
            //inventory is created lazy as most creatures won't have one
            if (_inventory == null)
                _inventory = new List<Item>();
            _inventory.Add(item);
        }

        protected short _health = 20;
        /// <summary>
        /// Invoke damage on the creature
        /// </summary>
        /// <param name="damageAmount">amount of damage</param>
        internal void PutDamage(short damageAmount)
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
            _tile.Creature = null;
            _tile.Corpse = this;
        }

        /// <summary>
        /// Calculates the best path to reach the specified tile and takes the first step
        /// </summary>
        //public void MoveTowards(Tile tile)
        //{
        //    Tile tile = GetNextTileTowards(tile);
        //    MoveTo(tile);
        //    Tile[] tiles = _tile.GetTraversablePath(tile);
        //    if (tiles.Length > 1)
        //        MoveTo(tiles[1]);
        //}

        public Tile GetNextTileTowards(Tile tile)
        {
            var candidates = (from l in Location.GetNeighbours() select new { Distance = l.GetChebyshevDistanceFrom(tile), Tile = l }).OrderBy((a) => a.Distance);
            foreach (var candidate in candidates)
            {
                Door door = candidate.Tile.Structure as Door;
                if (door == null && candidate.Tile.BlocksLight)
                    continue;

                if (candidate.Tile.Creature != null)
                {
                    if (candidate.Tile.Creature == Engine.Instance.Player)
                        return candidate.Tile;
                    continue;
                }
                return candidate.Tile;
            }
            return null;
        }

        protected Tile GetNextTileInScentTrail()
        {
            Tile smelliestNeighbour = (from n in Location.GetNeighbours() orderby n.ScentLevel descending select n).FirstOrDefault();
            if (smelliestNeighbour.ScentLevel > Location.ScentLevel)
                return smelliestNeighbour;
            return null;
        }

        Gun _selectedGun;
        internal Gun SelectedGun
        {
            get
            {
                return _selectedGun;
            }
        }

        /// <summary>
        /// Interact with object on neighbouring tile
        /// </summary>
        internal void Interact()
        {
            foreach (Tile tile in Location.GetNeighbours())
            {
                Door door = tile.Structure as Door;
                if (door != null)
                {
                    if (door.IsOpen)
                        door.Close();
                    else
                        door.Open();

                    //if a door opens and closes need to update nearby light fields
                    //TODO redesign this, no need to update every light on the map
                    //refactor
                    foreach (PointLight light in Core.Engine.Instance.World.Lights)
                        light.RecalculateLightfield();
                    EndTurn();
                }
            }
        }

        public void WeildGun(Gun gun)
        {
            _selectedGun = gun;
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

        public virtual void PostTurn()
        {
            
        }

        protected Sound _heardSound;
        public virtual void Hear(Sound sound)
        {
            _heardSound = sound;
        }

        public short Health 
        {
            get
            {
                return _health;
            }
        }

        public abstract CreatureType Type
        {
            get;
        }

        public virtual void Serialize(BinaryWriter bw)
        {
            bw.Write((short)_health);
        }

        public virtual void Deserialize(BinaryReader br, World world)
        {
            _health = br.ReadInt16();
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
