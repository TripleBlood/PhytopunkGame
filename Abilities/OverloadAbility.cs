using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class OverloadAbility : Ability
    {
        public CharacterBattleController target;
        
        public OverloadAbility()
        {
            //TODO: Replace from file read...
            this.name = "Overload";
            this.description = "Description";
            this.APcost = 1;
            this.EPcost = 2;
            this.baseCooldown = 3;
            this.cooldown = 0;
            this.targetType = true;
        }

        public override void EndEvent(out bool busy)
        {
            Destroy(this);
            busy = false;
        }

        public void Update()
        {
            if (initiated && active)
            {
                StartCoroutine(OverloadMain());
            }
        }

        private IEnumerator OverloadMain()
        {
            for (int i = 0; i < 3; i++)
            {
                Debug.Log("Overload in " + (3-i));
                yield return new WaitForSeconds(1);
            }

            StartCoroutine(new ShockEffect(target).ApplyEffect(target.characterDataComponent.effects));

            Debug.Log(description);
            
            EndEvent(out battleManager.busy);
        }
    }
}