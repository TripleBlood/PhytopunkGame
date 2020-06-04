using System.Collections;
using System.Collections.Generic;
using Models;

namespace DefaultNamespace
{
    public class HackedEffect : Effect
    {
        public override IEnumerator ApplyEffect(List<Effect> effects)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator WereOffEffect(List<Effect> effects)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator BeginTurnEffect(List<Effect> effects)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator EndTurnEffect(List<Effect> effects)
        {
            throw new System.NotImplementedException();
        }
    }
}