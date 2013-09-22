using System;

namespace Aftermath.Utils
{
    /// <summary>
    /// Provides quick access to RNG
    /// </summary>
    class Dice
    {
        static Random _r = new Random();

        public static int Next(int maxValue)
        {
            return _r.Next(maxValue);
        }
    }
}
