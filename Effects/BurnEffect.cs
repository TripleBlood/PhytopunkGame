using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class BurnEffect : Effect
    {
        
        public BurnEffect(CharacterBattleController characterBattleController)
        {
            this.characterBattleController = characterBattleController;
            this.name = "Burn";
            this.description = "-10 HP every turn";
            this.baseDuration = 3;
            this.duration = 3;
            this.iconResourceURL = "AbilityIcons/FlareAbIcon";

            this.despelable = true;
            this.positive = false;
        }
        
        public override IEnumerator ApplyEffect(List<Effect> effects)
        {
            bool noInteruption = true;

            int limit = effects.Count;
            for (int i = 0; i < limit; i++)
            {
                try
                {
                    if (effects[i].name.Equals("Frozen"))
                    {
                        noInteruption = false;
                        characterBattleController.DestroyEffect(effects[i], effects);
                        i--;
                        
                        // WetEffect wetEffect = new WetEffect(characterBattleController);
                        // characterBattleController.TryAddEffect(wetEffect, effects);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (noInteruption)
            {
                //effects.Add(this);
                characterBattleController.AddEffect(this, effects);
            
                Debug.Log(characterBattleController.gameObject.name +  " is burning");
                yield return null;
            }
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
                // characterBattleController.DestroyEffect(this, effects);
                // effects.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in removal!");
            }
            Debug.Log("Burn effect were off");
            yield return null;
        }

        public override IEnumerator BeginTurnEffect(List<Effect> effects)
        {
            duration -= 1;
            try
            {
                characterBattleController.DeltaHP(-10);
                uiEffectController.UpdatCounter(duration.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Debug.Log(characterBattleController.gameObject.name +  " is burning for " + duration + " turns."   );
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