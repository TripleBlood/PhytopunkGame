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
                            EndTargeting();
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
                    "Zapps target, dealing 20 damage and applying \"Shocked\" status for 2 turns", 2, 0, 2, 2, true);
                overloadAbility.battleManager = battleManager;
                
                // Some shit
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Deselect();
        }

        public override void Construct(BattleManager battleManager, Map map, BattleController currentBattleController)
        {
            this.battleManager = battleManager;
            this.map = map;
            this.currentBattleController = currentBattleController;
            mainUI = battleManager.mainUI;

            currentCharControl = (CharacterBattleController) currentBattleController;
            _currentCharacterDataComponent = currentCharControl.characterDataComponent;
        }

        public override void EndTargeting()
        {
            Destroy(courser);
            Deselect();
            
            // SwapTargetingController!!!

            Destroy(this);
        }

        public void Deselect()
        {
            Destroy(confirmButtonGameObject);

            targetSelected = false;
        }
    }
}