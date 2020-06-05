using System;
using System.Collections;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class AttackAbility : Ability
    {
        public GameObject attacker;
        public GameObject target;

        public CharacterBattleController attackerBC;
        public CharacterBattleController targetBC;

        public Vector3 origin;
        public Vector3 destination;

        private void Awake()
        {
            initiated = false;
            active = false;
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

        public override void EndEvent(out bool busy)
        {
            Destroy(this);
            busy = false;
        }

        IEnumerator Placeholder()
        {
            GameObject projectile =
                Instantiate(Resources.Load("Projectiles/Potion/Potion"), origin, Quaternion.identity) as GameObject;
            Debug.Log("DAMAGE!");
            yield return null;

            while ((projectile.transform.position - destination).magnitude >= 0.005f)
            {
                projectile.transform.position =
                    Vector3.MoveTowards(projectile.transform.position, destination, 10 * Time.deltaTime);
                yield return null;
            }

            targetBC.DeltaHP(-(Convert.ToInt32(attackerBC.characterDataComponent.baseDamage * attackerBC.characterDataComponent.damageMultModifier) + attackerBC.characterDataComponent.damageAddModifier));
            Destroy(projectile);
            
            EndEvent(out battleManager.busy);
        }
    }
}