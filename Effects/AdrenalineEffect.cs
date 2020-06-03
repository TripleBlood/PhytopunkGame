using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class AdrenalineEffect : Effect
    {
        
        public AdrenalineEffect(CharacterBattleController characterBattleController)
        {
            this.characterBattleController = characterBattleController;
            this.name = "Adrenaline";
            this.description = "-2 EP recovery per turn";
            this.baseDuration = 1;
            this.duration = 1;
            this.iconResourceURL = "AbilityIcons/AdrenalineAbIcon";

            this.despelable = false;
            this.positive = false;
        }
        
        public override IEnumerator ApplyEffect(List<Effect> effects)
        {
            effects.Add(this);
            characterBattleController.AddEffect(this, effects);
                
            characterBattleController.characterDataComponent.apRecoveryModifier -= 2;
            Debug.Log(characterBattleController.gameObject.name +  " is adrenalized");
            yield return null;
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
                characterBattleController.characterDataComponent.apRecoveryModifier += 2;
                effects.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in removal!");
            }
            Debug.Log("Adrenaline effect were off");
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
            Debug.Log(characterBattleController.gameObject.name +  " is adrenalized for " + duration + " turns."   );
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