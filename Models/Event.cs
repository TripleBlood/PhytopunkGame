using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public abstract class Event
    {
        public GameObject caster;
        public List<GameObject> target;
        public List<Tile> tiles;
            
        /// <summary>
        /// Describes event behavior
        /// </summary>
        /// <returns>Coroutine command sequence</returns>
        public abstract List<IEnumerator> Behavior();

    }
}