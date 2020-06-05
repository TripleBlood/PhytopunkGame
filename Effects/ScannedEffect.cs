using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class ScannedEffect : Effect
    {
        public ScannedEffect(CharacterBattleController characterBattleController)
        {
            this.characterBattleController = characterBattleController;
            this.name = "Scanned";
            this.description = "Takes 50% more damage";
            this.baseDuration = 2;
            this.duration = 2;
            this.iconResourceURL = "AbilityIcons/VulnerabilityScanAbIcon";

            this.despelable = true;
            this.positive = false;
        }
        
        public override IEnumerator ApplyEffect(List<Effect> effects)
        {
            //effects.Add(this);
            characterBattleController.AddEffect(this, effects);
                
            characterBattleController.characterDataComponent.damageResistance -= 25;
            
            Debug.Log(characterBattleController.gameObject.name +  "'s Vulnerabilities are scanned!");
            yield return null;
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
                characterBattleController.characterDataComponent.damageResistance += 25;
                
                // characterBattleController.DestroyEffect(this, effects);
                // effects.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in removal!");
            }
            Debug.Log("Vulnerability scan effect were off");
            yield return null;
        }

        public override IEnumerator BeginTurnEffect(List<Effect> effects)
        {
            duration -= 1;
            
            try
            {
                uiEffectController.UpdatCounter(duration.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Debug.Log(characterBattleController.gameObject.name +  "'s Vulnerabilities are scanned for " + duration + " turns."   );
            yield return null;
        }

        public override IEnumerator EndTurnEffect(List<Effect> effects)
        {
            if (duration < 1)
            {
                expired = true;
            }
            yield return null;
        }
    }
}