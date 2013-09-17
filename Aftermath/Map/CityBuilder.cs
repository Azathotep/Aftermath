using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Aftermath.Map
{
    /// <summary>
    /// Class that provides methods to build a city map
    /// </summary>
    class CityBuilder
    {
        /// <summary>
        /// Create a city from a bitmap. Each pixel of the bitmap is a tile and the color indicates what
        /// type of tile
        /// </summary>
        public World FromBitmap(string filePath)
        {
            World city;
            Bitmap bm = Bitmap.FromFile(filePath) as Bitmap;
            city = new World(bm.Width, bm.Height);

            Color wallColor = Color.FromArgb(195, 195, 195);
            Color roadColor = Color.FromArgb(127, 127, 127);
            Color roadLineColor = Color.FromArgb(255, 255, 0);
            Color carpetColor = Color.FromArgb(239, 228, 176);
            Color doorColor = Color.FromArgb(0, 0, 0);
            //the color of pixels in the bitmap determines which tile types go where
            for (int y = 0; y < bm.Height; y++)
                for (int x = 0; x < bm.Width; x++)
                {
                    Tile tile = city.GetTile(x, y);
                    Color c = bm.GetPixel(x, y);
                    if (c == wallColor)
                        tile.Wall = WallType.Steel;
                    if (c == roadColor)
                        tile.Floor = FloorType.Road;
                    if (c == roadLineColor)
                        tile.Floor = FloorType.RoadLine;
                    if (c == carpetColor)
                        tile.Floor = FloorType.Carpet;
                    if (c == doorColor)
                        tile.Wall = WallType.Door;
                }
            return city;
        }
    }
}
