using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Aftermath.Rendering;
using Aftermath.Input;
using Aftermath.Map;
using Aftermath.Creatures;

namespace Aftermath.Core
{
    /// <summary>
    /// The central engine of the game. Manages initialization and calls in from the main game loop. Provides properties to access 
    /// the main subsystems of the game.
    /// </summary>
    class Engine
    {
        //TODO This class should not be coupled to XNA
        XnaRenderer _renderer;
        TextureManager _textureManager;
        IKeyboardHandler _keyboardHandler;        
        Camera _camera;
        World _world;
        TurnSystem _turnSystem;
        Player _player;
        FovRecursiveShadowcast _fov;

        public Engine(XnaRenderer renderer)
        {
            _instance = this;
            _renderer = renderer;
            _textureManager = new TextureManager();
            _keyboardHandler = new XnaKeyboardHandler(KeyHandler);
            _camera = new Camera();
            _turnSystem = new TurnSystem();
        }

        static Engine _instance;
        /// <summary>
        /// Allows the engine to be reference from anywhere rather than having to pass stuff down
        /// TODO use dependency injection instead?
        /// </summary>
        public static Engine Instance
        {
            get
            {
                return _instance;
            }
        }

        public FovRecursiveShadowcast Fov
        {
            get
            {
                return _fov;
            }
        }

        public TurnSystem TurnSystem
        {
            get
            {
                return _turnSystem;
            }
        }

        public void Initialize()
        {
            _world = new World(10, 10);
            _camera.Position = new Vector2F(5.5f, 5.5f);
            _player = new Player();
            _turnSystem.RegisterCreature(_player);
            _world.GetRandomEmptyTile().PlaceCreature(_player);

            for (int i = 0; i < 10; i++)
            {
                Zombie zombie = new Zombie();
                _turnSystem.RegisterCreature(zombie);
                _world.GetRandomEmptyTile().PlaceCreature(zombie);
            }

            _fov = new FovRecursiveShadowcast();

            _keyboardHandler.RegisterKey(InputKey.W, retriggerInterval:0);
            _keyboardHandler.RegisterKey(InputKey.A, retriggerInterval: 0);
            _keyboardHandler.RegisterKey(InputKey.S, retriggerInterval: 0);
            _keyboardHandler.RegisterKey(InputKey.D, retriggerInterval: 0);

            _keyboardHandler.RegisterKey(InputKey.Left, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.Right, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.Up, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.Down, retriggerInterval: 20);

            _keyboardHandler.RegisterKey(InputKey.Escape, retriggerInterval: 0);
        }

        void KeyHandler(InputKey key)
        {
            float cameraSpeed = 1f;
            switch (key)
            {
                case InputKey.W:
                    _camera.Move(0, -cameraSpeed);
                    break;
                case InputKey.A:
                    _camera.Move(-cameraSpeed, 0);
                    break;
                case InputKey.S:
                    _camera.Move(0, cameraSpeed);
                    break;
                case InputKey.D:
                    _camera.Move(cameraSpeed, 0);
                    break;
                case InputKey.Escape:
                    if (OnExit != null)
                        OnExit(this, EventArgs.Empty);
                    break;
            }

            //TODO if the player holds down two keys then the first key ends the turn and currentactor becomes null
            //the second key handler then fires but currentactor is now null.
            //for now put a check that the currentactor isn't null but review this later
            if (_turnSystem.CurrentActor != null && _turnSystem.CurrentActor.IsPlayerControlled)
            {
                switch (key)
                {
                    case InputKey.Left:
                        _turnSystem.CurrentActor.Move(CompassDirection.West);
                        break;
                    case InputKey.Right:
                        _turnSystem.CurrentActor.Move(CompassDirection.East);
                        break;
                    case InputKey.Up:
                        _turnSystem.CurrentActor.Move(CompassDirection.North);
                        break;
                    case InputKey.Down:
                        _turnSystem.CurrentActor.Move(CompassDirection.South);
                        break;
                }
            }
        }

        public event EventHandler OnExit;

        HashSet<Tile> _playerVisibleTiles = new HashSet<Tile>();
        HashSet<Tile> _playerSeenTiles = new HashSet<Tile>();
        int _playerLastVisibleFilesFetchTime;

        public void DrawFrame(GameTime gameTime)
        {
            _renderer.Clear();

            //only calculate the visible tiles from the player's perspective if the world has changed
            //since it was last calculated. This prevents an unnecessary FOV calculation every frame
            //TODO put this into the player class?
            if (TurnSystem.MinorTurnNumber != _playerLastVisibleFilesFetchTime)
            {
                _playerVisibleTiles = _player.GetVisibleTiles();
                foreach (Tile visibleTile in _playerVisibleTiles)
                {
                    _playerSeenTiles.Add(visibleTile);
                }
                _playerLastVisibleFilesFetchTime = TurnSystem.MinorTurnNumber;
            }

            //set the camera position to center on the player
            _camera.Position = new Vector2F(_player.X * 1, _player.Y * 1);
            _renderer.SetView(_camera.Position);

            //calculate the number of tiles that can fit on the screen both vertically and horizontally
            //TODO test this works and move it out, only need to calculate when the resolution or zoom changes,
            //not every frame
            Vector2 topLeft = _renderer.ScreenToWorld(0.2f, 0f);
            Vector2 bottomRight = _renderer.ScreenToWorld(1, 1);
            Rectangle tileRangeToDraw = new Rectangle((int)topLeft.X - 1, (int)topLeft.Y - 1, (int)bottomRight.X - (int)topLeft.X + 2, (int)bottomRight.Y - (int)topLeft.Y + 2);

            _renderer.Begin();

            //int width=20;
            //int height = 14;
            int width = 30;
            int height = 20;
            //draw the viewable part of the map to the screen
            for (int y=(int)_camera.Position.Y - height;y<=_camera.Position.Y + height;y++)
                for (int x = (int)_camera.Position.X - width; x <= _camera.Position.X + width; x++)
                {
                    Tile tile = _world.GetTile(x, y);
                    if (tile == null)
                        continue;
                    string textureName = "steel.floor";
                    if (tile.Wall == WallType.Steel)
                    {
                        Tile south = tile.GetNeighbour(CompassDirection.South);
                        if (south != null && south.Wall == WallType.None && _playerSeenTiles.Contains(south))
                        {
                            if (tile.X % 9 == 0)
                                textureName = "steel.wallnorth2";
                            else
                                textureName = "steel.wallnorth1";
                        }
                        else
                            textureName = "steel.solidwall";
                    }

                    bool isVisible = _playerVisibleTiles.Contains(tile);
                    bool hasSeen = _playerSeenTiles.Contains(tile);
                    isVisible = true;
                    if (!isVisible && !hasSeen)
                        continue;
                    _renderer.Draw(_textureManager.GetTexture(textureName), new RectangleF(x, y, 1, 1), 1, 0, new Vector2F(0, 0), Color.AliceBlue);
                    if (isVisible)
                    {
                        if (tile.Creature != null)
                        {
                            GameTexture texture = tile.Creature.Texture;
                            bool flipHorizontal = !tile.Creature.FacingRight;
                            _renderer.Draw(texture, new RectangleF(x, y, 1, 1), 0.5f, 0, new Vector2F(0, 0), Color.AliceBlue, flipHorizontal);
                        }
                    }
                    else
                    {
                        //tile not visible but remembered. Draw dark overlay.
                        _renderer.Draw(new GameTexture("floorTexture", new RectangleI(0, 0, 64, 64)), new RectangleF(x, y, 1, 1), 0.7f, 0, new Vector2F(0, 0), new Color(0, 0, 0, 0.5f));
                    }
                }

            _renderer.End();
        }

        internal void UpdateFrame(GameTime gameTime)
        {
            _keyboardHandler.Update();
            _turnSystem.Update();
        }
    
        internal void LoadContent()
        {
            _textureManager.RegisterSpriteSheetTextures("steel");
        }

        internal HashSet<Tile> GetFov(int eyeX, int eyeY, int sightRadius)
        {
            return _fov.GetFov(_world, new Vector2I(eyeX, eyeY), sightRadius);
        }
    }
}
