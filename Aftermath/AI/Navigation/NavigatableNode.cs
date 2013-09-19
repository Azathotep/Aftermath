using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Map;

namespace Aftermath.AI.Navigation
{
    /// <summary>
    /// Implementation of INavigatableNode for game
    /// </summary>
    class NavigatableNode : INavigatableNode
    {
        Tile _tile;

        public NavigatableNode(Tile tile)
        {
            _tile = tile;
        }

        public override string ToString()
        {
            return "" + NodeUid;
        }

        public Tile Tile
        {
            get
            {
                return _tile;
            }
        }

        public INavigatableNode[] GetNeighbours()
        {
            List<INavigatableNode> ret = new List<INavigatableNode>();
            foreach (CompassDirection direction in Compass.CardinalDirections)
            {
                Tile neighbour = _tile.GetNeighbour(direction);
                if (neighbour == null)
                    continue;
                if (!neighbour.IsPassable)
                    continue;
                ret.Add(new NavigatableNode(neighbour));
            }
            return ret.ToArray();
        }

        public int GetHeuristicCostTo(INavigatableNode target)
        {
            return _tile.GetManhattenDistanceFrom(((NavigatableNode)target)._tile);
        }

        public int GetCostTo(AStarNode nodeInfo, INavigatableNode to)
        {
            return 1;
        }

        public int NodeUid
        {
            get 
            {
                return _tile.X + _tile.Y * 10000;
            }
        }
    }
}
