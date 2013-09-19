using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aftermath.AI.Navigation
{
    /// <summary>
    /// Implementation of A* search algorithm
    /// </summary>
    class AStarAlgorithm
    {
        AStarOpenList _openList = new AStarOpenList();
        HashSet<int> _closedList = new HashSet<int>();

        /// <summary>
        /// Returns the shortest path from one INavigatableNode to another, or null if no valid path exists between
        /// the nodes
        /// </summary>
        /// <param name="from">starting node</param>
        /// <param name="goal">goal node</param>
        /// <returns>List of nodes along path, or null if there is no path between the nodes</returns>
        public List<INavigatableNode> FindPath(INavigatableNode from, INavigatableNode goal)
        {
            _openList.Clear();
            _closedList.Clear();
            _openList.AddOrReplaceNode(from, null, 0, 0);

            while (!_openList.IsEmpty)
            {
                AStarNode node = _openList.RemoveLowestFNode();
                _closedList.Add(node.Node.NodeUid);
                if (node.Node.NodeUid == goal.NodeUid)
                    return GetShortestPathSoFar(node);

                foreach (var neighbour in node.Node.GetNeighbours())
                {
                    if (_closedList.Contains(neighbour.NodeUid))
                        continue;
                    int g = node.G + node.Node.GetCostTo(node, neighbour);
                    int h = neighbour.GetHeuristicCostTo(goal);
                    int f = g + h;
                    _openList.AddOrReplaceNode(neighbour, node, f, g);
                }
            }
            return new List<INavigatableNode>();
        }

        /// <summary>
        /// Returns the shortest path to the specified node found so far
        /// </summary>
        List<INavigatableNode> GetShortestPathSoFar(AStarNode endNode)
        {
            List<INavigatableNode> ret = new List<INavigatableNode>();
            for (AStarNode node = endNode; node != null; node = node.Parent)
                ret.Add(node.Node);
            ret.Reverse();
            return ret;
        }

        /// <summary>
        /// Open List implementation
        /// </summary>
        class AStarOpenList
        {
            //open list uses a binary heap
            BinaryHeap<INavigatableNode> _heap = new BinaryHeap<INavigatableNode>();
            //map NodeId to AStarNode so that AStarNodes in the open list 
            //can be looked up quickly given a INavigatableNode
            Dictionary<int, AStarNode> _nodes = new Dictionary<int, AStarNode>();

            /// <summary>
            /// Remove and return the node with the lowest F score (cost so far + heuristic cost to target) from the open list
            /// </summary>
            /// <returns></returns>
            public AStarNode RemoveLowestFNode()
            {
                INavigatableNode lowest = _heap.Remove();
                AStarNode ret = _nodes[lowest.NodeUid];
                _nodes.Remove(lowest.NodeUid);
                return ret;
            }

            /// <summary>
            /// Add a node to the open list. If a matching node already exists in the open list it's values are
            /// updated if the existing node's F score is higher than the new node's F score
            /// </summary>
            /// <param name="node">node to add</param>
            /// <param name="parent">parent of node (the last node in path)</param>
            /// <param name="f">f score of new node (g + h)</param>
            /// <param name="g">g score of new node (cost so far)</param>
            public void AddOrReplaceNode(INavigatableNode node, AStarNode parent, int f, int g)
            {
                AStarNode existingNode;
                if (_nodes.TryGetValue(node.NodeUid, out existingNode))
                {
                    int xn = existingNode.F;
                    if (f < existingNode.F)
                    {
                        //update existing node
                        existingNode.Parent = parent;
                        existingNode.F = f;
                        existingNode.G = g;
                        _heap.Reposition(existingNode.Node, f);
                    }
                }
                else
                {
                    AStarNode newNode = new AStarNode(node, parent, f, g);
                    _nodes.Add(node.NodeUid, newNode);
                    _heap.Add(node, f);
                }
            }

            /// <summary>
            /// Whether the open list is empty
            /// </summary>
            public bool IsEmpty
            {
                get
                {
                    return _heap.Count == 0;
                }
            }

            /// <summary>
            /// Clears the open list
            /// </summary>
            public void Clear()
            {
                _nodes.Clear();
                _heap.Clear();
            }
        }
    }
}
