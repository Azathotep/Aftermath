using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aftermath.Utils;
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
        Matrix _worldViewProjection;
        SpriteBatch _spriteBatch;
        GameWindow _window;

        public XnaRenderer(GraphicsDeviceManager deviceManager, ContentManager contentManager, GameWindow window)
        {
            _deviceManager = deviceManager;
            _contentManager = contentManager;
            _window = window;
        }

        SpriteFont _font;

        public void Initialize()
        {
            _spriteBatch = new SpriteBatch(_deviceManager.GraphicsDevice);
            _font = _contentManager.Load<SpriteFont>("Font");
        }

        static Color ClearColor = new Color(50, 50, 50);
        public void Clear()
        {
            _deviceManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, ClearColor, 1, 0);
            _deviceManager.GraphicsDevice.SetRenderTarget(null);
        }

        public void SetDeviceMode(int width, int height, bool fullscreen)
        {
            //fullscreen in windows is not implemented in monogame
            //_deviceManager.IsFullScreen = fullscreen
            //alternative is to use a borderless window and resize
            //downside is that the resolution of the fullscreen window is fixed to the screen resolution
            //and cannot be changed
            if (fullscreen)
            {
                _window.IsBorderless = true;
                _deviceManager.PreferredBackBufferWidth = Screen.PrimaryScreen.Bounds.Width;
                _deviceManager.PreferredBackBufferHeight = Screen.PrimaryScreen.Bounds.Height;
                _window.Position = Point.Zero;
                _deviceManager.IsFullScreen = true;
                
            }
            else
            {
                _window.IsBorderless = false;
                //Strange, when NOT running under the debugger changing the Position of the Window causes the OnClientSizeChanged
                //event to be raised which monogame catches and resets the backbufferwidth and height. So need to set position here first.
                _window.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - width / 2,
                                             Screen.PrimaryScreen.Bounds.Height / 2 - height / 2);
                _deviceManager.PreferredBackBufferWidth = width;
                _deviceManager.PreferredBackBufferHeight = height;
                
            }
            _deviceManager.ApplyChanges();
        }

        public void Begin(Matrix world, Matrix projection, Matrix view)
        {
            Effect effect = _contentManager.Load<Effect>("basicshader.mgfxo");
            effect.Parameters["xWorld"].SetValue(world);
            effect.Parameters["xProjection"].SetValue(projection);
            effect.Parameters["xView"].SetValue(view);
            _spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect, Matrix.CreateScale(1));
        }

        public void End()
        {
            _spriteBatch.End();
        }


        public void Draw(GameTexture texture, RectangleF dest, float depth, float rotation, Vector2F origin, bool flipHorizontal = false)
        {
            Draw(texture, dest, depth, rotation, origin, Color.AliceBlue, flipHorizontal);
        }

        public void Draw(GameTexture texture, RectangleF dest, float depth, float rotation, Vector2F origin, Color color, bool flipHorizontal = false)
        {
            Texture2D xnaTexture = _contentManager.Load<Texture2D>(texture.Name);

            //Rectangle? sourceRect = null;
            
            //if (source.HasValue)
            //    sourceRect = new Rectangle(source.Value.X, source.Value.Y, source.Value.Width, source.Value.Height);

            Rectangle sourceRect = new Rectangle(texture.Bounds.X, texture.Bounds.Y, texture.Bounds.Width, texture.Bounds.Height);
            float scaleW = 1 / (float)sourceRect.Width * dest.Width;
            float scaleH = 1 / (float)sourceRect.Height * dest.Height;

            origin.X *= sourceRect.Width;
            origin.Y *= sourceRect.Height;

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

        public Vector2 MeasureString(string text)
        {
            return _font.MeasureString(text);
        }

        public RectangleF DrawStringBox(string text, RectangleF bounds, Color color, bool rightAlign = false)
        {
            float screenWidth = bounds.Width; //ViewWidth;
            float screenHeight = bounds.Height; //ViewHeight;
            string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string drawString = "";
            string lineSoFar = "";
            string linePlusWord = "";
            foreach (string word in words)
            {
                if (lineSoFar.Length > 0)
                    linePlusWord += " ";
                linePlusWord += word;
                Vector2 newSize = _font.MeasureString(linePlusWord);
                if (newSize.X >= screenWidth)
                {
                    drawString += lineSoFar + Environment.NewLine;
                    lineSoFar = word;
                    linePlusWord = lineSoFar;
                }
                else
                    lineSoFar = linePlusWord;
            }
            if (lineSoFar.Length > 0)
                drawString += lineSoFar;

            //Vector2 worldPos = ScreenToWorld(bounds.X, bounds.Y);
            Vector2 worldPos = new Vector2(bounds.X, bounds.Y);
            float _worldScreenScale = 1f;
            _spriteBatch.DrawString(_font, drawString, worldPos, color, 0, Vector2.Zero, _worldScreenScale, SpriteEffects.None, 0.1f);

            //bounds.Width = width;
            //bounds.Height = height;
            return bounds;
        }
    }
}
