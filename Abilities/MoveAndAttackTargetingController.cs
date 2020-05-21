using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DefaultNamespace.Utils;
using Models;
using Newtonsoft.Json.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        private List<Tile> curPath = new List<Tile>();
        private Tile curTile;

        private List<GameObject> additionalRingCoursers = new List<GameObject>();
        private List<GameObject> additionalNumberCoursers = new List<GameObject>();

        public GameObject middlePointMapCourserPrefab;

        private CharacterBattleController currentCharControl;
        private CharacterDataComponent _currentCharacterDataComponent;

        private CharacterBattleController targetForAttack;
        private Vector3 originForAttack;
        private Vector3 destinationForAttcak;

        private GameObject confirmButtonGameObject;
        private Button confirmButton;

        private float timeHoldingBtn;

        private int APcostForMovement;
        private int maxPathLength = 1;


        public override void Construct(BattleManager battleManager, Map map, BattleController currentBattleController)
        {
            this.battleManager = battleManager;
            this.map = map;
            this.currentBattleController = currentBattleController;
            mainUI = battleManager.mainUI;

            currentCharControl = (CharacterBattleController) currentBattleController;
            _currentCharacterDataComponent = currentCharControl.characterDataComponent;

            maxPathLength = _currentCharacterDataComponent.ap *
                            (_currentCharacterDataComponent.speed + _currentCharacterDataComponent.speedModifier);

            initiated = true;
        }

        private void Awake()
        {
            // Assets/Resources/MapCoursers/MoveMiddlePoint.prefab
            var middlePointMapCourser = Resources.Load("MapCoursers/MoveMiddlePoint");
            middlePointMapCourserPrefab = middlePointMapCourser as GameObject;

            courser = Instantiate(Resources.Load("MapCoursers/mapTileCourser") as GameObject);
        }

        private void Start()
        {
            timeHoldingBtn = 0.0f;
        }

        private void Update()
        {
            if (initiated)
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

                if (moveConfirm)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (EventSystem.current.IsPointerOverGameObject()) return;

                        if (path.Count < maxPathLength)
                        {
                            RaycastHit hitLMBInfo = new RaycastHit();
                            bool hitLMB = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                                out hitLMBInfo,
                                Single.PositiveInfinity, mask);
                            if (hitLMB)
                            {
                                if (hitLMBInfo.collider.gameObject.tag.Equals("Floor"))
                                {
                                    curPath = MapUtils.FindPath(map, path.Last(),
                                        map.GetTileByVectorPoint(hitLMBInfo.point));
                                    if (curPath.Count + path.Count > maxPathLength)
                                    {
                                        curPath = curPath.GetRange(0, maxPathLength - path.Count);
                                    }

                                    curTile = curPath.Last();
                                    Vector3 pathPoint = map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);

                                    additionalRingCoursers.Add(Instantiate(middlePointMapCourserPrefab,
                                        pathPoint, Quaternion.identity));
                                    Vector3 vect = pathPoint + new Vector3(0, 0.7f, 0);
                                    Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

                                    GameObject text = Instantiate(Resources.Load("UIElements/MoveStep")) as GameObject;
                                    text.transform.SetParent(mainUI.transform, false);
                                    text.GetComponent<UIinSpace>().Initiate(vect);
                                    text.GetComponent<UIinSpace>().ChangeText(additionalRingCoursers.Count.ToString());
                                    additionalNumberCoursers.Add(text);

                                    confirmButtonGameObject.GetComponent<UIinSpace>()
                                        .Initiate(vect, new Vector2(20, 45));

                                    path.AddRange(curPath);
                                }
                            }
                        }
                        else
                        {
                            // TODO: Make notification here!
                        }
                    }
                }
                else if (attackConfirm)
                {
                }
                else if (state)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        //need this shit to prevent clicking Under UI
                        if (EventSystem.current.IsPointerOverGameObject()) return;

                        RaycastHit hitLMBInfo = new RaycastHit();
                        bool hitLMB = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                            out hitLMBInfo,
                            Single.PositiveInfinity, mask);
                        if (hitLMB)
                        {
                            if (hitLMBInfo.collider.gameObject.tag.Equals("Floor"))
                            {
                                curPath = MapUtils.FindPath(map, _currentCharacterDataComponent.position,
                                    map.GetTileByVectorPoint(hitLMBInfo.point));
                                if (curPath.Count > maxPathLength)
                                {
                                    curPath = curPath.GetRange(0, maxPathLength);
                                }

                                curTile = curPath.Last();
                                Vector3 pathPoint = map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);

                                additionalRingCoursers.Add(Instantiate(middlePointMapCourserPrefab,
                                    pathPoint, Quaternion.identity));
                                Vector3 vect = pathPoint + new Vector3(0, 0.7f, 0);
                                Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

                                GameObject text = Instantiate(Resources.Load("UIElements/MoveStep")) as GameObject;
                                text.transform.SetParent(mainUI.transform, false);
                                text.GetComponent<UIinSpace>().Initiate(vect);
                                text.GetComponent<UIinSpace>().ChangeText("1");
                                additionalNumberCoursers.Add(text);

                                confirmButtonGameObject =
                                    Instantiate(Resources.Load("UIElements/MoveBtn")) as GameObject;
                                confirmButtonGameObject.transform.SetParent(mainUI.transform, false);
                                confirmButton = confirmButtonGameObject.GetComponent<Button>();
                                confirmButton.onClick.AddListener(ConfirmTarget);
                                confirmButtonGameObject.GetComponent<UIinSpace>()
                                    .Initiate(vect, new Vector2(20, 45));

                                path.AddRange(curPath);

                                moveConfirm = true;

                                // Button button = Instantiate() as Button;
                                // button.onClick.AddListener(ConfirmTarget);
                            }
                            Component bc;
                            
                            if (hitLMBInfo.collider.TryGetComponent(typeof(BattleController), out bc))
                            {
                                
                                //Debug.Log("WOW! " + hitLMBInfo.collider.TryGetComponent(typeof(BattleController), out bc));
                                
                                
                                targetForAttack =
                                    hitLMBInfo.collider.gameObject.GetComponent<CharacterBattleController>();

                                Vector3 attackAlignment =
                                    hitLMBInfo.collider.gameObject.transform.position - gameObject.transform.position;

                                float angle = Vector3.SignedAngle(attackAlignment, Vector3.forward, Vector3.up);
                                
                                
                                Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + attackAlignment, Color.red, 3.5f);
                                Debug.DrawLine(targetForAttack.gameObject.transform.position, targetForAttack.gameObject.transform.position + Vector3.forward, Color.blue, 3.5f);
                                Debug.DrawLine(targetForAttack.gameObject.transform.position, targetForAttack.gameObject.transform.position + Vector3.right, Color.yellow, 3.5f);
                                Debug.DrawLine(targetForAttack.gameObject.transform.position, targetForAttack.gameObject.transform.position + Vector3.back, Color.red, 3.5f);
                                Debug.DrawLine(targetForAttack.gameObject.transform.position, targetForAttack.gameObject.transform.position + Vector3.left, Color.black, 3.5f);
                                
                                int adjIndex = GetAdjIndexByAngle(angle);
                                int opponentAdjIndex = (adjIndex + 2) % 4;

                                //Debug.Log("Angle = " + angle + " - " + adjIndex + ", " + opponentAdjIndex);
                                
                                List<Vector3> attackOrigin =
                                    GetPointsOrigin(_currentCharacterDataComponent.position, adjIndex);
                                List<Vector3> attackDestination =
                                    GetPointsOrigin(targetForAttack.characterDataComponent.position, opponentAdjIndex);

                                foreach (Vector3 origin in attackOrigin)
                                {
                                    foreach (Vector3 destination in attackDestination)
                                    {
                                        // int maskForAttack = ~(1 << 9);
                                        RaycastHit hitAtckInfo = new RaycastHit();
                                        bool hitAtck = Physics.Raycast(origin, destination - origin, out hitAtckInfo,
                                            (destination - origin).magnitude);
                                        
                                        Debug.DrawLine(origin, destination, Color.white, 5);

                                        if (!hitAtck || hitAtckInfo.collider.gameObject.GetComponent<CharacterBattleController>() == targetForAttack)
                                        {
                                            originForAttack = origin;
                                            destinationForAttcak = destination;

                                            curTile = targetForAttack.characterDataComponent.position;
                                            
                                            Vector3 pathPoint = map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);
                                            Vector3 vect = pathPoint + new Vector3(0, 0.7f, 0);
                                            Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);
                                            
                                            confirmButtonGameObject =
                                                Instantiate(Resources.Load("UIElements/AttackBtn")) as GameObject;
                                            confirmButtonGameObject.transform.SetParent(mainUI.transform, false);
                                            confirmButton = confirmButtonGameObject.GetComponent<Button>();
                                            confirmButton.onClick.AddListener(ConfirmTarget);
                                            confirmButtonGameObject.GetComponent<UIinSpace>()
                                                .Initiate(vect, new Vector2(20, 45));
                                            
                                            attackConfirm = true;
                                            
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
                        Deselect();
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
                Type type = typeof(MoveAbility);
                MoveAbility moveAbility = (MoveAbility)battleManager.gameObject.AddComponent(type);

                try
                {
                    moveAbility.SetProperties("Move", "MoveObject", APcostForMovement, 0, 0, 0, true);
                    moveAbility.path = path;
                    moveAbility.movingObject = this.gameObject;
                    moveAbility.battleManager = battleManager;

                    moveAbility.initiated = true;
                    
                    battleManager.eventQueue.Add(moveAbility);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e); 
                    Debug.Log("Fuck, this doesn't work!");
                }
            }

            Deselect();

            // throw new System.NotImplementedException();
        }

        public void Deselect()
        {
            foreach (GameObject gameObjectt in additionalRingCoursers)
            {
                Destroy(gameObjectt);
            }

            foreach (GameObject numberCourser in additionalNumberCoursers)
            {
                Destroy(numberCourser);
            }

            Destroy(confirmButtonGameObject);

            curPath = new List<Tile>();
            path = new List<Tile>();
            additionalNumberCoursers = new List<GameObject>();
            additionalRingCoursers = new List<GameObject>();

            moveConfirm = false;
            attackConfirm = false;
        }

        public int GetAdjIndexByAngle(float angle)
        {
            if (Math.Abs(angle) < 45)
            {
                return 2;
            }
            else if (Math.Abs(angle) > 135)
            {
                return 0;
            }
            else if (angle > 0)
            {
                return 3;
            }
            else
            {
                return 1;
            }
        }

        private Vector3[][] pointsAdd = new Vector3[][]
        {
            new Vector3[3]
            {
                new Vector3(0, 1.5f, 0.35f),
                new Vector3(0.65f, 1, 0.35f),
                new Vector3(-0.65f, 1, 0.35f)
            },
            new Vector3[3]
            {
                new Vector3(0.35f, 1.5f, 0),
                new Vector3(0.35f, 1, 0.65f),
                new Vector3(0.35f, 1, -0.65f)
            },
            new Vector3[3]
            {
                new Vector3(0, 1.5f, -0.35f),
                new Vector3(0.65f, 1, -0.35f),
                new Vector3(-0.65f, 1, -0.35f)
            },
            new Vector3[3]
            {
                new Vector3(-0.35f, 1.5f, 0),
                new Vector3(-0.35f, 1, 0.65f),
                new Vector3(-0.35f, 1, -0.65f)
            }
        };

        List<Vector3> GetPointsOrigin(Tile tile, int index)
        {
            
            List<Vector3> result = new List<Vector3>();
            if (index > 3 || index < 0)
            {
                return new List<Vector3>();
            }

            int cover = tile.CoverArray[index];
            if (cover == 1)
            {
                result = pointsAdd[index].ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    result[i] += map.GetCoordByTileIndexes(tile.x, tile.z, tile.y);
                }
                
                return result;
            }

            if (cover == 2)
            {
                result = pointsAdd[index].ToList().GetRange(1, 2);
                
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] += map.GetCoordByTileIndexes(tile.x, tile.z, tile.y);
                }

                return result;
            }

            return new List<Vector3>
            {
                map.GetCoordByTileIndexes(tile.x, tile.z, tile.y) + new Vector3(0, 1.5f, 0)
            };
        }


        public override void EndTargeting()
        {
            Destroy(courser);
            Deselect();
            foreach (GameObject VARIABLE in additionalRingCoursers)
            {
                Destroy(VARIABLE);
            }
            Destroy(this);
        }
    }
}