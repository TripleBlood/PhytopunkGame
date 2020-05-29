using System;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class CharacterDataComponent : DataComponent
    {
        public Side side;
        
        //Do I need baseHPMosifier?

        /// <summary>
        /// Max AP
        /// </summary>
        public int baseAP;
        /// <summary>
        /// Current AP
        /// </summary>
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

        

        

        /// <summary>
        /// String representation of abilities. Need to initiate battle! 
        /// </summary>
        public List<String> abilities;
        
        /// <summary>
        /// Stores
        /// </summary>
        public List<Type> targetControllerTypes;

        public List<int> abilityCooldowns;
        
        public List<Effect> effects = new List<Effect>();
        
        private Character _character;


        public GameObject selectionRing;
        public GameObject observeRing;

        public void DeleteEffect(Effect effect)
        {
            try
            {
                StartCoroutine(effect.WereOffEffect(this.effects));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in effect were off");
                
            }
        }

        private void Awake()
        {
            targetControllerTypes = new List<Type>();
            foreach (string ability in abilities)
            {
                targetControllerTypes.Add(AbilityUtils.GetTargetingControllerType(ability));
                abilityCooldowns.Add(0);
            }
            // throw new NotImplementedException();
        }


        // void Start()
        // {
        //     // Initiate character variable should be from savefile, but for now it's created form default constructor
        //     
        // }
        //
        // private void Update()
        // {
        //     // Heh
        // }
        
    }
}