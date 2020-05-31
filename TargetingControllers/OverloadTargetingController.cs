using System;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class OverloadTargetingController : TargetingController
    {
        private GameObject courser;

        private CharacterBattleController targetForAttack;
        private CharacterBattleController currentCharControl;
        private CharacterDataComponent _currentCharacterDataComponent;

        private GameObject confirmButtonGameObject;
        private Button confirmButton;

        private float timeHoldingBtn;

        private bool targetSelected;

        private void Awake()
        {
            courser = Instantiate(Resources.Load("MapCoursers/mapTileCourser") as GameObject);
        }

        private void Update()
        {
            if (initiated)
            {
                int mask = (1 << 9) | (1 << 13);
                //mask = ~mask;

                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                    Single.PositiveInfinity, mask);
                if (hit)
                {
                    //check on traversability
                    courser.transform.position = map.GetCourserPosition(hitInfo.point);
                }

                if (targetSelected)
                {
                    if (Input.GetMouseButton(1))
                    {
                        timeHoldingBtn += Time.deltaTime;
                    }

                    if (Input.GetMouseButtonUp(1))
                    {
                        if (timeHoldingBtn <= 0.3f)
                        {
                            Deselect();
                        }

                        timeHoldingBtn = 0;
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (EventSystem.current.IsPointerOverGameObject()) return;

                        RaycastHit hitLMBInfo = new RaycastHit();
                        bool hitLMB = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                            out hitLMBInfo,
                            Single.PositiveInfinity, mask);
                        if (hitLMB)
                        {
                            Component bc;
                            if (hitLMBInfo.collider.TryGetComponent(typeof(BattleController), out bc))
                            {
                                targetForAttack =
                                    hitLMBInfo.collider.gameObject.GetComponent<CharacterBattleController>();
                                if (currentCharControl.characterDataComponent.ap > 1)
                                {
                                    Tile curTile = targetForAttack.characterDataComponent.position;

                                    Vector3 curTilePoint =
                                        map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);
                                    Vector3 vect = curTilePoint + new Vector3(0, 0.7f, 0);
                                    Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

                                    confirmButtonGameObject =
                                        Instantiate(Resources.Load("UIElements/AttackBtn")) as GameObject;
                                    confirmButtonGameObject.transform.SetParent(mainUI.transform, false);
                                    confirmButton = confirmButtonGameObject.GetComponent<Button>();
                                    confirmButton.onClick.AddListener(ConfirmTarget);
                                    confirmButtonGameObject.GetComponent<UIinSpace>()
                                        .Initiate(vect, new Vector2(20, 45));

                                    targetSelected = true;

                                    return;
                                }
                            }
                        }
                    }
                    
                    if (Input.GetMouseButton(1))
                    {
                        timeHoldingBtn += Time.deltaTime;
                    }

                    if (Input.GetMouseButtonUp(1))
                    {
                        if (timeHoldingBtn <= 0.3f)
                        {
                            // SwapTargetingController
                            // EndTargeting();
                            currentCharControl.SwapTargeting(0);
                        }

                        timeHoldingBtn = 0;
                    }
                }
            }
        }

        public override void ConfirmTarget()
        {
            Type type = typeof(OverloadAbility);
            OverloadAbility overloadAbility = (OverloadAbility) battleManager.gameObject.AddComponent(type);

            try
            {
                overloadAbility.SetProperties("Overload",
                    "Zapps target, dealing 20 damage and applying \"Shocked\" status for 2 turns", 2, 1, 2, 2, true);
                overloadAbility.battleManager = battleManager;
                overloadAbility.target = targetForAttack;
                
                currentCharControl.DeltaAP(-2);
                currentCharControl.DeltaEP(-1);

                overloadAbility.initiated = true;
                
                battleManager.eventQueue.Add(overloadAbility);
                
                // Some shit

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Deselect();
            currentCharControl.SwapTargeting(0);
        }

        public override void Construct(BattleManager battleManager, Map map, BattleController currentBattleController)
        {
            this.battleManager = battleManager;
            this.map = map;
            this.currentBattleController = currentBattleController;
            mainUI = battleManager.mainUI;

            currentCharControl = (CharacterBattleController) currentBattleController;
            _currentCharacterDataComponent = currentCharControl.characterDataComponent;
            
            currentCharControl.DeltaAPRed(2);
            currentCharControl.DeltaEPRed(1);

            initiated = true;
        }

        public override void EndTargeting()
        {
            
            currentCharControl.AvertCurrentApRedUI();
            currentCharControl.AvertCurrentEpRedUI();
            
            initiated = false;
            
            Destroy(courser);
            Deselect();
            
            // currentCharControl.SwapTargeting(0);
            // SwapTargetingController!!!

            Destroy(this);
        }

        public void Deselect()
        {
            
            if (targetSelected)
            {
                Destroy(confirmButtonGameObject);
            } 
            //
            // try
            // {
            //     Destroy(confirmButtonGameObject);
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            // }
            

            targetSelected = false;
        }
    }
}