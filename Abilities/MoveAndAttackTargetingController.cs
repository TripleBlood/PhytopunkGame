using System;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveAndAttackTargetingController : TargetingController
    {
        //Очень большой костыль!!!
        public bool initiated = false;

        public bool state = true;
        public GameObject courser;
        public CharacterDataComponent caster;

        private bool moveConfirm;
        private bool attackConfirm;

        private List<Tile> path = new List<Tile>();

        private List<GameObject> additionalRingCoursers = new List<GameObject>();

        public GameObject middlePointMapCourserPrefab;

        private CharacterBattleController currentCharControl;
        private CharacterDataComponent _currentCharacterDataComponent;

        private float timeHoldingBtn;

        private int APcostForMovement;


        public override void Construct(BattleManager battleManager, Map map, BattleController currentBattleController)
        {
            this.battleManager = battleManager;
            this.map = map;
            this.currentBattleController = currentBattleController;

            currentCharControl = (CharacterBattleController) currentBattleController;
            _currentCharacterDataComponent = currentCharControl.characterDataComponent;

            initiated = true;
        }

        private void Awake()
        {
            // Assets/Resources/MapCoursers/MoveMiddlePoint.prefab
            var middlePointMapCourser = Resources.Load("MapCoursers/MoveMiddlePoint");
            middlePointMapCourserPrefab = middlePointMapCourser as GameObject;

            courser = Instantiate(Resources.Load("MapCoursers/mapTileCourser") as GameObject);

            // try
            // {
            //     currentCharControl = currentBattleController as CharacterBattleController;
            //     _currentCharacterDataComponent = currentCharControl.characterDataComponent;
            // }
            // catch (NullReferenceException e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }
            // TODO: check for potential errors. Remove from here! This should be in BattleManager!


            // middlePointMapCourserPrefab = Resources.Load("Assets/MapCoursers/MoveMiddlePoint.prefab", typeof(GameObject));
            //Debug.Log(middlePointMapCourser.name);
        }

        private void Start()
        {
            // middlePointMapCourserPrefab = Resources.Load("Assets/MapCoursers/MoveMiddlePoint.prefab", typeof(GameObject));
            // Debug.Log(middlePointMapCourserPrefab.name);
            timeHoldingBtn = 0.0f;
        }

        private void Update()
        {
            if (initiated)
            {
                if (moveConfirm)
                {
                }
                else if (attackConfirm)
                {
                }
                else if (state)
                {
                    // TODO: Correct mask!
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

                    if (Input.GetMouseButtonUp(0))
                    {
                        //Placeholder DELETE IT LATER!
                        currentBattleController = battleManager.currentBattleController;
                        currentCharControl = (CharacterBattleController) currentBattleController;
                        _currentCharacterDataComponent = currentCharControl.characterDataComponent;

                        Map mapp = battleManager.map;
                        bool testf = battleManager.map.testField;
                        map.testField = false;
                        testf = battleManager.map.testField;
                        Debug.Log(map.testField + " " + battleManager.map.testField);
                        RaycastHit hitLMBInfo = new RaycastHit();
                        bool hitLMB = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitLMBInfo,
                            Single.PositiveInfinity, mask);
                        if (hitLMB)
                        {
                            if (hitLMBInfo.collider.gameObject.tag.Equals("Floor"))
                            {
                                additionalRingCoursers.Add(Instantiate(middlePointMapCourserPrefab,
                                    map.GetCourserPosition(hitLMBInfo.point), Quaternion.identity));

                                path = MapUtils.FindPath(map, _currentCharacterDataComponent.position,
                                    map.GetTileByVectorPoint(hitLMBInfo.point));
                                moveConfirm = true;
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
                    if (timeHoldingBtn <= 0.5f)
                    {
                        foreach (GameObject gameObjectt in additionalRingCoursers)
                        {
                            Destroy(gameObjectt);
                        }
                    }

                    timeHoldingBtn = 0;
                }
            }
            // throw new NotImplementedException();
        }

        public override void ConfirmTarget()
        {
            APcostForMovement = (int) Math.Ceiling((double) path.Count / (double) _currentCharacterDataComponent.speed);
            if (moveConfirm)
            {
                battleManager.eventQueue.Enqueue(new MoveAbility(path, APcostForMovement));
            }

            // throw new System.NotImplementedException();
        }
    }
}