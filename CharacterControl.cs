using System;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class CharacterControl : MonoBehaviour
    {
        public string name;
        public string description;
        public enum Side
        {
            ALLIE, ENEMY, NEUTRAL_ENEMY, NEUTRAL_FRIEND 
        }

        public Side side;

        public int baseHP;
        public int hp;
        //Do I need baseHPMosifier?

        public int baseAP;
        public int ap;
        public int apRecovery;
        public int apRecoveryModifier;

        public int baseEP;
        public int ep;
        public int epRecovery;
        public int epRecoveryModifier;

        //DamageModifiers?

        public int speed;
        public int speedModifier;
        
        //All upgradeable stats here (Strength, Agility...) 

        //Do I need this?
        public Tile position;

        public bool dead;

        public List<Ability> abilities;
        public List<Effect> effects;
        
        private Character _character;
        
        void Start()
        {
            // Initiate character variable should be from savefile, but for now it's created form default constructor
            
        }

        private void Update()
        {
            // Heh
        }
    }
}