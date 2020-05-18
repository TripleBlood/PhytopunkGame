using System;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Serialization;
using Priority_Queue;

namespace DefaultNamespace.Utils
{
    public class MapUtils
    {
        public static int[] GetTileIndex(Vector3 inputVector, float tileSize)
        {
            int[] result = new int[2];
            if (inputVector.x >= 0.0f)
            {
                result[0] = Convert.ToInt32(Math.Truncate(inputVector.x / tileSize));
            }
            else
            {
                result[0] = Convert.ToInt32(Math.Truncate(inputVector.x / tileSize)) - 1;
            }

            if (inputVector.z >= 0.0f)
            {
                result[1] = Convert.ToInt32(Math.Truncate(inputVector.z / tileSize));
            }
            else
            {
                result[1] = Convert.ToInt32(Math.Truncate(inputVector.z / tileSize)) - 1;
            }

            return result;
        }

        /// <summary>
        /// Returns hash for adjacency/cover hashMaps, has specific arrangement (prioritizing y coordinate)
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <returns></returns>
        public static string FormMapHash(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            string hash = "";
            string str1 = "" + x1 + '#' + z1 + '#' + y1;
            string str2 = "" + x2 + '#' + z2 + '#' + y2;

            int sum1 = x1 + y1 + z1;
            int sum2 = x2 + y2 + z2;
            if (sum1 < sum2)
            {
                hash = str1 + '#' + str2;
            }
            else if (sum1 == sum2)
            {
                if (y1 != y2)
                {
                    if (y1 < y2)
                    {
                        hash = str1 + '#' + str2;
                    }
                    else
                    {
                        hash = str2 + "#" + str1;
                    }
                }
                else
                {
                    if (x1 < x2)
                    {
                        hash = str1 + '#' + str2;
                    }
                    else
                    {
                        hash = str2 + "#" + str1;
                    }
                }
            }
            else
            {
                hash = str2 + "#" + str1;
            }

            return hash;
        }

        public static string GetTileHash(int x, int z, int y)
        {
            string tileHash = "" + x + "#" + z + "#" + y;
            return tileHash;
        }

        /// <summary>
        /// Retrieve List of adjacent Tiles (occupation check not included)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static List<Tile> GetAdjTilesNoOccCheck(Map map, int x, int z, int y)
        {
            List<Tile> result = new List<Tile>();

            string basepointHash = MapUtils.GetTileHash(x, z, y);
            Tile basePointTile;
            if (map.mapTileDict.TryGetValue(basepointHash, out basePointTile))
            {
                for (int i = 0; i < 14; i++)
                {
                    int[] indecies2 = map.GetTileIndexesByAdjIndex(i, x, z, y);
                    string adjPointHash = GetTileHash(indecies2[0], indecies2[1], indecies2[2]);

                    string adjDictHash = MapUtils.FormMapHash(x, y, z,
                        indecies2[0], indecies2[2], indecies2[1]);

                    Tile adjPoint;
                    bool hit = false;

                    if (map.mapTileDict.TryGetValue(adjPointHash, out adjPoint))
                    {
                        if (map.mapAdjDict.TryGetValue(adjDictHash, out hit))
                        {
                            result.Add(adjPoint);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieve List of passable adjacent Tiles (occupation check included)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static List<Tile> GetAdjTilesWithOccCheck(Map map, int x, int z, int y)
        {
            List<Tile> result = new List<Tile>();

            string basepointHash = MapUtils.GetTileHash(x, z, y);
            Tile basePointTile;
            if (map.mapTileDict.TryGetValue(basepointHash, out basePointTile))
            {
                for (int i = 0; i < 14; i++)
                {
                    int[] indecies2 = map.GetTileIndexesByAdjIndex(i, x, z, y);
                    string adjPointHash = GetTileHash(indecies2[0], indecies2[1], indecies2[2]);

                    string adjDictHash = MapUtils.FormMapHash(x, y, z,
                        indecies2[0], indecies2[2], indecies2[1]);

                    Tile adjPoint;
                    bool hit = false;

                    if (map.mapTileDict.TryGetValue(adjPointHash, out adjPoint))
                    {
                        if (map.mapAdjDict.TryGetValue(adjDictHash, out hit))
                        {
                            // Check if something is standing on tile
                            if (!adjPoint.occupied)
                            {
                                result.Add(adjPoint);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static List<Tile> FindPath(Map map, Tile start, Tile goal)
        {
            List<Tile> path = new List<Tile>();
            SimplePriorityQueue<Tile> frontier = new SimplePriorityQueue<Tile>();

            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();

            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            Tile currentTile = new Tile();

            while (frontier.Count > 0)
            {
                currentTile = frontier.Dequeue();

                if (currentTile.Equals(goal))
                {
                    break;
                }

                foreach (Tile next in MapUtils.GetAdjTilesWithOccCheck(map, currentTile.x, currentTile.z, currentTile.y))
                {
                    int newCost;

                    newCost = costSoFar[currentTile] + 1;
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        int priority = newCost + ManhattanDistance(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = currentTile;
                    }
                    // if (costSoFar.TryGetValue(currentTile, out costSoFarCurrent))
                    // {
                    //     newCost = costSoFarCurrent + 1; // +1 because cost to move between all tiles is 1
                    //     if (true) //change!
                    //     {
                    //         costSoFar.Add(next, newCost);
                    //         int priority = newCost + ManhattanDistance(next, goal);
                    //         frontier.Enqueue(next, priority);
                    //         cameFrom.Add(next, currentTile);
                    //     }
                    // }

                    // int newCost = 
                }
            }

            while (!currentTile.Equals(start))
            {
                path.Add(currentTile);
                currentTile = cameFrom[currentTile];
            }
            path.Add(start);
            path.Reverse();
            
            path.RemoveAt(0);
            
            return path;
        }

        public static int ManhattanDistance(Tile start, Tile finish)
        {
            return Math.Abs(start.x - finish.x) + Math.Abs(start.y - finish.y) + Math.Abs(start.z + finish.z);
        }
        
        
    }
}