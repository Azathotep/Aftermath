using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Map;
using Aftermath.Utils;

namespace Aftermath.Core
{
    /// <summary>
    /// Implementation of the recursive shadowcast algorithm as described at roguebasin
    /// (http://roguebasin.roguelikedevelopment.org/index.php?title=FOV_using_recursive_shadowcasting).
    /// A flaw with this FOV algorithm is that it not symmetrical.
    /// </summary>
    public class FovRecursiveShadowcast
    {
        //TODO decouple this class from the game (World and Tile classes)
        Vector2I _eye;
        int _viewRadius;
        World _world;

        public FovRecursiveShadowcast(World world)
        {
            _world = world;
        }

        public HashSet<Tile> GetFov(Vector2I eye, int viewRadius)
        {
            HashSet<Tile> ret = new HashSet<Tile>();
            _eye = eye;
            _viewRadius = viewRadius;
            OctScan(1, 1, 1, 0, ret);
            OctScan(2, 1, -1, 0, ret);
            OctScan(3, 1, -1, 0, ret);
            OctScan(4, 1, 1, 0, ret);
            OctScan(5, 1, 1, 0, ret);
            OctScan(6, 1, -1, 0, ret);
            OctScan(7, 1, -1, 0, ret);
            OctScan(8, 1, 1, 0, ret);
            ret.Add(_world.GetTile(eye.X, eye.Y));
            return ret;
        }

        private void OctScan(int oct, int depth, float slopeStart, float slopeEnd, HashSet<Tile> visibleTiles)
        {
            int gx = 0;
            int gy = 0;
            float endX = 0.5f;
            float endY = 0.5f;
            float startX = 0.5f;
            float startY = 0.5f;
            switch (oct)
            {
                case 1:
                    gx = 1;
                    startX = -0.5f;
                    startY = -0.5f;
                    endX = -0.5f;
                    break;
                case 2:
                    gx = -1;
                    startY = -0.5f;
                    break;
                case 3:
                    gy = 1;
                    startY = -0.5f;
                    endX = -0.5f;
                    endY = -0.5f;
                    break;
                case 4:
                    gy = -1;
                    endX = -0.5f;
                    break;
                case 5:
                    endY = -0.5f;
                    gx = -1;
                    break;
                case 6:
                    startX = -0.5f;
                    endX = -0.5f;
                    endY = -0.5f;
                    gx = 1;
                    break;
                case 7:
                    startX = -0.5f;
                    gy = -1;
                    break;
                case 8:
                    startX = -0.5f;
                    startY = -0.5f;
                    endY = -0.5f;
                    gy = 1;
                    break;
            }

            int dx = 0;
            int dy = 0;
            switch (oct)
            {
                case 1:
                case 2:
                    dx = -(int)Math.Round(depth * slopeStart);
                    dy = -depth;
                    break;
                case 3:
                case 4:
                    dx = depth;
                    dy = (int)Math.Round(depth * slopeStart);
                    break;
                case 5:
                case 6:
                    dx = (int)Math.Round(depth * slopeStart);
                    dy = depth;
                    break;
                case 7:
                case 8:
                    dx = -depth;
                    dy = -(int)Math.Round(depth * slopeStart);
                    break;
            }
            int y;
            int x;
            bool first = true;
            while (true)
            {
                x = _eye.X + dx;
                y = _eye.Y + dy;
                bool atEnd = false;
                switch (oct)
                {
                    case 1:
                    case 5:
                        if (GetSlope(_eye.X, _eye.Y, x, y) < slopeEnd)
                            atEnd = true;
                        break;
                    case 2:
                    case 6:
                        if (GetSlope(_eye.X, _eye.Y, x, y) > slopeEnd)
                            atEnd = true;
                        break;
                    case 3:
                    case 7:
                        if (GetSlopeInv(_eye.X, _eye.Y, x, y) > slopeEnd)
                            atEnd = true;
                        break;
                    case 4:
                    case 8:
                        if (GetSlopeInv(_eye.X, _eye.Y, x, y) < slopeEnd)
                            atEnd = true;
                        break;
                }
                if (atEnd)
                    break;

                if (!first)
                {
                    if (_world.IsTileOpaque(x, y))
                    {
                        if (!_world.IsTileOpaque(x - gx, y - gy))
                        {
                            if (oct == 1 || oct == 2 || oct == 5 || oct == 6)
                                OctScan(oct, depth + 1, slopeStart, GetSlope(_eye.X, _eye.Y, x + endX, y + endY), visibleTiles);
                            else
                                OctScan(oct, depth + 1, slopeStart, GetSlopeInv(_eye.X, _eye.Y, x + endX, y + endY), visibleTiles);
                        }
                    }
                    else
                    {
                        if (_world.IsTileOpaque(x - gx, y - gy))
                        {
                            if (oct == 1 || oct == 2 || oct == 5 || oct == 6)
                                slopeStart = GetSlope(_eye.X, _eye.Y, x + startX, y + startY);
                            else
                                slopeStart = GetSlopeInv(_eye.X, _eye.Y, x + startX, y + startY);
                        }
                    }
                }
                if (dx * dx + dy * dy <= _viewRadius * _viewRadius)
                {
                    Tile tile = _world.GetTile(x, y);
                    if (tile != null)
                        visibleTiles.Add(tile);
                }
                dx += gx;
                dy += gy;
                first = false;
            }
            dx -= gx;
            dy -= gy;

            if (depth < _viewRadius)
            {
                if (!_world.IsTileOpaque(_eye.X + dx, _eye.Y + dy))
                    OctScan(oct, depth + 1, slopeStart, slopeEnd, visibleTiles);
            }
        }

        private float GetSlope(float x, float y, float x2, float y2)
        {
            return (x2 - x) / (y2 - y);
        }

        private float GetSlopeInv(float x, float y, float x2, float y2)
        {
            return (y2 - y) / (x2 - x);
        }
    }
}
