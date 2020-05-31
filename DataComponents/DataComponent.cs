using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class DataComponent : MonoBehaviour
    {
        public string nameData;
        public string description;
        
        public int baseHP;
        public int hp;

        public abstract void DeltaHP(int delta);
        // TODO: Need Death and Resurrect methods! 
        
        public bool dead;
        
        //Do I need this?
        public Tile position;
        public Vector3 vectPosition;

        /// <summary>
        /// If battleController make tile impassable — true.
        /// </summary>
        public bool requiresSpace;

        public bool movable;
        public bool targetable;
        
        // Need abstract method to manageHP here predefined!
        // Need abstract targeting method predefined here?
    }
}