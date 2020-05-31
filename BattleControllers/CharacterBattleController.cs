using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

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
            
            // TODO: Rewrite wereoff! can't use foreach here becouse collection is modified during cycle!
            foreach (Effect effect in characterDataComponent.effects)
            {
                StartCoroutine(effect.EndTurnEffect(characterDataComponent.effects));
            }
            
            List<Effect> expiredEffects = new List<Effect>();
            foreach (Effect effect in characterDataComponent.effects)
            {
                if (effect.expired)
                {
                    expiredEffects.Add(effect);
                }
            }

            foreach (Effect expiredEffect in expiredEffects)
            {
                StartCoroutine(expiredEffect.WereOffEffect(characterDataComponent.effects));
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
            RecoverAPBeginTurn();

            foreach (Effect effect in characterDataComponent.effects)
            {
                StartCoroutine(effect.BeginTurnEffect(characterDataComponent.effects));
            }

            characterDataComponent.selectionRing.SetActive(true);
            currentTargetingController = (TargetingController) gameObject.AddComponent
                (characterDataComponent.targetControllerTypes[0]);
            currentTargetingController.Construct(battleManager, map, this);

            UpdateAbilityIconsUI(battleManager.AbilityBtnList);
            SetMAxAPUI();
            SetCurrentAPActiveUI();
            AvertCurrentApRedUI();
            // SetCurrentApRedUI(3);

            SetMAxEPUI();
            SetCurrentEPActiveUI();
            AvertCurrentEpRedUI();
            // SetCurrentEpRedUI(1);

            UpdateHPUI();
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
        
        public void DeltaAPRed(int apCost)
        {
            if (battleManager.currentBattleController == this)
            {
                SetMAxAPUI();
                SetCurrentAPActiveUI();
                SetCurrentApRedUI(apCost);
            }
        }
        
        public void DeltaEPRed(int epCost)
        {
            if (battleManager.currentBattleController == this)
            {
                SetMAxEPUI();
                SetCurrentEPActiveUI();
                SetCurrentEpRedUI(epCost);
            }
        }

        public void DeltaAP(int apCost)
        {
            characterDataComponent.DeltaAP(apCost);
            if (battleManager.currentBattleController == this)
            {
                SetMAxAPUI();
                SetCurrentAPActiveUI();
                AvertCurrentApRedUI();
            }
        }
        
        public void DeltaEP(int epCost)
        {
            characterDataComponent.DeltaEP(epCost);
            if (battleManager.currentBattleController == this)
            {
                SetMAxEPUI();
                SetCurrentEPActiveUI();
                AvertCurrentEpRedUI();
            }
        }

        public override void SwapTargeting(int index)
        {
            if (index >= characterDataComponent.targetControllerTypes.Count)
            {
                Debug.Log("Index is out of targeting controllers array");
                return;
            }

            if (characterDataComponent.ap - AbilityUtils.GetAbilityCost(characterDataComponent.abilities[index])[0] >= 0 &&
                characterDataComponent.ep - AbilityUtils.GetAbilityCost(characterDataComponent.abilities[index])[1] >= 0)
            {
                currentTargetingController.EndTargeting();
                Destroy(currentTargetingController);

                currentTargetingController =
                    (TargetingController) gameObject.AddComponent(characterDataComponent.targetControllerTypes[index]);
                currentTargetingController.Construct(battleManager, map, this);
            }
            else
            {
                Debug.Log("No enough resources!");
            }
            
        }

        IEnumerator WaitForSec(float sec)
        {
            yield return WaitForSec(sec);
        }

        public void UpdateAbilityIconsUI(List<GameObject> abilityBtnList)
        {
            for (int i = 0; i < abilityBtnList.Count; i++)
            {
                if (i + 1 < characterDataComponent.abilities.Count)
                {
                    abilityBtnList[i].GetComponent<Image>().sprite =
                        Resources.Load(AbilityUtils.GetAbilityIcon(characterDataComponent.abilities[i + 1]),
                            typeof(Sprite)) as Sprite;
                }
                else
                {
                    abilityBtnList[i].SetActive(false);
                }
            }
        }

        public void UpdateHPUI()
        {
            battleManager.Hp.GetComponent<Image>().fillAmount =
                (float) characterDataComponent.hp / (float) characterDataComponent.baseHP;
        }

        public void SetMAxAPUI()
        {
            for (int i = 0; i < 8; i++)
            {
                if (i < characterDataComponent.baseAP)
                {
                    battleManager.ApPanelsBlack[i].SetActive(true);
                }
                else
                {
                    battleManager.ApPanelsBlack[i].SetActive(false);
                }
            }
        }

        public void SetCurrentAPActiveUI()
        {
            for (int i = 0; i < 8; i++)
            {
                if (i < characterDataComponent.ap)
                {
                    battleManager.ApPanelsActive[i].SetActive(true);
                }
                else
                {
                    battleManager.ApPanelsActive[i].SetActive(false);
                }
            }
        }

        public void SetCurrentApRedUI(int cost)
        {
            for (int i = 0; i < 8; i++)
            {
                if (i < characterDataComponent.ap - cost)
                {
                    battleManager.ApPanelsActive[i].GetComponent<Image>().color =
                        new Color(0.3113208f, 1, 0.5469358f, 1);
                }
                else
                {
                    battleManager.ApPanelsActive[i].GetComponent<Image>().color = new Color(1, 0.1f, 0.2180327f, 1);
                }
            }
        }
        
        public void AvertCurrentApRedUI()
        {
            for (int i = 0; i < 8; i++)
            {
                battleManager.ApPanelsActive[i].GetComponent<Image>().color = new Color(0.3113208f, 1, 0.5469358f, 1);
            }
        }

        public void SetMAxEPUI()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < characterDataComponent.baseEP)
                {
                    battleManager.EpPanelsBlack[i].SetActive(true);
                }
                else
                {
                    battleManager.EpPanelsBlack[i].SetActive(false);
                }
            }
        }

        public void SetCurrentEPActiveUI()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < characterDataComponent.ep)
                {
                    battleManager.EpPanelsActive[i].SetActive(true);
                }
                else
                {
                    battleManager.EpPanelsActive[i].SetActive(false);
                }
            }
        }

        public void SetCurrentEpRedUI(int cost)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < characterDataComponent.ep - cost)
                {
                    battleManager.EpPanelsActive[i].GetComponent<Image>().color = new Color(1, 0.7876751f, 0, 1);
                }
                else
                {
                    battleManager.EpPanelsActive[i].GetComponent<Image>().color = new Color(1, 0.1f, 0.2180327f, 1);
                }
            }
        }

        public void AvertCurrentEpRedUI()
        {
            for (int i = 0; i < 5; i++)
            {
                battleManager.EpPanelsActive[i].GetComponent<Image>().color = new Color(1, 0.7876751f, 0, 1);
            }
        }
    }
}