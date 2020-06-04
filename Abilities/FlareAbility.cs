using System.Collections;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class FlareAbility : Ability
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
                StartCoroutine(FlareMain());
            }
        }
        
        private IEnumerator FlareMain()
        {
            active = false;
            
            GameObject projectile =
                Instantiate(Resources.Load("Projectiles/Potion/Potion"), origin, Quaternion.identity) as GameObject;
            Debug.Log("Burn!!");
            yield return null;

            if (targetBC != attackerBC)
            {
                Debug.Log(targetBC == attackerBC);
                while ((projectile.transform.position - destination).magnitude >= 0.005f)
                {
                    projectile.transform.position =
                        Vector3.MoveTowards(projectile.transform.position, destination, 10 * Time.deltaTime);
                    yield return null;
                }
            }

            targetBC.DeltaHP(-25);
            Destroy(projectile);

            BurnEffect burnEffect = new BurnEffect(targetBC);
            
            targetBC.TryAddEffect(burnEffect, targetBC.characterDataComponent.effects);
            
            // StartCoroutine(stimulatorEffect.ApplyEffect(targetBC.characterDataComponent.effects));

            Debug.Log(description);
            
            EndEvent(out battleManager.busy);
        }
        
        public override void EndEvent(out bool busy)
        {
            Destroy(this);
            busy = false;
        }
    }
}