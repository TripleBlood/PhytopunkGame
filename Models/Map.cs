using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Utils;
using UnityEngine;
using static Models.Tile;

namespace Models
{
    public class Map
    {
        public bool testField = true;

        public float bottomFloorLevel;

        public Tile[,,] tiles;
        public Tile[,,] distanceList;

        public Dictionary<string, Tile> mapTileDict;
        public Dictionary<string, bool> mapAdjDict;
        public Dictionary<string, int> mapCoverDict;

        /// <summary>
        /// X-component of map
        /// </summary>
        public int width; // x

        /// <summary>
        /// Z-component of map
        /// </summary>
        public int length; // z

        /// <summary>
        /// Y-component of map
        /// </summary>
        public int floorCount; // y

        private float xOffset { get; set; }
        private float zOffset { get; set; }
        private float yOffset { get; set; } //shows the lowest level coord

        // For Adjacency and low covers check
        public readonly Vector3[] checkVectorsArrLowCover = new Vector3[4]
        {
            new Vector3(0, -1.5f, 0.55f),
            new Vector3(0.55f, -1.5f, 0),
            new Vector3(0, -1.5f, -0.55f),
            new Vector3(-0.55f, -1.5f, 0)
        };

        public readonly Vector3 upVect = new Vector3(0, 2.0f, 0);

        // For Adjacency and high covers check
        public readonly Vector3[] checkVectorsArrHighCover = new Vector3[4]
        {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left
        };


        /// <summary>
        /// Map Initialization
        /// </summary>
        /// <param name="floorCount">Count of floors in this map</param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        public Map(int floorCount, int width, int length, float xOffset, float zOffset, float yOffset)
        {
            this.width = width;
            this.length = length;
            this.floorCount = floorCount;
            this.xOffset = xOffset;
            this.zOffset = zOffset;
            this.yOffset = yOffset;

            this.testField = true;

            // get rid of 3x arrays, adjacency arrays in Tile class
            tiles = new Tile[width, length, floorCount];
            distanceList = new Tile[width, length, floorCount];

            mapTileDict = new Dictionary<string, Tile>();
            mapAdjDict = new Dictionary<string, bool>();
            mapCoverDict = new Dictionary<string, int>();

            int mask = 1 << 9;
            mask = ~mask;

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    for (int k = 0; k < tiles.GetLength(2); k++)
                    {
                        mapTileDict.Add(MapUtils.GetTileHash(i, j, k), new Tile(i, j, k));
                        tiles[i, j, k] = new Tile(i, j, k);
                    }
                }
            }

            //Initializing map with dictionary
            foreach (KeyValuePair<string, Tile> tile in mapTileDict)
            {
                int xbuffer = tile.Value.x;
                int zbuffer = tile.Value.z;
                int ybuffer = tile.Value.y;

                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(GetOriginPointForTopDownVect(xbuffer, zbuffer, ybuffer), Vector3.down,
                    out hitInfo, 3, mask);
                if (hit && hitInfo.transform.gameObject.tag.Equals("Floor"))
                {
                    // Debug.DrawLine(GetOriginPointForTopDownVect(xbuffer, zbuffer, ybuffer), hitInfo.point, Color.green,
                    //     2);
                    //Debug.Log(hitInfo.point);
                    GetTileIndexesByCoords(hitInfo.point);
                    if ((hitInfo.transform.gameObject.tag.Equals("Floor")) &&
                        GetTileIndexesByCoords(hitInfo.point).SequenceEqual(new int[] {xbuffer, zbuffer, ybuffer})
                    )
                    {
                        tile.Value.traversable = true;
                    }
                }
            }

            foreach (KeyValuePair<string, Tile> tile in mapTileDict)
            {
                int xbuffer = tile.Value.x;
                int zbuffer = tile.Value.z;
                int ybuffer = tile.Value.y;

                Tile actualTile = tile.Value;

                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(GetOriginPointForTopDownVect(xbuffer, zbuffer, ybuffer), Vector3.down,
                    out hitInfo, 3, mask);
                if (hit)
                {
                    if (hitInfo.transform.gameObject.tag.Equals("Floor") &&
                        GetTileIndexesByCoords(hitInfo.point).SequenceEqual(new int[] {xbuffer, zbuffer, ybuffer})
                    )
                    {
                        Vector3 pointForAdj = hitInfo.point + upVect;

                        // hit = Physics.Raycast(pointForAdj, new Vector3(0, -1.3f, 0.6f),out hitInfo);

                        for (int l = 0; l < 4; l++)
                        {
                            // 
                            RaycastHit adjHitInfo;
                            bool adjHit = Physics.Raycast(pointForAdj, checkVectorsArrLowCover[l],
                                out adjHitInfo, 2.15f, mask);

                            // Debug.DrawLine(pointForAdj, (pointForAdj + checkVectorsArrLowCover[l]* 2.1f) ,
                            //     Color.magenta, 25);
                            if (adjHit)
                            {
                                int[] hitPointCoord =
                                    GetTileIndexesByCoords(adjHitInfo.point); //Where raycast hit

                                int[] adjPointCoord =
                                    GetTileIndexesByAdjIndex(l + 5, xbuffer, zbuffer,
                                        ybuffer); //Where raycast "supposed" to hit

                                if (adjHitInfo.transform.gameObject.tag.Equals("Ladder") &&
                                    hitPointCoord.SequenceEqual(new int[] {xbuffer, zbuffer, ybuffer})
                                ) // If hit ladder withing tile
                                {
                                    //TODO: check uses
                                    int[] ladderTile = GetTileIndexesByAdjIndex(l, xbuffer, zbuffer, ybuffer);
                                    if (!ladderTile.SequenceEqual(new int[] {-1, -1, -1}))
                                    {
                                        mapAdjDict.Add(MapUtils.FormMapHash(xbuffer, ybuffer, zbuffer,
                                            ladderTile[0], ladderTile[2], ladderTile[1]), true);
                                        //tiles[ladderTile[0], ladderTile[1], ladderTile[2]].AdjacencyArray[((l + 2) % 4) + 9] = true;
                                    }
                                }
                                else if (adjHitInfo.transform.gameObject.tag.Equals("Floor") &&
                                         hitPointCoord[0] == adjPointCoord[0] &&
                                         hitPointCoord[1] == adjPointCoord[1] &&
                                         Math.Abs(hitPointCoord[2] - adjPointCoord[2]) < 2
                                    // Same floor or 1 floor higher/lower
                                ) // Not sure whether I check all possible clauses...
                                {
                                    // if (hitPointCoord[2] > adjPointCoord[2]) //1 floor higher
                                    // {
                                    //     tiles[i, j, k].AdjacencyArray[l] = true;
                                    // }
                                    // else if (hitPointCoord[2] == adjPointCoord[2]) //same floor
                                    // {
                                    //     tiles[i, j, k].AdjacencyArray[l + 5] = true;
                                    // }
                                    // else //floor lower
                                    // {
                                    //     tiles[i, j, k].AdjacencyArray[l + 9] = true;
                                    // }
                                    if (hitPointCoord.SequenceEqual(new int[] {-1, -1, -1}) ||
                                        !mapAdjDict.ContainsKey(MapUtils.FormMapHash(hitPointCoord[0], hitPointCoord[2],
                                            hitPointCoord[1],
                                            xbuffer, ybuffer, zbuffer)))
                                    {
                                        mapAdjDict.Add(MapUtils.FormMapHash(hitPointCoord[0], hitPointCoord[2],
                                            hitPointCoord[1],
                                            xbuffer, ybuffer, zbuffer), true);
                                    }
                                }
                                else if (Math.Abs(hitPointCoord[0] - adjPointCoord[0]) < 2 &&
                                         Math.Abs(hitPointCoord[1] - adjPointCoord[1]) < 2 &&
                                         Math.Abs(hitPointCoord[2] - adjPointCoord[2]) < 2
                                ) //any other cases should be covers...
                                {
                                    //Check floor //TODO: FLOOR DIFFERENCE IS WORKING WRONG!
                                    int floorDifference =
                                        hitPointCoord[2] - ybuffer; // -1 lower, 0 same, +1 higher
                                    int[] probablyAdjTile = new int[]
                                        {adjPointCoord[0], adjPointCoord[1], floorDifference};
                                    int[] buffer;

                                    RaycastHit highHitInfo;
                                    bool hitHigh = false;

                                    hitHigh = Physics.Raycast(pointForAdj, checkVectorsArrHighCover[l],
                                        out highHitInfo, 1, mask);
                                    if (hitHigh)
                                    {
                                        buffer = GetTileIndexesByAdjIndex(l + 5, xbuffer, zbuffer, ybuffer);
                                        if (!buffer.SequenceEqual(new int[] {-1, -1, -1}))
                                        {
                                            if (!mapCoverDict.ContainsKey(MapUtils.FormMapHash(buffer[0], buffer[2],
                                                buffer[1], xbuffer, ybuffer, zbuffer)))
                                            {
                                                mapCoverDict.Add(MapUtils.FormMapHash(buffer[0], buffer[2], buffer[1],
                                                    xbuffer, ybuffer, zbuffer), 2);
                                            }

                                            Debug.DrawLine(pointForAdj, highHitInfo.point, Color.green, 25);

                                            actualTile.CoverArray[l] = 2;
                                        }
                                    }
                                    else
                                    {
                                        //Debug.DrawLine(pointForAdj, pointForAdj + checkVectorsArrHighCover[l], Color.green, 25);

                                        if (adjPointCoord[2] + floorDifference > -1 &&
                                            adjPointCoord[2] + floorDifference < floorCount &&
                                            tiles[adjPointCoord[0], adjPointCoord[1],
                                                adjPointCoord[2] + floorDifference].traversable)
                                        {
                                            if (floorDifference >= 0)
                                            {
                                                buffer = GetTileIndexesByAdjIndex(5 + l - (5 * floorDifference),
                                                    xbuffer, zbuffer, ybuffer);
                                                if (!buffer.SequenceEqual(new int[] {-1, -1, -1}) &&
                                                    !mapAdjDict.ContainsKey(MapUtils.FormMapHash(buffer[0], buffer[2],
                                                        buffer[1],
                                                        xbuffer, ybuffer, zbuffer)))
                                                {
                                                    mapAdjDict.Add(MapUtils.FormMapHash(buffer[0], buffer[2], buffer[1],
                                                        xbuffer, ybuffer, zbuffer), true);
                                                }

                                                // tiles[i, j, k].AdjacencyArray[5 + l - (5 * floorDifference)] = true;
                                            }
                                            else if (floorDifference == -1)
                                            {
                                                buffer = GetTileIndexesByAdjIndex(l + 9, xbuffer, zbuffer, ybuffer);
                                                if (!buffer.SequenceEqual(new int[] {-1, -1, -1}) &&
                                                    !mapAdjDict.ContainsKey(MapUtils.FormMapHash(buffer[0], buffer[2],
                                                        buffer[1],
                                                        xbuffer, ybuffer, zbuffer)))
                                                {
                                                    mapAdjDict.Add(MapUtils.FormMapHash(buffer[0], buffer[2], buffer[1],
                                                        xbuffer, ybuffer, zbuffer), true);
                                                }

                                                // tiles[i, j, k].AdjacencyArray[l + 9] = true;
                                            }
                                        }

                                        buffer = GetTileIndexesByAdjIndex(l, xbuffer, zbuffer, ybuffer);
                                        if (!buffer.SequenceEqual(new int[] {-1, -1, -1}))
                                        {
                                            if (!mapCoverDict.ContainsKey(MapUtils.FormMapHash(buffer[0], buffer[2],
                                                buffer[1], xbuffer, ybuffer, zbuffer)))
                                            {
                                                mapCoverDict.Add(MapUtils.FormMapHash(buffer[0], buffer[2], buffer[1],
                                                    xbuffer, ybuffer, zbuffer), 1);
                                            }

                                            actualTile.CoverArray[l] = 1;
                                        }

                                        // tiles[i, j, k].CoverArray[l] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Debug.Log(tiles.GetLength(0) + ", " + tiles.GetLength(1) + ", " + tiles.GetLength(2));

            // for (int i = 0; i < tiles.GetLength(0); i++)
            // {
            //     for (int j = 0; j < tiles.GetLength(1); j++)
            //     {
            //         for (int k = 0; k < tiles.GetLength(2); k++)
            //         {
            //             RaycastHit hitInfo = new RaycastHit();
            //             bool hit = Physics.Raycast(GetOriginPointForTopDownVect(i, j, k), Vector3.down, out hitInfo, 3,
            //                 mask);
            //             if (hit)
            //             {
            //                 //Console.WriteLine(hitInfo.point);
            //                 if (k == 1)
            //                 {
            //                     //Debug.DrawLine(GetOriginPointForTopDownVect(i, j, k), hitInfo.point, Color.green, 2);
            //                 }
            //
            //                 //Debug.DrawLine(GetOriginPointForTopDownVect(i, j, k), hitInfo.point, Color.green, 2);
            //                 //Debug.Log(hitInfo.point);
            //                 GetTileIndexesByCoords(hitInfo.point);
            //                 if ((hitInfo.transform.gameObject.tag.Equals("Floor")) &&
            //                     GetTileIndexesByCoords(hitInfo.point).SequenceEqual(new int[] {i, j, k})
            //                 )
            //                 {
            //                     tiles[i, j, k].traversable = true;
            //                 }
            //             }
            //         }
            //     }
            // }
            //
            // // This cycle checks adjacency and covers
            // for (int i = 0; i < tiles.GetLength(0); i++)
            // {
            //     for (int j = 0; j < tiles.GetLength(1); j++)
            //     {
            //         for (int k = 0; k < tiles.GetLength(2); k++)
            //         {
            //             RaycastHit hitInfo = new RaycastHit();
            //             bool hit = Physics.Raycast(GetOriginPointForTopDownVect(i, j, k), Vector3.down, out hitInfo, 3,
            //                 mask);
            //             if (hit)
            //             {
            //                 if (hitInfo.transform.gameObject.tag.Equals("Floor") &&
            //                     GetTileIndexesByCoords(hitInfo.point).SequenceEqual(new int[] {i, j, k})
            //                 )
            //                 {
            //                     Vector3 pointForAdj = hitInfo.point + upVect;
            //
            //                     // hit = Physics.Raycast(pointForAdj, new Vector3(0, -1.3f, 0.6f),out hitInfo);
            //
            //                     for (int l = 0; l < 4; l++)
            //                     {
            //                         // 
            //                         RaycastHit adjHitInfo;
            //                         bool adjHit = Physics.Raycast(pointForAdj, checkVectorsArrLowCover[l],
            //                             out adjHitInfo, 3.65f, mask);
            //                         if (adjHit)
            //                         {
            //                             int[] hitPointCoord =
            //                                 GetTileIndexesByCoords(adjHitInfo.point); //Where raycast hit
            //                             int[] adjPointCoord =
            //                                 GetTileIndexesByAdjIndex(l + 5, i, j,
            //                                     k); //Where raycast "supposed" to hit
            //
            //                             if (adjHitInfo.transform.gameObject.tag.Equals("Ladder") &&
            //                                 hitPointCoord.SequenceEqual(new int[] {i, j, k})
            //                             ) // If hit ladder withing tile
            //                             {
            //                                 tiles[i, j, k].AdjacencyArray[l] = true;
            //                                 //tiles[i, j, k].AdjacencyArray[4] = true;
            //                                 //TODO: check uses
            //                                 int[] ladderTile = GetTileIndexesByAdjIndex(l, i, j, k);
            //                                 if (!ladderTile.SequenceEqual(new int[] {-1, -1, -1}))
            //                                 {
            //                                     tiles[ladderTile[0], ladderTile[1], ladderTile[2]]
            //                                         .AdjacencyArray[((l + 2) % 4) + 9] = true;
            //                                 }
            //                             }
            //                             else if (adjHitInfo.transform.gameObject.tag.Equals("Floor") &&
            //                                      hitPointCoord[0] == adjPointCoord[0] &&
            //                                      hitPointCoord[1] == adjPointCoord[1] &&
            //                                      Math.Abs(hitPointCoord[2] - adjPointCoord[2]) < 2
            //                                 // Same floor or 1 floor higher/lower
            //                             ) // Not sure whether I check all possible clauses...
            //                             {
            //                                 if (hitPointCoord[2] > adjPointCoord[2]) //1 floor higher
            //                                 {
            //                                     tiles[i, j, k].AdjacencyArray[l] = true;
            //                                 }
            //                                 else if (hitPointCoord[2] == adjPointCoord[2]) //same floor
            //                                 {
            //                                     tiles[i, j, k].AdjacencyArray[l + 5] = true;
            //                                 }
            //                                 else //floor lower
            //                                 {
            //                                     tiles[i, j, k].AdjacencyArray[l + 9] = true;
            //                                 }
            //                             }
            //                             else if (Math.Abs(hitPointCoord[0] - adjPointCoord[0]) < 2 &&
            //                                      Math.Abs(hitPointCoord[1] - adjPointCoord[1]) < 2 &&
            //                                      Math.Abs(hitPointCoord[2] - adjPointCoord[2]) < 2
            //                             ) //any other cases should be covers...
            //                             {
            //                                 //Check floor //TODO: FLOOR DIFFERENCE IS WORKING WRONG!
            //                                 int floorDifference =
            //                                     hitPointCoord[2] - k; // -1 lower, 0 same, +1 higher
            //                                 int[] probablyAdjTile = new int[]
            //                                     {adjPointCoord[0], adjPointCoord[1], floorDifference};
            //
            //                                 RaycastHit highHitInfo;
            //                                 bool hitHigh = false;
            //
            //                                 hitHigh = Physics.Raycast(pointForAdj, checkVectorsArrHighCover[l],
            //                                     out highHitInfo, 1, mask);
            //                                 
            //                                 if (hitHigh)
            //                                 {
            //                                     tiles[i, j, k].CoverArray[l] = 2;
            //                                     //Debug.DrawLine(pointForAdj, pointForAdj + checkVectorsArrHighCover[l], Color.green, 25 );
            //                                 }
            //                                 else
            //                                 {
            //                                     if (adjPointCoord[2] + floorDifference > -1 &&
            //                                         adjPointCoord[2] + floorDifference < floorCount &&
            //                                         tiles[adjPointCoord[0], adjPointCoord[1],
            //                                             adjPointCoord[2] + floorDifference].traversable)
            //                                     {
            //                                         if (floorDifference >= 0)
            //                                         {
            //                                             tiles[i, j, k].AdjacencyArray[5 + l - (5 * floorDifference)] =
            //                                                 true;
            //                                         }
            //                                         else if (floorDifference == -1)
            //                                         {
            //                                             tiles[i, j, k].AdjacencyArray[l + 9] = true;
            //                                         }
            //                                     }
            //
            //                                     tiles[i, j, k].CoverArray[l] = 1;
            //                                 }
            //                             }
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }
            // }
        }

        /// <summary>
        /// Gets tile 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile FindTile(int x, int z, int y)
        {
            string str = MapUtils.GetTileHash(x, z, y);
            Tile tile = new Tile();
            if (mapTileDict.TryGetValue(str, out tile))
            {
            }

            return tile;
        }

        /// <summary>
        /// Returns vector of current map indexes where initial point is located 
        /// </summary>
        /// <param name="convertiblePoint"></param>
        /// <returns></returns>
        public int[] GetTileIndexesByCoords(Vector3 convertiblePoint)
        {
            int[] result = new int[]
            {
                Convert.ToInt32(Math.Round(convertiblePoint.x - xOffset)),
                Convert.ToInt32(Math.Round(convertiblePoint.z - zOffset)),
                Convert.ToInt32(Math.Truncate(convertiblePoint.y - yOffset)) / 3 // 3 is floor height
            };
            return result;
        }

        /// <summary>
        /// Get coordinates in space of tile by Tile indecies
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns> Coordinates in space as Vector3 </returns>
        public Vector3 GetCoordByTileIndexes(int x, int z, int y)
        {
            int mask = 1 << 9;
            mask = ~mask;

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(GetOriginPointForTopDownVect(x, z, y), Vector3.down,
                out hitInfo, 3, mask);

            if (hit && hitInfo.transform.gameObject.tag.Equals("Floor"))
            {
                Vector3 vect = hitInfo.point;
                return vect;
            }

            return new Vector3(xOffset + x, y * 3, zOffset + z);
        }

        /// <summary>
        /// Get coordinates in space of tile by Tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns> Coordinates in space as Vector3 </returns>
        public Vector3 GetCoordByTile(Tile tile)
        {
            int x = tile.x;
            int y = tile.y;
            int z = tile.z;

            int mask = 1 << 9;
            mask = ~mask;

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(GetOriginPointForTopDownVect(x, z, y), Vector3.down,
                out hitInfo, 3, mask);

            if (hit && hitInfo.transform.gameObject.tag.Equals("Floor"))
            {
                Vector3 vect = hitInfo.point;
                return vect;
            }

            return new Vector3(xOffset + x, y * 3, zOffset + z);
        }

        /// <summary>
        /// Get coordinates in space for tile where inputVector is located 
        /// </summary>
        /// <param name="inputVector"></param>
        /// <returns></returns>
        public Vector3 GetCourserPosition(Vector3 inputVector)
        {
            int[] point = GetTileIndexesByCoords(inputVector);
            return GetCoordByTileIndexes(point[0], point[1], point[2]);
        }

        /// <summary>
        /// Gets Tile by point in space
        /// </summary>
        /// <param name="inputVector"></param>
        /// <returns></returns>
        public Tile GetTileByVectorPoint(Vector3 inputVector)
        {
            int[] point = GetTileIndexesByCoords(inputVector);
            string strHash = MapUtils.GetTileHash(point[0], point[1], point[2]);
            return FindTile(point[0], point[1], point[2]);
        }

        /// <summary>
        /// Input GameObject occupies tile
        /// </summary>
        /// <param name="occupier"></param>
        /// <param name="tile"></param>
        /// <returns>Returns bool if occupation is successful.</returns>
        public bool OccupyTile(GameObject occupier, Tile tile)
        {
            if (!tile.occupied)
            {
                try
                {
                    // Better to do this another way
                    occupier.GetComponent<CharacterDataComponent>().position = tile;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                tile.characterOnTile = occupier;
                tile.occupied = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Input GameObject leaves tile. Additional check whether input gameObject leaves tile... Don't know why I've wrote it... 
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool LeaveTile(Tile tile, GameObject gameObject)
        {
            if (gameObject == tile.characterOnTile)
            {
                tile.characterOnTile = null;
                tile.occupied = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns vector required for initial map tiles' traversability check 
        /// </summary>
        /// <param name="x">X component of the vector</param>
        /// <param name="z">Z component of the vector</param>
        /// <param name="y">Y component of the vector</param>
        /// <returns></returns>
        public Vector3 GetOriginPointForTopDownVect(int x, int z, int y)
        {
            return new Vector3(x + xOffset, 3 * y + yOffset + 2.7f, z + zOffset);
        }

        /// <summary>
        /// Returns vector of current map indexes of adjacent tile (x, z, y). Returns (-1, -1, -1) if exceeds bounds (width, length, floorCount)
        /// TODO: Rewrite. This part has been wrote in retarded way for whatever fucking reason...
        /// </summary>
        /// <param name="adjIndex">Adjacency Index
        /// TopNorth, TopEast, TopSouth, TopWest, Top
        /// North,    East,    South,    West,
        /// BotNorth, BotEast, BotSouth, BotWest, Bot</param>
        /// <param name="x">X component of the tile which neighbor we are looking for</param>
        /// <param name="z">Z component of the tile which neighbor we are looking for</param>
        /// <param name="y">Y component of the tile which neighbor we are looking for</param>
        /// <returns></returns>
        public int[] GetTileIndexesByAdjIndex(int adjIndex, int x, int z, int y)
        {
            int[] result = new int[3] {-1, -1, -1};

            switch (adjIndex)
            {
                //TopNorth
                case 0:
                    result = new int[3] {x, z + 1, y + 1};
                    break;
                //TopEast
                case 1:
                    result = new int[3] {x + 1, z, y + 1};
                    break;
                //TopSouth
                case 2:
                    result = new int[3] {x, z - 1, y + 1};
                    break;
                //TopWest
                case 3:
                    result = new int[3] {x - 1, z, y + 1};
                    break;
                //Top
                case 4:
                    result = new int[3] {x, z, y + 1};
                    break;
                //North
                case 5:
                    result = new int[3] {x, z + 1, y};
                    break;
                //East
                case 6:
                    result = new int[3] {x + 1, z, y};
                    break;
                //South
                case 7:
                    result = new int[3] {x, z - 1, y};
                    break;
                //West
                case 8:
                    result = new int[3] {x - 1, z, y};
                    break;
                //BotNorth
                case 9:
                    result = new int[3] {x, z + 1, y - 1};
                    break;
                //BotEast
                case 10:
                    result = new int[3] {x + 1, z, y - 1};
                    break;
                //BotSouth
                case 11:
                    result = new int[3] {x, z - 1, y - 1};
                    break;
                //BotWest
                case 12:
                    result = new int[3] {x - 1, z, y - 1};
                    break;
                //Bot
                case 13:
                    result = new int[3] {x, z, y - 1};
                    break;
            }

            if (result[0] < 0 || result[0] >= width ||
                result[1] < 0 || result[1] >= length ||
                result[2] < 0 || result[2] >= floorCount)
            {
                result[0] = -1;
                result[1] = -1;
                result[2] = -1;
            }

            return result;
        }

        public List<Vector3> GetPointsForIlluminationCheck(Tile tile)
        {
            List<Vector3> result = new List<Vector3>();

            result.Add(GetCoordByTile(tile) + new Vector3(0, 0.2f, 0));
            result.Add(GetCoordByTile(tile) + new Vector3(0, 1.7f, 0));

            return result;
        }

        public void UpdateIllumination(List<GameObject> lightSources)
        {
            bool test = true;
            foreach (KeyValuePair<string, Tile> keyValuePair in mapTileDict)
            {
                Tile tile = keyValuePair.Value;

                int illuminationBuffer = 0;
                int sourcesRequired = 2;

                foreach (Vector3 pointForCheck in GetPointsForIlluminationCheck(tile))
                {
                    foreach (GameObject lightSource in lightSources)
                    {
                        Light lightComponent = lightSource.GetComponent<Light>();

                        Vector3 tileToLightSource = lightSource.transform.position - pointForCheck;

                        if (lightComponent.type == LightType.Directional)
                        {
                            RaycastHit hitInfoLight = new RaycastHit();
                            bool hitLight = Physics.Raycast(pointForCheck, -lightSource.transform.forward,
                                out hitInfoLight, Single.PositiveInfinity);

                            if (!hitLight)
                            {
                                illuminationBuffer += 1;
                            }
                        }
                        else if (lightComponent.type == LightType.Spot)
                        {
                            if (tileToLightSource.magnitude <= lightComponent.range)
                            {
                                if (Math.Abs(Vector3.SignedAngle(lightSource.transform.forward, -tileToLightSource,
                                        Vector3.up)) <= lightComponent.spotAngle / 2 &&
                                    Math.Abs(Vector3.SignedAngle(lightSource.transform.forward, -tileToLightSource,
                                        Vector3.forward)) <= lightComponent.spotAngle / 2 &&
                                    Math.Abs(Vector3.SignedAngle(lightSource.transform.forward, -tileToLightSource,
                                        Vector3.right)) <= lightComponent.spotAngle / 2)
                                {
                                    RaycastHit hitInfoLight = new RaycastHit();
                                    bool hitLight = Physics.Raycast(pointForCheck, tileToLightSource,
                                        out hitInfoLight, tileToLightSource.magnitude);
                                    if (!hitLight)
                                    {
                                        illuminationBuffer += 1;

                                        Debug.DrawLine(pointForCheck, pointForCheck + Vector3.up, Color.white, 5);
                                        Debug.DrawLine(pointForCheck, pointForCheck + tileToLightSource,
                                            Color.green, 5);
                                    }
                                }
                            }
                        }
                        else if (lightComponent.type == LightType.Rectangle)
                        {
                        }
                        else if (lightComponent.type == LightType.Point)
                        {
                        }
                    }

                    if (illuminationBuffer >= sourcesRequired)
                    {
                        tile.IsIlluminated = true;
                    }
                    else
                    {
                        tile.IsIlluminated = false;
                    }
                }
            }
        }
    }
}