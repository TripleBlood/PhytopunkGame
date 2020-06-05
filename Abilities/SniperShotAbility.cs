using System;
using System.Collections;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class SniperShotAbility : Ability
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
                StartCoroutine(SniperShotMain());
            }
        }
        
        private IEnumerator SniperShotMain()
        {
            active = false;
            
            GameObject projectile =
                Instantiate(Resources.Load("Projectiles/Potion/Potion"), origin, Quaternion.identity) as GameObject;
            Debug.Log("Snipe!!");
            yield return null;

            if (targetBC != attackerBC)
            {
                Debug.Log(targetBC == attackerBC);
                while ((projectile.transform.position - destination).magnitude >= 0.005f)
                {
                    projectile.transform.position =
                        Vector3.MoveTowards(projectile.transform.position, destination, 30 * Time.deltaTime);
                    yield return null;
                }
            }

            
            targetBC.DeltaHP(-(attackerBC.characterDataComponent.baseDamage + Convert.ToInt32((destination-origin).magnitude)*3));
            Destroy(projectile);

            Debug.Log(Convert.ToInt32((destination-origin).magnitude));
            
            EndEvent(out battleManager.busy);
        }
        
        public override void EndEvent(out bool busy)
        {
            Destroy(this);
            busy = false;
        }
        
    }
}