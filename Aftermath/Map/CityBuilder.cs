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
            Color glassColor = Color.FromArgb(153, 217, 234);
            //the color of pixels in the bitmap determines which tile types go where
            for (int y = 0; y < bm.Height; y++)
                for (int x = 0; x < bm.Width; x++)
                {
                    Tile tile = city.GetTile(x, y);
                    Color c = bm.GetPixel(x, y);
                    tile.Material = new Floor(FloorType.Pavement);
                    if (c == wallColor)
                        tile.Material = new Wall();
                    if (c == roadColor)
                        tile.Material = new Road(RoadType.Concrete);
                    if (c == roadLineColor)
                        tile.Material = new Road(RoadType.Line);
                    if (c == carpetColor)
                        tile.Material = new Floor(FloorType.Carpet);
                    if (c == doorColor)
                        tile.Material = new Door();
                    if (c == glassColor)
                        tile.Material = new Glass();
                }
            return city;
        }
    }
}
