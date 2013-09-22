using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Utils;
using Microsoft.Xna.Framework;

namespace Aftermath.Core
{
    /// <summary>
    /// Provides methods to manage a camera position and view in the world
    /// </summary>
    class Camera
    {
        /// <summary>
        /// Camera position, coordinates of the center of the camera view
        /// </summary>
        public Vector2F Position = new Vector2F();

        public float WorldScale = 1;

        public Vector2F ScreenToWorld(int x, int y)
        {
            Vector2F ret = new Vector2F((x + Position.X) / WorldScale, (y + Position.Y) / WorldScale);
            return ret;
        }

        public void Move(float x, float y)
        {
            Position.X += x;
            Position.Y += y;
        }
    }
}
