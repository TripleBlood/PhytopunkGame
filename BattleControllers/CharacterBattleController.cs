using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class CharacterBattleController : BattleController
    {
        public CharacterDataComponent characterDataComponent;
        public bool skipTurn;

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
            
            if (Input.GetKeyUp(KeyCode.J))
            {
                StartCoroutine(HPDeltaUI(11));
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
                //Destroy(expiredEffect.uiEffectController.gameObject);
                DestroyEffect(expiredEffect, characterDataComponent.effects);
            }

            // for (int i = 0; i < expiredEffects.Count; i++)
            // {
            //     Destroy(expiredEffects[i].uiEffectController.gameObject);
            //     StartCoroutine(expiredEffects[i].WereOffEffect(characterDataComponent.effects));
            // }

            characterDataComponent.selectionRing.SetActive(false);
            try
            {
                currentTargetingController.EndTargeting();
                Destroy(currentTargetingController);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            // throw new NotImplementedException();
        }

        /// <summary>
        /// AP-recovery, Effect duration decrease...
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void BeginTurn()
        {
            SetMAxAPUI();
            SetCurrentAPActiveUI();
            AvertCurrentApRedUI();
            // SetCurrentApRedUI(3);

            SetMAxEPUI();
            SetCurrentEPActiveUI();
            AvertCurrentEpRedUI();
            // SetCurrentEpRedUI(1);

            UpdateHPUI();
            UpdateEffects();
            
            if (characterDataComponent.position.IsIlluminated)
            {
                DeltaEP(characterDataComponent.epRecoverySun);
            }

            foreach (Effect effect in characterDataComponent.effects)
            {
                StartCoroutine(effect.BeginTurnEffect(characterDataComponent.effects));
            }

            if (skipTurn)
            {
                skipTurn = false;
                this.battleManager.EndTurnBM();
                return;
            }
            
            RecoverAPBeginTurn();
            DeltaEP(characterDataComponent.epRecovery + characterDataComponent.epRecoveryModifier);

            characterDataComponent.selectionRing.SetActive(true);
            currentTargetingController = (TargetingController) gameObject.AddComponent
                (characterDataComponent.targetControllerTypes[0]);
            currentTargetingController.Construct(battleManager, map, this);

            UpdateAbilityIconsUI(battleManager.AbilityBtnList);

            for (int i = 1; i < characterDataComponent.targetControllerTypes.Count; i++)
            {
                SetCD(characterDataComponent.targetControllerTypes[i], characterDataComponent.abilityCooldowns[i] - 1);
            }
            
            
        }
        
        // ———————————————— Variables delta Block ———————————————— //

        public void RecoverAPBeginTurn()
        {
            DeltaAP(characterDataComponent.apRecovery + characterDataComponent.apRecoveryModifier);
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

        public void DeltaHP(int delta)
        {
            characterDataComponent.DeltaHP(Convert.ToInt32(delta * ((double)(100 - characterDataComponent.damageResistance )/(double)100)));
            StartCoroutine(HPDeltaUI(Convert.ToInt32(delta * ((double) (100 - characterDataComponent.damageResistance) / (double) 100))));
            if (battleManager.currentBattleController == this)
            {
                UpdateHPUI();
            }
        }
        
        // ———————————————— Swapping targeting Controller Block ———————————————— //

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

        // ———————————————— UI Block ———————————————— //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abilityBtnList"></param>
        public void UpdateAbilityIconsUI(List<GameObject> abilityBtnList)
        {
            for (int i = 0; i < abilityBtnList.Count; i++)
            {
                if (i + 1 < characterDataComponent.abilities.Count)
                {
                    abilityBtnList[i].SetActive(true);
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

            try
            {
                Text text = battleManager.Hp.transform.Find("HPNumber").gameObject.GetComponent<Text>();
                text.text = characterDataComponent.hp + " / " + characterDataComponent.baseHP;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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

        public void UpdateCooldownUI(Type type)
        {
            int index;

            index = characterDataComponent.targetControllerTypes.IndexOf(type);
            if (index != null)
            {
                GameObject button;
                button = battleManager.AbilityBtnList[index - 1];
                if (characterDataComponent.abilityCooldowns[index] > 0)
                {
                    button.GetComponent<Button>().interactable = false;
                    button.transform.Find("Cooldown").gameObject.SetActive(true);
                    button.transform.Find("Cooldown").gameObject.GetComponent<Text>().text =
                        characterDataComponent.abilityCooldowns[index].ToString();
                }
                else
                {
                    button.GetComponent<Button>().interactable = true;
                    button.transform.Find("Cooldown").gameObject.SetActive(false);
                }
            }
        }

        public void SetCD(Type type, int turns)
        {
            if (turns < 0)
            {
                turns = 0;
            }
            int index;

            index = characterDataComponent.targetControllerTypes.IndexOf(type);
            if (index != null)
            {
                characterDataComponent.abilityCooldowns[index] = turns;
                if (battleManager.currentBattleController == this)
                {
                    UpdateCooldownUI(type);
                }
            }
        }

        public void UpdateEffects()
        {
            for (int i = 0; i < battleManager.EffectPanel.transform.childCount; i++)
            {
                Destroy(battleManager.EffectPanel.transform.GetChild(i).gameObject);
            }
            
            foreach (Effect effect in characterDataComponent.effects)
            {
                GameObject effectIcon = (Instantiate(Resources.Load("EffectIcons/EffectIcon")) as GameObject);
                effectIcon.transform.SetParent(battleManager.EffectPanel.transform, false);
                effect.uiEffectController = effectIcon.GetComponent<UIEffectController>();
                effect.uiEffectController.UpdatCounter(effect.duration.ToString());
                effect.uiEffectController.UpdateIcon(effect.iconResourceURL);
            }
        }

        public void TryAddEffect(Effect effect, List<Effect> effects)
        {
            StartCoroutine(effect.ApplyEffect(effects));
        }
        
        public void AddEffect(Effect effect, List<Effect> effects)
        {
            if (effects == characterDataComponent.effects)
            {
                if (this == battleManager.currentBattleController)
                {
                    GameObject effectIcon = (Instantiate(Resources.Load("EffectIcons/EffectIcon")) as GameObject);
                    effectIcon.transform.SetParent(battleManager.EffectPanel.transform, false);
                    effect.uiEffectController = effectIcon.GetComponent<UIEffectController>();
                    effect.uiEffectController.UpdatCounter(effect.duration.ToString());
                    effect.uiEffectController.UpdateIcon(effect.iconResourceURL);
                }
                
                effects.Add(effect);

                // StartCoroutine(effect.ApplyEffect(effects));
            }
        }
        
        public void DestroyEffect(Effect effect, List<Effect> effects)
        {
            if (effects == characterDataComponent.effects)
            {
                try
                {
                    if (this == battleManager.currentBattleController)
                    {
                        Destroy(effect.uiEffectController.gameObject);
                    }

                    StartCoroutine(effect.WereOffEffect(effects));
                    effects.Remove(effect);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public IEnumerator HPDeltaUI(int delta)
        {
            Color color;
            if (delta > 0)
            {
                color = Color.green;
            }
            else
            {
                color = Color.red;
            }
            
            Tile curTile = characterDataComponent.position;
            Vector3 curTilePoint = map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);

            float yRandom = Random.Range(0, 1);
            float xRandom = Random.Range(-0.5f, +0.5f);
            float zRandom = Random.Range(-0.5f, +0.5f);
            
            
            Vector3 randomOffset = new Vector3(xRandom, yRandom, zRandom);
            
            Vector3 vect = curTilePoint + new Vector3(0, 1.5f, 0) + randomOffset;
            Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

            GameObject text =
                Instantiate(Resources.Load("UIElements/MoveStep")) as GameObject;
            text.transform.SetParent(battleManager.mainUI.transform, false);
            text.GetComponent<UIinSpace>().Initiate(vect, true, color);
            text.GetComponent<UIinSpace>().ChangeText(delta.ToString());
            yield return null;
        }
    }
}