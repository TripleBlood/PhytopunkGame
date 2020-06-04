using System;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class StimulatorTargetingController : TargetingController
    {
        private GameObject courser;

        private CharacterBattleController targetForAttack;
        private CharacterBattleController currentCharControl;
        private CharacterDataComponent _currentCharacterDataComponent;
        
        private Vector3 originForAttack;
        private Vector3 destinationForAttcak;

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
                                    Vector3 attackAlignment =
                                        hitLMBInfo.collider.gameObject.transform.position -
                                        gameObject.transform.position;

                                    float angle = Vector3.SignedAngle(attackAlignment, Vector3.forward, Vector3.up);

                                    int adjIndex = TargetingUtils.GetAdjIndexByAngle(angle);
                                    int opponentAdjIndex = (adjIndex + 2) % 4;

                                    List<Vector3> attackOrigin =
                                        TargetingUtils.GetPointsOrigin(map, _currentCharacterDataComponent.position,
                                            adjIndex);
                                    List<Vector3> attackDestination =
                                        TargetingUtils.GetPointsOrigin(map,
                                            targetForAttack.characterDataComponent.position,
                                            opponentAdjIndex);

                                    foreach (Vector3 origin in attackOrigin)
                                    {
                                        foreach (Vector3 destination in attackDestination)
                                        {
                                            // int maskForAttack = ~(1 << 9);
                                            RaycastHit hitAtckInfo = new RaycastHit();
                                            bool hitAtck = Physics.Raycast(origin, destination - origin,
                                                out hitAtckInfo,
                                                (destination - origin).magnitude);

                                            Debug.DrawLine(origin, destination, Color.white, 5);

                                            if (!hitAtck || hitAtckInfo.collider.gameObject
                                                    .GetComponent<CharacterBattleController>() == targetForAttack)
                                            {
                                                originForAttack = origin;
                                                destinationForAttcak = destination;
                                                
                                                Tile curTile = targetForAttack.characterDataComponent.position;

                                                Vector3 curTilePoint =
                                                    map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);
                                                Vector3 vect = curTilePoint + new Vector3(0, 0.7f, 0);
                                                Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

                                                confirmButtonGameObject =
                                                    Instantiate(Resources.Load("UIElements/ConfirmBtn")) as GameObject;
                                                confirmButtonGameObject.transform.SetParent(mainUI.transform, false);
                                                confirmButton = confirmButtonGameObject.GetComponent<Button>();
                                                confirmButton.onClick.AddListener(ConfirmTarget);
                                                confirmButtonGameObject.GetComponent<UIinSpace>()
                                                    .Initiate(vect, new Vector2(20, 45));

                                                targetSelected = true;
                                                return;
                                            }
                                            else
                                            {
                                                //Debug.Log("Attack is unavailable!");
                                            }
                                        }
                                    }
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
            Type type = typeof(StimulatorAbility);
            StimulatorAbility stimulatorAbility = (StimulatorAbility) battleManager.gameObject.AddComponent(type);

            try
            {
                stimulatorAbility.SetProperties("Stimulator",
                    "Heal 25 hp. Applies \"Stimulator\"", 2, 0, 4, 4, true);
                stimulatorAbility.battleManager = battleManager;
                stimulatorAbility.targetBC = targetForAttack;
                stimulatorAbility.attackerBC = currentCharControl;
                stimulatorAbility.origin = originForAttack;
                stimulatorAbility.destination = destinationForAttcak;

                currentCharControl.SetCD(typeof(StimulatorTargetingController), 4);
                currentCharControl.DeltaAP(-2);

                stimulatorAbility.initiated = true;

                battleManager.eventQueue.Add(stimulatorAbility);

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

            targetSelected = false;
        }
    }
}