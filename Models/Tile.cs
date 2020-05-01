using System;
using UnityEngine;

namespace Models
{
    public class Tile
    {
        public int x;
        public int z;
        public int y;
        
        //TODO: get rid of 3x arrays, adjacency arrays in Tile class
        public bool[] AdjacencyArray = new bool[14]; 
        // adjacencyMap
        // [TopNorth, TopEast, TopSouth, TopWest, Top,
        //  North,    East,    South,    West,
        //  BotNorth, BotEast, BotSouth, BotWest, Bot]
        
        public byte[] CoverArray = new byte[4]; //byte to economize memory
        // [North,    East,    South,    West]
        // 0 for no Cover, 1 for low, 2 for high

        public bool traversable;
        public bool occupied;
        public GameObject CharacterOnTile;

        public bool IsIlluminated; 
        public Tile()
        {
            occupied = false;
            CharacterOnTile = null;
            IsIlluminated = true;
        }

        public Tile(int x, int z, int y)
        {
            this.x = x;
            this.z = z;
            this.y = y;
            occupied = false;
            CharacterOnTile = null;
            IsIlluminated = true;
            traversable = false;
        }

        /** ————————————————————————————————————————————————————————————————
         * Occupation status methods
         */

        /**
         * Initial occupation of tile
         */
        public void Occupy(GameObject model, int xOffset, int zOffset, Vector3 mapMetrics)
        {
            if (occupied)
            {
                //TODO: Occupy adjacent tile
                return;
            }

            try
            {
                
            }
            catch (MissingComponentException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Leave()
        {
            occupied = false;
            CharacterOnTile = null;
        }

        /**
         * Occupation evoked by movement
         */
        public void Occupy(GameObject model)
        {
            if (!occupied)
            {
                occupied = true;
                CharacterOnTile = model;
                
                //TODO: Object movement (method additionally requires map metrics and offset parameters), 
                //TODO: Requires move() method (with proper pathfinding algorithm)
            }
            else
            {
                Debug.Log("ТЫШО?! ТУПОЙ? НЕ ВИДИШЬ?! ЗАНЯТО!");
            }
        }

        public void Swap(Tile tile)
        {
        }
    }
}