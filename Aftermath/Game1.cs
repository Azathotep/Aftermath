#region Using Statements
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Aftermath.Core;
using Aftermath.Rendering;
#endregion

namespace Aftermath
{
    /// <summary>
    /// Main XNA game class. Glue the game engine to this class.
    /// </summary>
    public class Game1 : Game
    {
        Engine _engine;
        GraphicsDeviceManager _graphics;
        XnaRenderer _renderer;

        public Game1()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            _renderer = new XnaRenderer(_graphics, Content, Window);
            _renderer.SetDeviceMode(800, 600, false); //(1024, 768, false);
            
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _renderer.Initialize();
            _engine = new Engine(_renderer);
            _engine.Initialize();
            _engine.OnExit += new EventHandler(_engine_OnExit);
            base.Initialize();
        }

        void _engine_OnExit(object sender, EventArgs e)
        {
            Exit();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _engine.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _engine.UpdateFrame(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _engine.DrawFrame(gameTime);
            base.Draw(gameTime);
        }
    }
}
