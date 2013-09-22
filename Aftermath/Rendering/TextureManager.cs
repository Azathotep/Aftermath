using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Utils;

namespace Aftermath.Rendering
{
    /// <summary>
    /// Manages textures for the game. Textures used in the game should be registered through this class.
    /// </summary>
    class TextureManager
    {
        Dictionary<string, GameTexture> _textures = new Dictionary<string, GameTexture>();

        /// <summary>
        /// Register all the spites in the specified spritesheet. Loads the spritesheet's corresponding XML file
        /// (which should be in the Content\SpriteSheets folder)
        /// </summary>
        /// <param name="spriteSheetName">name of the spritesheet. This is the name of the texture marked as 
        /// build action Content (and copied to output folder) without the file extension.</param>
        public void RegisterSpriteSheetTextures(string spriteSheetName)
        {
            SpriteSheet sh = new SpriteSheet();
            sh.Load(@"Content\SpriteSheets\" + spriteSheetName + ".xml");
            foreach (SpriteSheetTexture texture in sh.Images)
            {
                _textures.Add(spriteSheetName + "." + texture.Name, new GameTexture(spriteSheetName, new RectangleI(texture.X, texture.Y, sh.ImageSize, sh.ImageSize)));
            }
        }

        /// <summary>
        /// Obtains a registered texture by name
        /// </summary>
        public GameTexture GetTexture(string name)
        {
            return _textures[name];
        }
    }

    public class GameTexture
    {
        string _name;
        RectangleI _bounds;
        public GameTexture(string name, RectangleI bounds)
        {
            _name = name;
            _bounds = bounds;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public RectangleI Bounds
        {
            get
            {
                return _bounds;
            }
        }
    }
}
