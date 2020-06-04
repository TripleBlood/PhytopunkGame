using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;

namespace DefaultNamespace.Utils
{
    public class TargetingUtils
    {
        public static int GetAdjIndexByAngle(float angle)
        {
            if (Math.Abs(angle) < 45)
            {
                return 2;
            }
            else if (Math.Abs(angle) > 135)
            {
                return 0;
            }
            else if (angle > 0)
            {
                return 3;
            }
            else
            {
                return 1;
            }
        }

        public static readonly Vector3[][] pointsAdd = new Vector3[][]
        {
            new Vector3[3]
            {
                new Vector3(0, 1.5f, 0.35f),
                new Vector3(0.65f, 1, 0.35f),
                new Vector3(-0.65f, 1, 0.35f)
            },
            new Vector3[3]
            {
                new Vector3(0.35f, 1.5f, 0),
                new Vector3(0.35f, 1, 0.65f),
                new Vector3(0.35f, 1, -0.65f)
            },
            new Vector3[3]
            {
                new Vector3(0, 1.5f, -0.35f),
                new Vector3(0.65f, 1, -0.35f),
                new Vector3(-0.65f, 1, -0.35f)
            },
            new Vector3[3]
            {
                new Vector3(-0.35f, 1.5f, 0),
                new Vector3(-0.35f, 1, 0.65f),
                new Vector3(-0.35f, 1, -0.65f)
            }
        };

        public static List<Vector3> GetPointsOrigin(Map map, Tile tile, int index)
        {
            List<Vector3> result = new List<Vector3>();
            if (index > 3 || index < 0)
            {
                return new List<Vector3>();
            }

            int cover = tile.CoverArray[index];
            if (cover == 1)
            {
                result = pointsAdd[index].ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    result[i] += map.GetCoordByTileIndexes(tile.x, tile.z, tile.y);
                }

                return result;
            }

            if (cover == 2)
            {
                result = pointsAdd[index].ToList().GetRange(1, 2);

                for (int i = 0; i < result.Count; i++)
                {
                    result[i] += map.GetCoordByTileIndexes(tile.x, tile.z, tile.y);
                }

                return result;
            }

            result = pointsAdd[index].ToList();

            // for (int i = 0; i < result.Count; i++)
            // {
            //     result[i] += map.GetCoordByTileIndexes(tile.x, tile.z, tile.y);
            // }
            //
            // return result.GetRange(0, 1);

            return new List<Vector3>
            {
                map.GetCoordByTileIndexes(tile.x, tile.z, tile.y) + new Vector3(0, 1.5f, 0)
            };
        }
    }
}