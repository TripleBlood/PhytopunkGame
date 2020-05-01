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

        public override bool CastAbility(Tile target)
        {
            throw new System.NotImplementedException();
            
        }
    }
}