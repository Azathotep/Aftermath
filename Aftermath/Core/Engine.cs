using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Aftermath.Rendering;
using Aftermath.Input;
using Aftermath.Map;
using Aftermath.Creatures;
using Aftermath.Animations;
using Aftermath.State;
using Aftermath.UI;
using Aftermath.Lighting;
using Aftermath.Scenarios;

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
        
        //TODO this shouldnt be public
        public TargetingModule _targetingModule = new TargetingModule();
        AnimationManager _animationManager = new AnimationManager();

        UIManager _uiManager = new UIManager();

        public Engine(XnaRenderer renderer)
        {
            _instance = this;
            _renderer = renderer;
            _textureManager = new TextureManager();
            _keyboardHandler = new XnaKeyboardHandler(KeyHandler);
            _camera = new Camera();
            _turnSystem = new TurnSystem();
            _turnSystem.OnTurnAdvanced += _turnSystem_OnTurnAdvanced;
        }

        void _turnSystem_OnTurnAdvanced()
        {
            //advance the time of day each turn (minutes)
            _world.TimeOfDay += 1;
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

        public UIManager UIManager
        {
            get
            {
                return _uiManager;
            }
        }

        public TurnSystem TurnSystem
        {
            get
            {
                return _turnSystem;
            }
        }

        TargetingModule Crosshair
        {
            get
            {
                return _targetingModule;
            }
        }

        public TextureManager TextureManager
        {
            get
            {
                return _textureManager;
            }
        }

        public AnimationManager AnimationManager
        {
            get
            {
                return _animationManager;
            }
        }

        public void Initialize()
        {
            _fov = new FovRecursiveShadowcast();
            _turnSystem.RegisterTurnInhibitor(_animationManager);
            

            CityBuilder builder = new CityBuilder();
            _world = builder.FromBitmap(@"Content\city.bmp");

            _player = new Player();
            _player.WeildGun(new Creature.Gun());
            _world.GetRandomEmptyTile().PlaceCreature(_player);

            _world.TimeOfDay = 8 * 60;

            //give the player a flashlight
            //TODO refactor
            _player.Flashlight = new PointLight(_player.Location, 4, new Color(0.8f, 0.8f, 0.2f));
            _world.Lights.Add(_player.Flashlight);
            _player.Flashlight.RecalculateLightfield();

            for (int i = 0; i < 50; i++)
            {
                Zombie zombie = new Zombie();
                Tile tile = _world.GetRandomEmptyTile();
                if (tile.GetChebyshevDistanceFrom(_player.Location) < 15)
                {
                    i--;
                    continue;
                }
                tile.PlaceCreature(zombie);
            }

            //LoadScenario(new Tutorial1());

            
            _keyboardHandler.RegisterKey(InputKey.W, retriggerInterval:0);
            _keyboardHandler.RegisterKey(InputKey.A, retriggerInterval: 0);
            _keyboardHandler.RegisterKey(InputKey.S, retriggerInterval: 0);
            _keyboardHandler.RegisterKey(InputKey.D, retriggerInterval: 0);

            _keyboardHandler.RegisterKey(InputKey.F, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.R, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.I, retriggerInterval: 20);

            _keyboardHandler.RegisterKey(InputKey.Left, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.Right, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.Up, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.Down, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.OemPeriod, retriggerInterval: 20);

            _keyboardHandler.RegisterKey(InputKey.Escape, retriggerInterval: 20);
            _keyboardHandler.RegisterKey(InputKey.Enter, retriggerInterval: 20);
        }

        void LoadScenario(Scenario scenario)
        {
            Engine.Instance.TurnSystem.Clear();
            scenario.Initialize(out _world, out _player);
        }


        void KeyHandler(InputKey key)
        {
            if (UIManager.ProcessKey(key))
                return;
            GameState.CurrentState.ProcessKey(key);
        }

        public event EventHandler OnExit;

        HashSet<Tile> _playerVisibleTiles = new HashSet<Tile>();
        HashSet<Tile> _playerSeenTiles = new HashSet<Tile>();
        int _playerLastVisibleFilesFetchTime;

        float _zoomAmt = 0;

        public void DrawFrame(GameTime gameTime)
        {
            _renderer.Clear();

            //only calculate the visible tiles from the player's perspective if the world has changed
            //since it was last calculated. This prevents an unnecessary FOV calculation every frame
            //TODO put this into the player class?
            if (TurnSystem.MinorTurnNumber != _playerLastVisibleFilesFetchTime)
            {
                _playerVisibleTiles = _player.GetVisibleTiles(0.4f);
                foreach (Tile visibleTile in _playerVisibleTiles)
                {
                    _playerSeenTiles.Add(visibleTile);
                }
                _playerLastVisibleFilesFetchTime = TurnSystem.MinorTurnNumber;
            }

            //set the camera position to center on the player
            _camera.Position = new Vector2F(_player.Location.X * 1, _player.Location.Y * 1);

            //calculate the number of tiles that can fit on the screen both vertically and horizontally
            //TODO test this works and move it out, only need to calculate when the resolution or zoom changes,
            //not every frame
            Vector2 topLeft = _renderer.ScreenToWorld(0.2f, 0f);
            Vector2 bottomRight = _renderer.ScreenToWorld(1, 1);
            Rectangle tileRangeToDraw = new Rectangle((int)topLeft.X - 1, (int)topLeft.Y - 1, (int)bottomRight.X - (int)topLeft.X + 2, (int)bottomRight.Y - (int)topLeft.Y + 2);

            Matrix world = Matrix.Identity;
            Matrix projection = Matrix.CreateOrthographic(50, 40, -1000.5f, 100); //50, 40, -1000.5f, 100); //
            if (!Engine.Instance.Player.IsAlive)
            {
                projection = Matrix.CreateOrthographic(30 - 15 * _zoomAmt, 24 - 12 * _zoomAmt, -1000.5f, 100);
                if (_zoomAmt < 1.5f)
                    _zoomAmt += 0.1f;
            }

            Vector2 lookAt = new Vector2(_camera.Position.X, _camera.Position.Y);
            Matrix view = Matrix.CreateLookAt(new Vector3(lookAt, -1), new Vector3(lookAt, 0), new Vector3(0, -1, 0));
            
            _renderer.Begin(world, projection, view);

            int width = 50;  //60;
            int height = 40; // 40;
            //draw the viewable part of the map to the screen
            for (int y=(int)_camera.Position.Y - height;y<=_camera.Position.Y + height;y++)
                for (int x = (int)_camera.Position.X - width; x <= _camera.Position.X + width; x++)
                {
                    Tile tile = _world.GetTile(x, y);
                    if (tile == null)
                        continue;

                    float rotation;
                    string textureName = tile.Material.GetTexture(tile, out rotation);

                    bool isVisible = _playerVisibleTiles.Contains(tile);
                    bool hasSeen = _playerSeenTiles.Contains(tile);
                    
                    //obtain the lighting of this tile from the player's location
                    Light light = tile.GetLighting(Engine.Instance.Player.Location);

                    //isVisible = true;

                    if (!isVisible && !hasSeen)
                        continue;
                    Color color;
                    //if the tile is visible then draw the color using the tile's light, otherwise draw it dark
                    if (isVisible)
                        color = light.Color;
                    else
                        color = new Color(0.1f,0.1f,0.1f);

                    _renderer.Draw(_textureManager.GetTexture(textureName), new RectangleF(x, y, 1, 1), 1, rotation, new Vector2F(0.5f, 0.5f), color);
                    if (isVisible)
                    {
                        //draw corpse first
                        if (tile.Corpse != null)
                        {
                            _renderer.Draw(tile.Corpse.Texture, new RectangleF(x, y, 1, 1), 0.6f, 0, new Vector2F(0.5f, 0.5f), Color.AliceBlue);
                        }

                        if (tile.Creature != null)
                        {
                            GameTexture texture = tile.Creature.Texture;
                            bool flipHorizontal = !tile.Creature.FacingRight;
                            _renderer.Draw(texture, new RectangleF(x, y, 1, 1), 0.3f, 0, new Vector2F(0.5f, 0.5f), Color.AliceBlue, flipHorizontal);

                            Zombie zombie = tile.Creature as Zombie;
                            if (zombie != null)
                            {
                                string icon = "";
                                if (zombie.State == ZombieState.Alert)
                                    icon = "overlay.warning";
                                else if (zombie.State == ZombieState.Enraged)
                                    icon = "overlay.danger";
                                if (icon != "")
                                    _renderer.Draw(_textureManager.GetTexture(icon), new RectangleF(x, y-0.8f, 0.5f, 0.5f), 0.2f, 0, new Vector2F(0.5f, 0.5f));
                            }
                        }
                    }
                    else
                    {
                        //tile not visible but remembered. Draw an overlay indicating that tile is not visible.
                        //DrawTileOverlay(_renderer, tile, new Color(0f, 0f, 0, 0.9f));
                        _renderer.Draw(_textureManager.GetTexture("overlay.gauze"), new RectangleF(x, y, 1, 1), 0.7f, 0, new Vector2F(0.5f, 0.5f), new Color(0, 0, 0, 0.3f));
                    }
                }

            foreach (Animation animation in _animationManager.Animations)
            {
                animation.Render(_renderer);
            }

            if (GameState.CurrentState == GameState.InteractState)
            {
                DrawTileOverlay(_renderer, _player.Location, new Color(0, 0.2f, 0.2f, 0.005f));
                foreach (CompassDirection d in Compass.CardinalDirections)
                    DrawTileOverlay(_renderer, _player.Location.GetNeighbour(d), new Color(0, 0.2f, 0.2f, 0.01f));
            }

            //draw the crosshair if in aim mode
            if (GameState.CurrentState == GameState.AimingState)
            {
                DrawCrosshair();
            }

            _renderer.End();

            //Beginning of UI
            //TODO refactor

            world = Matrix.CreateScale(1f);
            //Matrix projection = Matrix.CreateOrthographic(1024, 768, -1000.5f, 500);
            //Matrix view = Matrix.CreateLookAt(new Vector3(512, 384, -100), new Vector3(512, 384, 0), new Vector3(0, -1, 0));
            projection = Matrix.CreateOrthographic(800, 600, -1000.5f, 500);
            view = Matrix.CreateLookAt(new Vector3(400, 300, -1), new Vector3(400, 300, 0), new Vector3(0, -1, 0));

            _renderer.Begin(world, projection, view);

            //display bullets
            for (int x = 0; x < _player.SelectedGun.LoadedAmmo; x++)
            {
                _renderer.Draw(new GameTexture("bullet", new RectangleI(0, 0, 32, 32)), new RectangleF(0 + x * 16, 0, 32, 32), 0.2f, 0, new Vector2F(0f, 0f), Color.AliceBlue);
            }

            _renderer.DrawStringBox("Ammo: " + _player.SelectedGun.LoadedAmmo, new RectangleF(200, 10, 550, 50), Color.White);

            if (!Engine.Instance.Player.IsAlive)
            {
                _renderer.DrawStringBox("You have been eaten by a zombie. Press Esc to exit.", new RectangleF(300, 250, 200, 100), Color.Black);
                _renderer.DrawStringBox("You have been eaten by a zombie. Press Esc to exit.", new RectangleF(301, 251, 200, 100), Color.White);
            }

            _uiManager.RenderUI(_renderer);
            
            _renderer.End();
        }

        private void DrawTileOverlay(XnaRenderer renderer, Tile tile, Color color)
        {
            renderer.Draw(_textureManager.GetTexture("overlay.white"), new RectangleF(tile.X, tile.Y, 1, 1), 0.4f, 0, new Vector2F(0.5f, 0.5f), color);
        }

        /// <summary>
        /// Set of tiles the player has seen/remembered
        /// </summary>
        public HashSet<Tile> PlayerSeenTiles
        {
            get
            {
                return _playerSeenTiles;
            }
        }

        void DrawCrosshair()
        {
            //if the target is out of range or not visible then draw a red square to indicate it cannot be fired
            //TODO refactor
            bool validTarget = true;
            if (!_playerVisibleTiles.Contains(_targetingModule.Tile))
                validTarget = false;

            if (!validTarget)
                _renderer.Draw(_textureManager.GetTexture("steel.floor"), new RectangleF(_targetingModule.Tile.X, _targetingModule.Tile.Y, 1, 1), 0.4f, 0, new Vector2F(0.5f, 0.5f), new Color(0.2f, 0, 0, 0.02f));
            _renderer.Draw(_textureManager.GetTexture("overlay.crosshair"), new RectangleF(_targetingModule.Tile.X, _targetingModule.Tile.Y, 1, 1), 0.3f, 0, new Vector2F(0.5f, 0.5f), Color.AliceBlue);
        }

        internal void UpdateFrame(GameTime gameTime)
        {
            _keyboardHandler.Update();
            _animationManager.Update();
            _turnSystem.Update();
        }
    
        internal void LoadContent()
        {
            _textureManager.RegisterSpriteSheetTextures("steel");
            _textureManager.RegisterSpriteSheetTextures("road");
            _textureManager.RegisterSpriteSheetTextures("house");
            _textureManager.RegisterSpriteSheetTextures("overlay");
        }

        internal HashSet<Tile> GetFov(Tile eyePosition, int sightRadius)
        {
            return _fov.GetFov(_world, new Vector2I(eyePosition.X, eyePosition.Y), sightRadius);
        }

        internal void Exit()
        {
            if (OnExit != null)
                OnExit(this, EventArgs.Empty);
        }

        public Player Player 
        {
            get
            {
                return _player;
            }
            set
            {
                _player = value;
            }
        }

        public World World 
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
            }
        }
    }
}
