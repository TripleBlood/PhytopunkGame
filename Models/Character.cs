using System.Collections.Generic;

namespace Models
{
    public class Character
    {
        public string name;
        public string description;

        public int baseHP;

        public int hp;
        //Do I need baseHPMosifier

        public int baseAP;
        public int ap;
        public int apRecovery;
        public int apRecoveryModifier;

        public int baseEP;
        public int ep;
        public int epRecovery;
        public int epRecoveryModifier;

        //DamageModifiers?

        public int speed;
        public int speedModifier;
        
        //All upgradeable stats here (Strength, Agility...) 

        //Do I need this?
        public Tile position;

        public bool dead;

        public List<Ability> abilities;
        public List<Effect> effects;

        //Data on equipped items should be here too?
        public Character()
        {
        }

        public Character(string name, string description, int baseHp, int hp, int baseAp, int ap, int apRecovery,
            int apRecoveryModifier, int baseEp, int ep, int epRecovery, int epRecoveryModifier, int speed,
            int speedModifier, Tile position, bool dead, List<Ability> abilities, List<Effect> effects)
        {
            this.name = name;
            this.description = description;
            this.baseHP = baseHp;
            this.hp = hp;
            this.baseAP = baseAp;
            this.ap = ap;
            this.apRecovery = apRecovery;
            this.apRecoveryModifier = apRecoveryModifier;
            this.baseEP = baseEp;
            this.ep = ep;
            this.epRecovery = epRecovery;
            this.epRecoveryModifier = epRecoveryModifier;
            this.speed = speed;
            this.speedModifier = speedModifier;
            this.position = position;
            this.dead = dead;
            this.abilities = abilities;
            this.effects = effects;
        }
    }
}