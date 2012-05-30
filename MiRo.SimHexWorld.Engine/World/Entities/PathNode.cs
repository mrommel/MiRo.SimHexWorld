using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    /// <summary>
    /// A node in an A* calculated path. 
    /// </summary>
    public class PathNode : IComparable, IComparable<PathNode>, IEquatable<PathNode>
    {
        Unit unit;

        /// <summary>
        /// The preceding node which lead to this node
        /// </summary>
        PathNode predecessor;

        /// <summary>
        /// The corresponding grid cell on the 2d map
        /// </summary>
        IGridCell cell;

        /// <summary>
        /// Destination coordinates in 2d map space
        /// </summary>
        HexPoint goal;

        /// <summary>
        /// base cost summed up for every already visited node
        /// </summary>
        float g;

        /// <summary>
        /// cost regarding to the heuristic, estimating the left distance
        /// to the goal.
        /// </summary>
        float h;

        /// <summary>
        /// total cost of this node (g + h)
        /// </summary>
        float f;

        /// <summary>
        /// base cost summed up for every already visited node
        /// </summary>
        public float G { get { return g; } }

        /// <summary>
        /// cost regarding to the heuristic, estimating the left distance
        /// to the goal.
        /// </summary>
        public float H { get { return h; } }

        /// <summary>
        /// total cost of this node (g + h)
        /// </summary>        
        public float F { get { return f; } }

        /// <summary>
        /// The grid cell on the 2d map represented by this node
        /// </summary>
        public IGridCell Cell { get { return cell; } }

        /// <summary>
        /// The preceding node which lead to this node
        /// </summary>
        public PathNode Predecessor
        {
            get { return predecessor; }
            set { predecessor = value; }
        }

        /// <summary>
        /// Creates a new path node. The cost (or weight) of this node is automatically
        /// calculated according to the predecessor, cell and goal.
        /// </summary>
        /// <param name="predecessor">preceding node</param>
        /// <param name="cell">grid cell represented by the node</param>
        /// <param name="goal">goal coordinate in 2d map space</param>
        public PathNode(Unit unit, PathNode predecessor, IGridCell cell, HexPoint goal)
        {
            this.unit = unit;
            this.cell = cell;
            this.predecessor = predecessor;
            this.goal = goal;

            Update();
        }

        /// <summary>
        /// Recalculates the costs. This is neccessary when the predecessor has changed.
        /// </summary>
        public void Update()
        {
            if (cell.Matches(goal))
            {
                // The path ends here
                g = 1;
                h = 1;
                f = 1;
            }
            else
            {
                // cost for moving from start to this node
                g = (predecessor != null ? predecessor.G : 0) + cell.Weight(unit);

                // heuristic estimating the cost for moving from this node to the goal by Manhattan distance
                h = 10 * (Math.Abs(cell.X - goal.X) + Math.Abs(cell.Y - goal.Y));

                // the combined cost is later used to compare nodes
                f = g + h;
            }
        }

        #region IComparable Member

        /// <summary>
        /// Path nodes are soley compared by comparing their costs (F).
        /// </summary>
        /// <param name="obj">An other PathNode</param>
        /// <returns>Returns -1 when the the other path node is more expensive, 1 if it is less expensive and 0 otherwise</returns>
        public int CompareTo(object obj)
        {
            PathNode node = obj as PathNode;
            if (node.F < f)
                return 1;
            else if (node.F > f)
                return -1;
            else
                return 0;
        }

        #endregion

        #region IEquatable<PathNode> Member

        /// <summary>
        /// Two path nodes are equal when they represent the same grid cell.
        /// </summary>
        /// <param name="other">Another path node</param>
        /// <returns>True if both nodes represent the same grid cell</returns>
        public bool Equals(PathNode other)
        {
            return cell == other.cell;
        }

        #endregion

        #region IComparable<PathNode> Member

        /// <summary>
        /// Path nodes are soley compared by comparing their costs (F).
        /// </summary>
        /// <param name="other">An other PathNode</param>
        /// <returns>Returns -1 when the the other path node is more expensive, 1 if it is less expensive and 0 otherwise</returns>       
        public int CompareTo(PathNode other)
        {
            if (other.F < f) return 1;
            else if (other.F > f) return -1;
            else return 0;
        }

        #endregion

        public HexPoint Point
        {
            get
            {
                return new HexPoint(cell.X, cell.Y);
            }
        }
    }
}
