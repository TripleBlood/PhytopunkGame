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
            active = false;

            ShockEffect shockEffect = new ShockEffect(target);
            
            target.TryAddEffect(shockEffect, target.characterDataComponent.effects);
            
            // StartCoroutine(shockEffect.ApplyEffect(target.characterDataComponent.effects));
            
            // if (target == battleManager.currentBattleController)
            // {
            //     GameObject effectIcon = (Instantiate(Resources.Load("EffectIcons/EffectIcon")) as GameObject);
            //     effectIcon.transform.SetParent(battleManager.EffectPanel.transform, false);
            //     shockEffect.uiEffectController = effectIcon.GetComponent<UIEffectController>();
            //     shockEffect.uiEffectController.UpdatCounter(shockEffect.duration.ToString());
            //     shockEffect.uiEffectController.UpdateIcon(shockEffect.iconResourceURL);
            // }

            Debug.Log(description);
            
            EndEvent(out battleManager.busy);
            yield return null;
        }
    }
}