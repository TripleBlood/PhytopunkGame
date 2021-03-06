using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Models
{
    public abstract class Effect
    {
        public string name;
        public string description;
        public int baseDuration;
        public int duration;
        public bool expired;
        public string iconResourceURL;
        
        public bool positive;
        public bool despelable;

        public UIEffectController uiEffectController;
        
        public CharacterBattleController characterBattleController;
        
        public abstract IEnumerator ApplyEffect(List<Effect> effects);

        public abstract IEnumerator WereOffEffect(List<Effect> effects);

        public abstract IEnumerator BeginTurnEffect(List<Effect> effects);

        public abstract IEnumerator EndTurnEffect(List<Effect> effects);
    }
}