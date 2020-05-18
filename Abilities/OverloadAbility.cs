using System.Collections;
using System.Collections.Generic;
using Models;

namespace DefaultNamespace
{
    public class OverloadAbility : Ability
    {
        public OverloadAbility()
        {
            //TODO: Replace from file read...
            this.name = "Overload";
            this.description = "Description";
            this.APcost = 1;
            this.EPcost = 2;
            this.baseCooldown = 3;
            this.cooldown = 0;
            this.targetType = true;
        }

        public override void EndEvent(out bool busy)
        {
            throw new System.NotImplementedException();
        }
    }
}