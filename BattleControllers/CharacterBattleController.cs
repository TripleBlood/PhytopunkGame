using System;
using System.Collections;
using Models;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DefaultNamespace
{
    public class CharacterBattleController : BattleController
    {
        public CharacterDataComponent characterDataComponent;

        private void Start()
        {
            // characterData = this.gameObject.GetComponent<CharacterControl>();
            // Debug.Log(characterData.name);
            // throw new NotImplementedException();
        }

        private void Awake()
        {
            characterDataComponent = this.gameObject.GetComponent<CharacterDataComponent>();
            // Debug.Log(characterDataComponent.name);
        }

        void Update()
        {
            Debug.Log("I am " + gameObject.name + "!");

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                this.battleManager.EndTurnBM();
            }
        }

        public override void EndTurnBC()
        {
            foreach (Effect effect in characterDataComponent.effects)
            {
                StartCoroutine(effect.EndTurnEffect(characterDataComponent.effects));
            }

            characterDataComponent.selectionRing.SetActive(false);
            currentTargetingController.EndTargeting();
            Destroy(currentTargetingController);
            // throw new NotImplementedException();
        }

        /// <summary>
        /// AP-recovery, Effect duration decrease...
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void BeginTurn()
        {
            // this.Start();
            RecoverAPBeginTurn();

            foreach (Effect effect in characterDataComponent.effects)
            {
                StartCoroutine(effect.BeginTurnEffect(characterDataComponent.effects));
            }

            characterDataComponent.selectionRing.SetActive(true);
            currentTargetingController = (TargetingController) gameObject.AddComponent
                (characterDataComponent.targetControllerTypes[0]);
            currentTargetingController.Construct(battleManager, map, this);
            // gh
        }

        public void RecoverAPBeginTurn()
        {
            // Notify Observer!?
            if (characterDataComponent.ap +
                (characterDataComponent.apRecovery + characterDataComponent.apRecoveryModifier) <
                characterDataComponent.baseAP)
            {
                characterDataComponent.ap +=
                    (characterDataComponent.apRecovery + characterDataComponent.apRecoveryModifier);
            }
            else
            {
                characterDataComponent.ap = characterDataComponent.baseAP;
            }
        }

        public override void SwapTargeting(int index)
        {
            if (index >= characterDataComponent.targetControllerTypes.Count)
            {
                Debug.Log("Index is out of targeting controllers array");
                return;
            }
            
            // TODO: Swap and targeting calls eachother FIX!
            
            currentTargetingController.EndTargeting();
            Destroy(currentTargetingController);

            currentTargetingController =
                (TargetingController) gameObject.AddComponent(characterDataComponent.targetControllerTypes[index]);
            currentTargetingController.Construct(battleManager, map, this);
        }

        IEnumerator WaitForSec(float sec)
        {
            yield return WaitForSec(sec);
        }
    }
}