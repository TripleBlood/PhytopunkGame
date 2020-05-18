using System.Collections.Generic;

namespace Models
{
    public abstract class Ability : EventBattle
    {
        //TODO: check how readonly works
        public string name;
        public string description;
        public int APcost;
        public int EPcost;
        public int baseCooldown;
        public int cooldown;
        
        /// <summary>
        /// True — character target, false — tile target
        /// </summary>
        public bool targetType;

        public List<Effect> effectList;
        
    }
}