using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class ShockEffect : Effect
    {
        public ShockEffect(CharacterBattleController characterBattleController)
        {
            this.characterBattleController = characterBattleController;
            this.name = "Shocked";
            this.description = "-1 EP recovery per turn";
            this.baseDuration = 2;
            this.duration = 2;
            this.iconResourceURL = "AbilityIcons/OverloadAbIcon";

            this.despelable = true;
            this.positive = false;
        }
        
        public override IEnumerator ApplyEffect(List<Effect> effects)
        {
            // Here I can check other effects in list...
            bool noInteruption = true;
            
            foreach (Effect effect in effects)
            {
                if (effect.name.Equals("Shocked"))
                {
                    noInteruption = false;
                    // Method that destroy effect...
                    characterBattleController.DestroyEffect(effect, effects);
                    break;
                }
            }

            if (noInteruption)
            {
                effects.Add(this);
                characterBattleController.AddEffect(this, effects);
                
                characterBattleController.characterDataComponent.apRecoveryModifier -= 1;
                Debug.Log(characterBattleController.gameObject.name +  " is zapped for " + duration + " turns."   );
                yield return null;
            }
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
                characterBattleController.characterDataComponent.apRecoveryModifier += 1;
                effects.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in removal!");
            }
            Debug.Log("Unzapped");
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
            Debug.Log(characterBattleController.gameObject.name +  " is zapped for " + duration + " turns."   );
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