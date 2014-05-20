using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteSheetBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: SpriteSheetBuilder.exe <input image folder>");
                return;
            }
            string imageFolder = args[0];
            string outputFolder = imageFolder;
            if (args.Length > 1)
                outputFolder = args[1];
            SpriteSheetBuilder sheetBuilder = new SpriteSheetBuilder();
            sheetBuilder.CreateFromFolder(imageFolder, outputFolder);
        }
    }
}
