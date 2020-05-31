using System.Collections.Generic;
using DefaultNamespace;

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
        
        public void SetProperties(string name, string description, int APcost, int EPcost, int baseCooldown,
            int cooldown, bool targetType)
        {
            this.name = name;
            this.description = description;
            this.APcost = APcost;
            this.EPcost = EPcost;
            this.baseCooldown = baseCooldown;
            this.cooldown = cooldown;
            this.targetType = targetType;
        }
        
    }
}