using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    }
}