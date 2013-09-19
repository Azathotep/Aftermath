using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aftermath.AI.Navigation
{
    /// <summary>
    /// Supports A* search
    /// </summary>
    interface INavigatableNode
    {
        /// <summary>
        /// Returns the neighbours of this node
        /// </summary>
        INavigatableNode[] GetNeighbours();

        /// <summary>
        /// Returns a heuristic based cost to a target node from this node
        /// </summary>
        int GetHeuristicCostTo(INavigatableNode target);

        /// <summary>
        /// Returns the cost of moving from this node to a neighbouring target node
        /// </summary>
        /// <param name="nodeInfo">Information about the route so far</param>
        /// <param name="to">target node</param>
        int GetCostTo(AStarNode nodeInfo, INavigatableNode to);

        /// <summary>
        /// ID that uniquely identifies the node on the navigatable map
        /// </summary>
        int NodeUid
        {
            get;
        }
    }
}
