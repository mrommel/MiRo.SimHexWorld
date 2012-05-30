using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiRo.SimHexWorld.Engine.World.Maps;
using MiRo.SimHexWorld.Engine.Types;

namespace MiRo.SimHexWorld.Engine.World.Entities
{
    /// <summary>
    /// Description of a path request. The path request
    /// can differ from the path itself, because the exact
    /// path might not exist. The path request stores
    /// what path was requested. The actual found path
    /// is stored in a separate Path instance.
    /// </summary>
    public class PathRequest
    {
        public UnitData data;
        public HexPoint start;
        public HexPoint end;

        public PathRequest(Unit unit, HexPoint start, HexPoint end)
        {
            data = unit.Data;
            this.start = start;
            this.end = end;
        }

        public override bool Equals(object obj)
        {
            PathRequest other = (PathRequest)obj;
            return start.Equals(other.start) && end.Equals(other.end) && data.Name == other.data.Name;
        }
    }

    public class Pathfinder
    {
        /// <summary>
        /// Maximum number of steps allowed to find a path.
        /// The more steps allowed the longer the process 
        /// can take. When the step limit is reached the
        /// search is aborted.
        /// </summary>
        int stepLimit = 140;

        /// <summary>
        /// Debug Mode settings. If true paths may be visualized.
        /// </summary>
        public static bool DebugMode { get; set; }

        /// <summary>
        /// The grid map giving information about the 2D grid.
        /// </summary>
        MapData map;

        /// <summary>
        /// List containing known paths
        /// </summary>
        Dictionary<PathRequest, Path> knownPaths = new Dictionary<PathRequest, Path>();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="map"></param>
        public Pathfinder(MapData map)
        {
            this.map = map;
        }

        /// <summary>
        /// Calculates a path from start to end. When no path can be found in
        /// reasonable time the search is aborted and an incomplete path is returned.
        /// When refresh is not set to true a cached path is returned where possible.
        /// </summary>
        /// <param name="start">start position in 2d map space</param>
        /// <param name="end">end position in 2d map space</param>
        /// <param name="refresh">force to recalculate the path</param>
        /// <returns></returns>
        public Path CalculatePath(Unit unit, HexPoint start, HexPoint end, bool refresh = false)
        {
            // swap points to calculate the path backwards (from end to start)
            HexPoint temp = end;
            end = start;
            start = temp;

            // Check whether the requested path is already known
            PathRequest request = new PathRequest(unit,start, end);

            if (!refresh && knownPaths.ContainsKey(request))
            {
                return knownPaths[request].Copy();
            }

            // priority queue of nodes that yet have to be explored sorted in
            // ascending order by node costs (F)
            PriorityQueue<PathNode> open = new PriorityQueue<PathNode>();

            // list of nodes that have already been explored
            LinkedList<IGridCell> closed = new LinkedList<IGridCell>();

            // start is to be explored first
            PathNode startNode = new PathNode(unit,null, map[start], end);
            open.Enqueue(startNode);

            int steps = 0;
            PathNode current;

            do
            {
                // abort if calculation is too expensive
                if (++steps > stepLimit) return null;

                // examine the cheapest node among the yet to be explored
                current = open.Dequeue();

                // Finish?
                if (current.Cell.Matches(end))
                {
                    // paths which lead to the requested goal are cached for reuse
                    Path path = new Path(current);

                    if (knownPaths.ContainsKey(request))
                    {
                        knownPaths[request] = path.Copy();
                    }
                    else
                    {
                        knownPaths.Add(request, path.Copy());
                    }

                    return path;
                }

                // Explore all neighbours of the current cell
                ICollection<IGridCell> neighbours = map.GetNeighbourCells(current.Cell);

                foreach (IGridCell cell in neighbours)
                {
                    // discard nodes that are not of interest
                    if (closed.Contains(cell) || (cell.Matches(end) == false && !cell.IsWalkable(unit)))
                    {
                        continue;
                    }

                    // successor is one of current's neighbours
                    PathNode successor = new PathNode(unit,current, cell, end);
                    PathNode contained = open.Find(successor);

                    if (contained != null && successor.F >= contained.F)
                    {
                        // This cell is already in the open list represented by
                        // another node that is cheaper
                        continue;
                    }
                    else if (contained != null && successor.F < contained.F)
                    {
                        // This cell is already in the open list but on a more expensive
                        // path -> "integrate" the node into the current path
                        contained.Predecessor = current;
                        contained.Update();
                        open.Update(contained);
                    }
                    else
                    {
                        // The cell is not in the open list and therefore still has to
                        // be explored
                        open.Enqueue(successor);
                    }
                }

                // add current to the list of the already explored nodes
                closed.AddLast(current.Cell);

            } while (open.Peek() != null);

            return null;
        }
    }
}
