using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rovio
{
    class AStar
    {
        public LinkedList<Point> path;
        private List<Point> openList;
        private AStarData[,] map;
        double thresh = 0.75;


        public AStar() 
        {
        path = new LinkedList<Point>();
        openList = new List<Point>();
         map = new AStarData[260,300];
        }

        private int distanceToGoal(Point v1, Point v2)
        {
            int distX = Math.Abs(v1.X - v2.X);
            int distY = Math.Abs(v1.Y - v2.Y);
            return distX + distY;
        }

        public bool FindPath(Point origin, Point destination)
        {
            openList.Clear();

            for (int i = 0; i < 260; i++)
            {
                for (int j = 0; j < 300; j++)
                {
                    map[i, j].closed = false;
                    map[i, j].G = float.MaxValue;
                    map[i, j].H = float.MaxValue;
                    map[i, j].F = float.MaxValue;
                    map[i, j].Link = new Point(-1, -1);
                    map[i, j].inPath = false;
                }
            }

            map[origin.X, origin.Y].closed = false;
            map[origin.X, origin.Y].G = 0.0f;
            map[origin.X, origin.Y].H = distanceToGoal(origin, destination);
            map[origin.X, origin.Y].F = map[origin.X, origin.Y].H;
            map[origin.X, origin.Y].Link = origin;
            map[origin.X, origin.Y].inPath = true;

            openList.Add(origin);


            bool done = false;
            Point Location = origin;

            while (true)
            {
                float cost = float.MaxValue;

                foreach (Point loc in openList)
                {
                    if (!map[loc.X, loc.Y].closed && map[loc.X, loc.Y].F < cost)
                    {
                        Location = loc;
                        cost = map[loc.X, loc.Y].F;
                    }
                }

                if (cost == float.MaxValue)
                {
                    path.Clear();
                    return false;
                }
                else
                {
                    map[Location.X, Location.Y].closed = true;
                    openList.Remove(Location);

                    if (map[destination.X, destination.Y].closed)
                    {
                        path.Clear();
                        Point nextClosed = destination;
                        path.AddFirst(nextClosed);

                        while (!done)
                        {
                            map[nextClosed.X, nextClosed.Y].inPath = true;
                            nextClosed = map[nextClosed.X, nextClosed.Y].Link;
                            if (nextClosed == origin)
                            {
                                return true;
                            }
                            path.AddFirst(nextClosed);
                        }
                    }
                }

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {





                        if (!(j == 0 && i == 0))
                        {
                            Point checkLoc = new Point(Location.X + j, Location.Y + i);

                            if ((Mapping.get(checkLoc) < thresh) &&
                                !(map[checkLoc.X, checkLoc.Y].closed)
                                //&&(((i != 0 && j != 0)) ? Mapping.get(Location.X + j, Location.Y) < thresh && Mapping.get(Location.X, Location.Y + i) < thresh : true)
                                )
                            {
                                float newCost;

                                if (i != 0 && j != 0)
                                {
                                    newCost = map[Location.X, Location.Y].G + 1.4f;
                                }
                                else
                                {
                                    newCost = map[Location.X, Location.Y].G + 1.0f;
                                }

                                if (map[checkLoc.X, checkLoc.Y].G > newCost)
                                {
                                    map[checkLoc.X, checkLoc.Y].G = newCost;
                                    map[checkLoc.X, checkLoc.Y].H = distanceToGoal(checkLoc, destination);
                                    map[checkLoc.X, checkLoc.Y].F = newCost + map[checkLoc.X, checkLoc.Y].H;
                                    map[checkLoc.X, checkLoc.Y].Link = Location;
                                    openList.Add(checkLoc);
                                }
                            }
                        }
                    }
                }
            }
        }

        public struct AStarData
        {
            public bool closed;

            public float G;
            public float H;
            public float F;

            public Point Link;
            public bool inPath;
        }
    }
}
