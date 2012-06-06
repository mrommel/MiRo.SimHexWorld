using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using System.Diagnostics;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    public class WayPoints
    {
        Queue<HexPoint> _points = new Queue<HexPoint>();
        HexPoint _goal;

        public bool Finished
        {
            get { return _points.Count == 0; }
        }

        public HexPoint Peek
        {
            get { return _points.Peek(); }
        }

        public HexPoint Pop()
        {
            return _points.Dequeue();
        }

        public static WayPoints FromPath(Path path)
        {
            WayPoints points = new WayPoints();

            while (path != null && !path.Finished)
            {
                IGridCell current = path.GetNextWaypoint();

                if (current != null)
                {
                    points.Push(new HexPoint(current.X, current.Y));
                    points._goal = new HexPoint(current.X, current.Y);
                }
            }

            return points;
        }

        protected void Push(HexPoint hexPoint)
        {
            _points.Enqueue(hexPoint);
        }

        public HexPoint Goal 
        { 
            get 
            {
                return _goal;
            } 
        }
    }

    public class Path
    {
        /// <summary>
        /// Current node
        /// </summary>
        PathNode current;

        /// <summary>
        /// Original start node
        /// </summary>
        PathNode start;

        /// <summary>
        /// Creates a new path beginning at the specified start
        /// </summary>
        /// <param name="start">start path node</param>
        public Path(PathNode start)
        {
            current = start;
            this.start = start;
        }

        /// <summary>
        /// Returns true, if the path should be at its end
        /// </summary>
        public bool Finished { get { return current == null; } }

        /// <summary>
        /// Peeks at the next path node without removing it from the path
        /// </summary>
        public PathNode Peek { get { return current; } }

        /// <summary>
        /// Delivers the next waypoint and moves on in the path,
        /// discarding the returned node.
        /// </summary>
        /// <returns></returns>
        public IGridCell GetNextWaypoint()
        {
            PathNode next = current;
            current = current.Predecessor;
            return next.Cell;
        }

        ///// <summary>
        ///// Just for debugging: Renders the complete path from beginning.
        ///// </summary>
        //[Conditional("DEBUG")]
        //public void DrawPath()
        //{
        //    DrawPath(start, start.Predecessor);
        //}

        ///// <summary>
        ///// Recursive function for rendering a path segment as a yellow line.
        ///// </summary>
        ///// <param name="from">Start node</param>
        ///// <param name="to">End node</param>
        //[Conditional("DEBUG")]
        //private void DrawPath(PathNode from, PathNode to)
        //{
        //    if (to == null) return;
        //    DebugDisplay.Instance.DrawLine(from.Cell.Position3D, to.Cell.Position3D, from == start ? Color.Red : Color.Yellow);
        //    DrawPath(to, to.Predecessor);
        //}

        /// <summary>
        /// Copies this instance of the path.
        /// </summary>
        /// <returns></returns>
        public Path Copy()
        {
            Path copy = new Path(start);
            return copy;
        }

        public bool IsValid { get { return start != null; } }

        public override string ToString()
        {
            Path copied = Copy();

            StringBuilder sb = new StringBuilder();

            
            while (!copied.Finished)
            {
                IGridCell current = copied.GetNextWaypoint();

                if (current != null) 
                    sb.Append("[" + current.X + ", " + current.Y + "],");
            }

            return sb.ToString();
        }
    }

}
