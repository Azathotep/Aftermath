using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Aftermath.Rendering
{
    /// <summary>
    /// XNA specific renderer
    /// </summary>
    class XnaRenderer
    {
        GraphicsDeviceManager _deviceManager;
        ContentManager _contentManager;
        Matrix _world;
        Matrix _view;
        Matrix _projection;
        Matrix _worldViewProjection;
        SpriteBatch _spriteBatch;

        public XnaRenderer(GraphicsDeviceManager deviceManager, ContentManager contentManager)
        {
            _deviceManager = deviceManager;
            _contentManager = contentManager;

            _spriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);

            int tilesX = 30; // 40; // 40;
            int tilesY = 24; // 32; // 32;

            //int tilesX = 20;
            //int tilesY = 16;

            //TODO arrghh refactor this
            //Vector2 lookAt = new Vector2(device.Viewport.Width * 0.5f, device.Viewport.Height * 0.5f);
            //_view = Matrix.CreateLookAt(new Vector3(lookAt, 0), new Vector3(lookAt, 10), new Vector3(0, -1, 0));
            _projection = Matrix.CreateOrthographic(tilesX, tilesY, -1000.5f, 100); //40, 32, -100.5f, 100); //_device.Viewport.Width, _device.Viewport.Height, -1000.5f, 100);
            _world = Matrix.CreateScale(1f);

            //the ratio of the world screen width to the view screen width
            //_worldScreenScale.X = tilesX / (float)ViewWidth;
            //_worldScreenScale.Y = tilesY / (float)ViewHeight;
            //PresentationParameters pp = device.PresentationParameters;
            //_mainRenderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
            //_normalRenderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
        }

        static Color ClearColor = new Color(50, 50, 50);
        public void Clear()
        {
            _deviceManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, ClearColor, 1, 0);
            _deviceManager.GraphicsDevice.SetRenderTarget(null);
        }

        public void SetDeviceMode(int width, int height, bool fullscreen)
        {
            _deviceManager.PreferredBackBufferWidth = width;
            _deviceManager.PreferredBackBufferHeight = height;
            _deviceManager.IsFullScreen = fullscreen;
        }

        public void Begin()
        {
            //TODO refactor??
            Effect effect = _contentManager.Load<Effect>("basicshader.mgfxo");
            effect.Parameters["xWorld"].SetValue(_world);
            effect.Parameters["xProjection"].SetValue(_projection);
            effect.Parameters["xView"].SetValue(_view);
            _spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect, Matrix.CreateScale(1));
        }

        public void End()
        {
            _spriteBatch.End();
        }

        internal void SetView(Vector2F cameraLook)
        {
            Vector2 lookAt = new Vector2(cameraLook.X, cameraLook.Y);
            _view = Matrix.CreateLookAt(new Vector3(lookAt, -1), new Vector3(lookAt, 0), new Vector3(0, -1, 0));
            _worldViewProjection = _world * _view * _projection;
        }

        //public void Draw(TextureRef texture, RectangleF bounds, float depth, bool flipHorizontal = false)
        //{
        //    Draw(texture, bounds, depth, 0, Vector2.Zero, Color.White, flipHorizontal);
        //}

        public void Draw(GameTexture texture, RectangleF dest, float depth, float rotation, Vector2F origin, Color color, bool flipHorizontal = false)
        {
            Texture2D xnaTexture = _contentManager.Load<Texture2D>(texture.Name);

            //Rectangle? sourceRect = null;
            
            //if (source.HasValue)
            //    sourceRect = new Rectangle(source.Value.X, source.Value.Y, source.Value.Width, source.Value.Height);

            Rectangle sourceRect = new Rectangle(texture.Bounds.X, texture.Bounds.Y, texture.Bounds.Width, texture.Bounds.Height);
            float scaleW = 1 / (float)sourceRect.Width * dest.Width;
            float scaleH = 1 / (float)sourceRect.Height * dest.Height;

            origin.X *= xnaTexture.Width;
            origin.Y *= xnaTexture.Height;

            SpriteEffects effects = SpriteEffects.None;
            if (flipHorizontal)
                effects = SpriteEffects.FlipHorizontally;

            _spriteBatch.Draw(xnaTexture, new Vector2(dest.X, dest.Y), sourceRect, color, rotation, new Vector2(origin.X, origin.Y), new Vector2(scaleW, scaleH), effects, depth);
        }

        /// <summary>
        /// Converts screen position into world coordinates
        /// </summary>
        /// <param name="x">screen X position from -1 to 1</param>
        /// <param name="y">screen Y position from -1 to 1</param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(float x, float y)
        {
            //TODO not sure this still works...
            x = (x * 2 - 1);
            y = ((1 - y) * 2 - 1);
            Vector2 ret = Vector2.Transform(new Vector2(x, y), Matrix.Invert(_worldViewProjection));
            return ret;
        }
    }
}
