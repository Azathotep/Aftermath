using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aftermath.AI.Navigation
{
    /// <summary>
    /// AStar node used for AStarAlgorithm search
    /// </summary>
    class AStarNode
    {
        public AStarNode(INavigatableNode node, AStarNode parent, int f, int g)
        {
            Parent = parent;
            Node = node;
            F = f;
            G = g;
        }

        /// <summary>
        /// Previous node in the shortest path to this node found so far
        /// </summary>
        public AStarNode Parent;

        /// <summary>
        /// Underlying object on the navigation map that this AStar node wraps around
        /// </summary>
        public INavigatableNode Node;

        /// <summary>
        /// F value of node (G+H). Cost so far to this node + heuristic based cost to goal
        /// </summary>
        public int F;

        /// <summary>
        /// G value of node. Cost of shortest path to this node found so far
        /// </summary>
        public int G;
    }
}
