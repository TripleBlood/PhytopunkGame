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
        }
        
        public override IEnumerator ApplyEffect(List<Effect> effects)
        {
            effects.Add(this);
            characterBattleController.characterDataComponent.apRecoveryModifier -= 1;
            Debug.Log(characterBattleController.gameObject.name +  " is zapped for " + duration + " turns."   );
            yield return null;
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
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
            Debug.Log(characterBattleController.gameObject.name +  " is zapped for " + duration + " turns."   );
            yield return null;
        }

        public override IEnumerator EndTurnEffect(List<Effect> effects)
        {
            if (duration == 0)
            {
                characterBattleController.characterDataComponent.DeleteEffect(this);
            }
            yield return null;
        }
    }
}