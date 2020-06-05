using System;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SplitshotTargetingController : TargetingController
    {
        private GameObject courser;

        private List<CharacterBattleController> targetForAttack = new List<CharacterBattleController>();
        private CharacterBattleController currentCharControl;
        private CharacterDataComponent _currentCharacterDataComponent;

        private List<Vector3> originForAttack = new List<Vector3>();
        private List<Vector3> destinationForAttack = new List<Vector3>();

        private GameObject confirmButtonGameObject;
        private Button confirmButton;

        private float timeHoldingBtn;

        private int targetSelected;

        private List<GameObject> additionalRingCoursers = new List<GameObject>();
        private List<GameObject> additionalNumberCoursers = new List<GameObject>();

        public GameObject middlePointMapCourserPrefab;

        private void Awake()
        {
            // Assets/Resources/MapCoursers/MoveMiddlePoint.prefab
            var middlePointMapCourser = Resources.Load("MapCoursers/MoveMiddlePoint");
            middlePointMapCourserPrefab = middlePointMapCourser as GameObject;

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

                if (targetSelected > 0)
                {
                    if (targetSelected < 3)
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
                                    CharacterBattleController target =
                                        //targetForAttack =
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
                                                target.characterDataComponent.position,
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
                                                        .GetComponent<CharacterBattleController>() == target)
                                                {
                                                    targetSelected++;

                                                    targetForAttack.Add(target);
                                                    originForAttack.Add(origin);
                                                    destinationForAttack.Add(destination);

                                                    Tile curTile = target.characterDataComponent.position;
                                                    Vector3 curTilePoint =
                                                        map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);

                                                    additionalRingCoursers.Add(Instantiate(middlePointMapCourserPrefab,
                                                        curTilePoint, Quaternion.identity));

                                                    Vector3 vect =
                                                        curTilePoint + new Vector3(0, 0.7f * targetSelected, 0);
                                                    Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

                                                    GameObject text =
                                                        Instantiate(
                                                            Resources.Load("UIElements/MoveStep")) as GameObject;
                                                    text.transform.SetParent(mainUI.transform, false);
                                                    text.GetComponent<UIinSpace>().Initiate(vect);
                                                    text.GetComponent<UIinSpace>()
                                                        .ChangeText(targetSelected.ToString());
                                                    additionalNumberCoursers.Add(text);


                                                    if (targetSelected == 3)
                                                    {
                                                        confirmButtonGameObject =
                                                            Instantiate(Resources.Load("UIElements/ConfirmBtn")) as
                                                                GameObject;
                                                        confirmButtonGameObject.transform.SetParent(mainUI.transform,
                                                            false);
                                                        confirmButton = confirmButtonGameObject.GetComponent<Button>();
                                                        confirmButton.onClick.AddListener(ConfirmTarget);
                                                        confirmButtonGameObject.GetComponent<UIinSpace>()
                                                            .Initiate(vect, new Vector2(20, 45));
                                                    }

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
                    }


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
                                CharacterBattleController target =
                                    //targetForAttack =
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
                                        TargetingUtils.GetPointsOrigin(map, target.characterDataComponent.position,
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
                                                    .GetComponent<CharacterBattleController>() == target)
                                            {
                                                targetSelected = 1;

                                                targetForAttack.Add(target);
                                                originForAttack.Add(origin);
                                                destinationForAttack.Add(destination);

                                                // originForAttack = origin;
                                                // destinationForAttcak = destination;

                                                Tile curTile = target.characterDataComponent.position;
                                                Vector3 curTilePoint =
                                                    map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);

                                                additionalRingCoursers.Add(Instantiate(middlePointMapCourserPrefab,
                                                    curTilePoint, Quaternion.identity));

                                                Vector3 vect = curTilePoint + new Vector3(0, 0.7f, 0);
                                                Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

                                                GameObject text =
                                                    Instantiate(Resources.Load("UIElements/MoveStep")) as GameObject;
                                                text.transform.SetParent(mainUI.transform, false);
                                                text.GetComponent<UIinSpace>().Initiate(vect);
                                                text.GetComponent<UIinSpace>().ChangeText(targetSelected.ToString());
                                                additionalNumberCoursers.Add(text);

                                                // confirmButtonGameObject =
                                                //     Instantiate(Resources.Load("UIElements/ConfirmBtn")) as GameObject;
                                                // confirmButtonGameObject.transform.SetParent(mainUI.transform, false);
                                                // confirmButton = confirmButtonGameObject.GetComponent<Button>();
                                                // confirmButton.onClick.AddListener(ConfirmTarget);
                                                // confirmButtonGameObject.GetComponent<UIinSpace>()
                                                //     .Initiate(vect, new Vector2(20, 45));
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
            //Rewrite
            Type type = typeof(SplitShotAbility);
            SplitShotAbility splitshotAbility = (SplitShotAbility) battleManager.gameObject.AddComponent(type);

            try
            {
                splitshotAbility.SetProperties("Splitshot",
                    "Performs 3 basic attacks", 4, 0, 4, 4, true);
                splitshotAbility.battleManager = battleManager;
                splitshotAbility.targetBC = targetForAttack;
                splitshotAbility.attackerBC = currentCharControl;
                splitshotAbility.origin = originForAttack;
                splitshotAbility.destination = destinationForAttack;
                
                Debug.Log(originForAttack.Count);

                currentCharControl.SetCD(typeof(SplitshotTargetingController), 4);
                currentCharControl.DeltaAP(-4);

                splitshotAbility.initiated = true;

                battleManager.eventQueue.Add(splitshotAbility);

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

            currentCharControl.DeltaAPRed(4);
            // currentCharControl.DeltaEPRed(2);

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
            if (targetSelected == 3)
            {
                Destroy(confirmButtonGameObject);
            }

            targetSelected = 0;


            foreach (GameObject additionalNumberCourser in additionalNumberCoursers)
            {
                Destroy(additionalNumberCourser);
            }

            foreach (GameObject additionalRingCourser in additionalRingCoursers)
            {
                Destroy(additionalRingCourser);
            }
            
            additionalNumberCoursers = new List<GameObject>();
            additionalRingCoursers = new List<GameObject>();

            targetForAttack = new List<CharacterBattleController>();
            originForAttack = new List<Vector3>();
            destinationForAttack = new List<Vector3>();
        }
    }
}