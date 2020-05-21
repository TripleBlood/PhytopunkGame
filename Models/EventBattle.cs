using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Models
{
    public abstract class EventBattle : MonoBehaviour
    {
        public BattleManager battleManager;
        public bool active = false;
        public bool initiated = false;

        // Don't need this, because Event initiated in targeting controller
        // public abstract void Initiate();

        public abstract void EndEvent(out bool busy);
    }
}