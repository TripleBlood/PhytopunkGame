using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class SplitShotAbility : Ability
    {
        public CharacterBattleController attackerBC;
        public List<CharacterBattleController> targetBC;

        public List<Vector3> origin;
        public List<Vector3> destination;

        private int finishedStrikes = 0;
        
        private void Awake()
        {
            initiated = false;
            active = false;
        }

        private void Update()
        {
            if (initiated && active)
            {
                StartCoroutine(SplitShotMain());
            }
        }

        private IEnumerator SplitShotMain()
        {
            active = false;


            for (int i = 0; i < targetBC.Count; i++)
            {
                StartCoroutine(SplitShotSub(i));
                yield return new WaitForSeconds(0.2f);
            }

            while (finishedStrikes < 3)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            EndEvent(out battleManager.busy);
        }

        private IEnumerator SplitShotSub(int index)
        {
            GameObject projectile = Instantiate(Resources.Load("Projectiles/Potion/Potion"), origin[index], Quaternion.identity) as GameObject;
            Debug.Log("HEAL!");
            yield return null;

            if (targetBC[index] != attackerBC)
            {
                Debug.Log(targetBC[index] == attackerBC);
                while ((projectile.transform.position - destination[index]).magnitude >= 0.005f)
                {
                    projectile.transform.position =
                        Vector3.MoveTowards(projectile.transform.position, destination[index], 10 * Time.deltaTime);
                    yield return null;
                }
            }

            targetBC[index].DeltaHP(-(Convert.ToInt32(attackerBC.characterDataComponent.baseDamage * attackerBC.characterDataComponent.damageMultModifier) + attackerBC.characterDataComponent.damageAddModifier));
            Destroy(projectile);
            
            Debug.Log(description);
            finishedStrikes++;
        }

        public override void EndEvent(out bool busy)
        {
            Destroy(this);
            busy = false;
        }
    }
}