using System.Collections.Generic;

namespace Models
{
    public abstract class Ability
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
        
        /// <summary>
        /// Describes ability's behavior
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract bool CastAbility(Tile target);
    }
}