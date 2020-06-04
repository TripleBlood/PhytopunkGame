using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class WetEffect : Effect
    {
        public WetEffect(CharacterBattleController characterBattleController)
        {
            this.characterBattleController = characterBattleController;
            this.name = "Wet";
            this.description = "Just wet, beware of electricity!";
            this.baseDuration = 1;
            this.duration = 1;
            
            // TODO: need to change icon!
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
                    if (effects[i].name.Equals("Shock") || effects[i].name.Equals("Wet"))
                    {
                        noInteruption = false;
                        characterBattleController.DestroyEffect(effects[i], effects);
                        
                        StunnedEffect stunnedEffect = new StunnedEffect(characterBattleController);
                        characterBattleController.AddEffect(stunnedEffect, effects);
                        i--;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
            effects.Add(this);
            characterBattleController.AddEffect(this, effects);
            
            Debug.Log(characterBattleController.gameObject.name +  " is wet");
            yield return null;
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            try
            {
                characterBattleController.DestroyEffect(this, effects);
                // effects.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log("Error in removal!");
            }
            Debug.Log("Wet effect were off");
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
            Debug.Log(characterBattleController.gameObject.name +  " is wet for " + duration + " turns."   );
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