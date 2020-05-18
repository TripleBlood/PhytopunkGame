using System.Collections;
using System.Collections.Generic;
using Models;

namespace DefaultNamespace
{
    public class MoveAbility : Ability
    {
        private List<Tile> path;
        
        
        public MoveAbility(List<Tile> path, int APcost)
        {
            this.name = "Move";
            this.description = "Move character to tile";
            this.APcost = APcost;
            this.EPcost = 0;
            this.baseCooldown = 0;
            this.cooldown = 0;
            this.targetType = true;
            this.path = path;
        }

        public override void EndEvent(out bool busy)
        {
            throw new System.NotImplementedException();
        }
    }
}
