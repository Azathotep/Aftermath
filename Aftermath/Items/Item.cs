using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Map;
using Aftermath.Rendering;
using Aftermath.Core;

namespace Aftermath.Items
{
    public abstract class Item
    {
        Tile _location;
        public Tile Location
        {
            get
            {
                return _location;
            }
        }

        public void Place(Tile targetTile)
        {
            _location = targetTile;
            targetTile.Item = this;
        }

        public abstract GameTexture Texture
        {
            get;
        }

        internal void RemoveFromTile()
        {
            _location.Item = null;
            _location = null;
        }
    }
}
