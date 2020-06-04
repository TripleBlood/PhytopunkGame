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
        public int epRecoverySun;

        //DamageModifiers?
        public int baseDamage;
        public double damageMultModifier;
        public int damageAddModifier;

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

        public override void DeltaHP(int delta)
        {
            if (hp + delta > baseHP)
            {
                hp = baseHP;
            }
            else if (hp + delta > 0)
            {
                hp += delta;
            }
            else
            {
                hp = 0;
                dead = true;
            }
        }

        public void DeltaAP(int delta)
        {
            if (ap + delta > baseAP)
            {
                ap = baseAP;
            }
            else if (hp + delta >= 0)
            {
                ap += delta;
            }
            else
            {
                ap = 0;
                throw new Exception("Some unexpected shit happened. Cast availability wasn't checked! AP");
            }
        }

        public void DeltaEP(int delta)
        {
            if (ep + delta > baseEP)
            {
                ep = baseEP;
            }
            else if (ep + delta >= 0)
            {
                ep += delta;
            }
            else
            {
                ep = 0;
                throw new Exception("Some unexpected shit happened. Cast availability wasn't checked! EP");
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