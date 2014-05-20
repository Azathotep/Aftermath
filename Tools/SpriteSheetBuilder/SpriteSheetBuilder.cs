using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;

namespace SpriteSheetBuilder
{
    class SpriteSheetBuilder
    {
        /// <summary>
        /// Inserts an image into the sprite sheet at the specified location
        /// </summary>
        /// <param name="sheet">sprite sheet to add image to</param>
        /// <param name="filePath">full path of image file</param>
        /// <param name="sheetX">X position in the spite sheet to place the image</param>
        /// <param name="sheetY">X position in the spite sheet to place the image</param>
        void AddFileToSheet(Graphics sheet, string filePath, int sheetX, int sheetY)
        {
            Image input = Bitmap.FromFile(filePath);

            int imageSize = input.Width;
            int subImageSize = input.Width + 2;

            int outX = sheetX * subImageSize;
            int outY = sheetY * subImageSize;
            using (Graphics inputGraphics = Graphics.FromImage(input))
            {
                sheet.DrawImage(input, outX + 1, outY + 1, input.Width, imageSize);
                sheet.DrawImage(input, new Rectangle(outX, outY + 1, 1, imageSize), new Rectangle(0, 0, 1, imageSize), GraphicsUnit.Pixel);
                sheet.DrawImage(input, new Rectangle(outX + subImageSize - 1, outY + 1, 1, imageSize), new Rectangle(imageSize - 1, 0, 1, imageSize), GraphicsUnit.Pixel);
                sheet.DrawImage(input, new Rectangle(outX + 1, outY, imageSize, 1), new Rectangle(0, 0, imageSize, 1), GraphicsUnit.Pixel);
                sheet.DrawImage(input, new Rectangle(outX + 1, outY + subImageSize - 1, imageSize, 1), new Rectangle(0, imageSize - 1, imageSize, 1), GraphicsUnit.Pixel);
                sheet.DrawImage(input, new Rectangle(outX, outY, 1, 1), new Rectangle(0, 0, 1, 1), GraphicsUnit.Pixel);
                sheet.DrawImage(input, new Rectangle(outX + subImageSize - 1, outY, 1, 1), new Rectangle(imageSize - 1, 0, 1, 1), GraphicsUnit.Pixel);
                sheet.DrawImage(input, new Rectangle(outX, outY + subImageSize - 1, 1, 1), new Rectangle(0, imageSize - 1, 1, 1), GraphicsUnit.Pixel);
                sheet.DrawImage(input, new Rectangle(outX + subImageSize - 1, outY + subImageSize - 1, 1, 1), new Rectangle(imageSize - 1, imageSize - 1, 1, 1), GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// Creates a new spritesheet from the specified folder of image files and saves
        /// it into the folder with the same name as that folder
        /// </summary>
        /// <param name="inputFolder">input folder containing all the image files to include in the spritesheet.
        /// The images must be square and a multiple of two in width</param>
        public void CreateFromFolder(string inputFolder, string outputFolder)
        {
            string[] files = Directory.GetFiles(inputFolder, "*.png");

            DirectoryInfo di = new DirectoryInfo(inputFolder);

            string sheetName = di.Name.ToLower();

            string sheetImageFilePath = inputFolder + @"\" + sheetName + ".png";
            File.Delete(sheetImageFilePath);

            //spritesheet will hold nxn images. Work out n from the number of images
            int numImagesWidth = (int)Math.Ceiling(Math.Sqrt(files.Count()));

            //every image is assumed to be the same size and nxn pixels. Obtain this size from the first image
            Image image = Bitmap.FromFile(files[0]);
            int imageSize = image.Width;
            //a 1 pixel border is placed around each image in the spritesheet to work around sampling problems
            int subImageSize = imageSize + 2;

            //construct an xml document to provide metadata for the spritesheet
            XElement mainElement = new XElement("main");
            mainElement.SetAttributeValue("size", imageSize);

            //build the spritesheet from each file in the input folder
            Image output = new Bitmap(subImageSize * numImagesWidth, subImageSize * numImagesWidth);
            int numImages = 0;
            using (Graphics outputGraphics = Graphics.FromImage(output))
            {
                int x = 0, y = 0;
                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    //ignore the sheet image file
                    if (String.Equals(fileName, sheetName, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    AddFileToSheet(outputGraphics, file, x, y);

                    //add xml element for image
                    XElement imageElement = new XElement("image");
                    imageElement.SetAttributeValue("name", Path.GetFileNameWithoutExtension(file));
                    imageElement.SetAttributeValue("x", x * subImageSize + 1);
                    imageElement.SetAttributeValue("y", y * subImageSize + 1);
                    mainElement.Add(imageElement);

                    x++;
                    if (x >= numImagesWidth)
                    {
                        x = 0;
                        y++;
                    }
                    numImages++;
                }
            }
            //save the sheet and its xml data
            mainElement.Save(outputFolder + @"\" + sheetName + ".xml");
            sheetImageFilePath = outputFolder + @"\" + sheetName + ".png";
            //System.Threading.Thread.Sleep(50000);
            File.Delete(sheetImageFilePath);
            output.Save(sheetImageFilePath, ImageFormat.Png);
            Console.WriteLine("Created spritesheet (" + sheetName + ") containing " + numImages + " sprites");
        }
    }
}
