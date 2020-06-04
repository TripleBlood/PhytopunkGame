using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class StimulatorEffect : Effect
    {
        public StimulatorEffect(CharacterBattleController characterBattleController)
        {
            this.characterBattleController = characterBattleController;
            this.name = "Stimulator";
            this.description = "Replenish 7 HP per turn. Increases speed by 1. Increases AP recovery per tern by 1";
            this.baseDuration = 2;
            this.duration = 2;
            this.iconResourceURL = "AbilityIcons/StimulatorAbIcon";

            this.despelable = false;
            this.positive = true;
        }
        
        public override IEnumerator ApplyEffect(List<Effect> effects)
        {
            //effects.Add(this);
            characterBattleController.AddEffect(this, effects);
                
            characterBattleController.characterDataComponent.apRecoveryModifier += 1;
            characterBattleController.characterDataComponent.speedModifier += 1;
            
            Debug.Log(characterBattleController.gameObject.name +  " is under effect of stimulator");
            yield return null;
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
                characterBattleController.characterDataComponent.apRecoveryModifier -= 1;
                characterBattleController.characterDataComponent.speedModifier -= 1;
                
                // characterBattleController.DestroyEffect(this, effects);
                // effects.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in removal!");
            }
            Debug.Log("Stimulator effect were off");
            yield return null;
        }

        public override IEnumerator BeginTurnEffect(List<Effect> effects)
        {
            duration -= 1;
            characterBattleController.DeltaHP(10);
            
            try
            {
                uiEffectController.UpdatCounter(duration.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Debug.Log(characterBattleController.gameObject.name +  " is under effect pf stimulator for " + duration + " turns."   );
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