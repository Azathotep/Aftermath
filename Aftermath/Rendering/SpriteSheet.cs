using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace Aftermath.Rendering
{
    /// <summary>
    /// Provides methods to access a spritesheet. A spritesheet consists of an image and an xml file detailing metadata for each
    /// sprite in the spritesheet.
    /// </summary>
    class SpriteSheet
    {
        int _size;
        List<SpriteSheetTexture> _images = new List<SpriteSheetTexture>();

        /// <summary>
        /// Initializes this spritesheet instance from the specified spritesheet XML file
        /// </summary>
        /// <param name="filePath">absolute or relative path to the spritesheet XML file</param>
        public void Load(string filePath)
        {
            XElement mainElement = XElement.Load(filePath);
            _size = (int)mainElement.Attribute("size");
            var images = (from m in mainElement.Descendants("image") select new { Name = m.Attribute("name"), X = m.Attribute("x"), Y = m.Attribute("y") });
            foreach (var i in images)
            {
                SpriteSheetTexture image = new SpriteSheetTexture();
                image.Name = (string)i.Name;
                image.X = (int)i.X;
                image.Y = (int)i.Y;
                _images.Add(image);
            }
        }

        /// <summary>
        /// The width and height of each sprite in the spritesheet. All sprites in a sheet
        /// must be the same size and square.
        /// </summary>
        public int ImageSize
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Returns information about each sprite in this spritesheet
        /// </summary>
        public List<SpriteSheetTexture> Images
        {
            get
            {
                return _images;
            }
        }
    }

    class SpriteSheetTexture
    {
        public string Name;
        public int X;
        public int Y;
    }
}
