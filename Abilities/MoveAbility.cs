using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveAbility : Ability
    {
        public List<Tile> path;
        public GameObject movingObject;


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

        public override void EndEvent(out bool busy)
        {
            Destroy(this);
            busy = false;
        }

        private void Update()
        {
            if (initiated && active)
            {
                StartCoroutine(Placeholder());
                active = false;
                return;
            }
            
        }

        IEnumerator Placeholder()
        {
            for (int i = 0; i < 3; i++)
            {
                Debug.Log("Fuck me, " + movingObject.name + " "+ i);
                yield return new WaitForSeconds(2);
            }

            EndEvent(out battleManager.busy);
        }
    }
}