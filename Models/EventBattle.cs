using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Models
{
    public abstract class EventBattle : MonoBehaviour
    {
        // TODO: Revise. May be I don't need this. (Most likely I don't)
        // public GameObject caster;
        // public List<GameObject> target;
        // public List<Tile> tiles;

        public BattleManager battleManager;
        public bool active;

        // Don't need this, because Event initiated in targeting controller
        // public abstract void Initiate();

        public abstract void EndEvent(out bool busy);
            
        /// <summary>
        /// Describes event behavior
        /// </summary>
        /// <returns>Coroutine command sequence</returns>
        //public abstract IEnumerator Behavior(out bool busy);

        public List<IEnumerator> listOfCoroutines;

    }
}