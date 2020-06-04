using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class FrozenEffect : Effect
    {
        public FrozenEffect(CharacterBattleController characterBattleController)
        {
            this.characterBattleController = characterBattleController;
            this.name = "Frozen";
            this.description = "Skips turn";
            this.baseDuration = 1;
            this.duration = 1;
            this.iconResourceURL = "AbilityIcons/CryoblastAbIcon";

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
                    if (effects[i].name.Equals("Burn"))
                    {
                        noInteruption = false;
                        characterBattleController.DestroyEffect(effects[i], effects);
                        i--;
                        
                        WetEffect wetEffect = new WetEffect(characterBattleController);
                        characterBattleController.AddEffect(wetEffect, effects);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (noInteruption)
            {
                effects.Add(this);
                characterBattleController.AddEffect(this, effects);
            
                Debug.Log(characterBattleController.gameObject.name +  " is burning");
                yield return null;
            }
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
                WetEffect wetEffect = new WetEffect(characterBattleController);
                characterBattleController.AddEffect(wetEffect, effects);
                
                characterBattleController.DestroyEffect(this, effects);
                // effects.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in removal!");
            }
            Debug.Log("Freeze effect were off");
            yield return null;
        }

        public override IEnumerator BeginTurnEffect(List<Effect> effects)
        {
            duration -= 1;
            try
            {
                characterBattleController.skipTurn = true;
                uiEffectController.UpdatCounter(duration.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Debug.Log(characterBattleController.gameObject.name +  " is frozen for " + duration + " turns."   );
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