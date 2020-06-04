using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Models;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public bool busy;

    public Canvas mainUI;

    public List<BattleController> battleControllersQueue;
    public BattleController currentBattleController;
    private bool inTurn;

    public TargetingController currentTargetController;

    public GameObject mapTileCourser;

    public string message;

    public int floors;
    public int width;
    public int length;

    public float xOffset;
    public float zOffset;
    public float yOffset;

    public Map map;

    public List<EventBattle> eventQueue = new List<EventBattle>();
    private EventBattle currentEvent;


    List<GameObject> abilityBtnList = new List<GameObject>();
    List<GameObject> apPanelsBlack = new List<GameObject>();
    List<GameObject> apPanelsActive = new List<GameObject>();
    List<GameObject> epPanelsBlack = new List<GameObject>();
    List<GameObject> epPanelsActive = new List<GameObject>();
    GameObject hp;
    private GameObject effectPanel;

    public List<GameObject> AbilityBtnList => abilityBtnList;
    public List<GameObject> ApPanelsBlack => apPanelsBlack;
    public List<GameObject> ApPanelsActive => apPanelsActive;
    public List<GameObject> EpPanelsBlack => epPanelsBlack;
    public List<GameObject> EpPanelsActive => epPanelsActive;
    public GameObject Hp => hp;
    public GameObject EffectPanel => effectPanel;

    public List<GameObject> lightSources;

    private void Awake()
    {
        map = new Map(floors, width, length, xOffset, zOffset, yOffset);

        abilityBtnList = GetAbilityBtn();
        apPanelsBlack = GetAPPointsBlack();
        apPanelsActive = GetAPPointsActive(apPanelsBlack);
        epPanelsBlack = GetEPPointsBlack();
        epPanelsActive = GetEPPointsActive(epPanelsBlack);
        hp = GetHPBar();
        effectPanel = GetEffectPanel();

        // b
        // Это гениально, оно работает, но я ваще не ебу как...
        // Type type = typeof(MoveAndAttackTargetingController);
        // currentTargetController = (TargetingController)gameObject.AddComponent(type);
    }


    /// <summary>
    /// Builds queue from given characters and random events, stored here, and organized here?
    /// Climate changes, random events, dialogs etc.
    /// </summary>
    /// <param name="characters"></param>
    public void BuildQueue(List<CharacterBattleController> characters)
    {
        // Implement this method if queue should be built in specific way,
        // but now here's placeholder
        // To each BattleController should be binned active instance of BattleManager

        Type type = typeof(InitiatePositions);
        InitiatePositions initiateEvent = (InitiatePositions) gameObject.AddComponent(type);
        busy = true;
        initiateEvent.battleManager = this;
        initiateEvent.SetCharacters(characters);
        initiateEvent.map = map;
        //
        // battleControllersQueue = (List<BattleController>)characters;

        foreach (CharacterBattleController character in characters)
        {
            battleControllersQueue.Add(character);
        }

        foreach (BattleController battleController in battleControllersQueue)
        {
            battleController.battleManager = this;
            battleController.map = map;

            if (battleController.GetType() == typeof(CharacterBattleController))
            {
                //Debug.Log(battleController.gameObject.name);
            }
        }

        initiateEvent.active = true;
        map.UpdateIllumination(lightSources);
        
        // StartTurn();
    }

    /// <summary>
    /// Evoked by current active BattleController...
    /// Not sure if this is correct way to do this operation
    /// </summary>
    public void EndTurnBM()
    {
        currentBattleController.EndTurnBC();
        currentBattleController.enabled = false;
        battleControllersQueue.Add(currentBattleController);
        battleControllersQueue.RemoveAt(0);
        inTurn = false;
        // StartTurn();
    }

    void StartTurn()
    {
        currentBattleController = battleControllersQueue[0];
        currentBattleController.enabled = true;

        // This should be in BattleController?
        // currentTargetController.currentBattleController = currentBattleController;
        // currentTargetController.Construct(this, map, currentBattleController);

        currentBattleController.BeginTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!busy)
        {
            if (eventQueue.Count > 0)
            {
                currentEvent = eventQueue.First();
                if (eventQueue.Count == 1)
                {
                    eventQueue = new List<EventBattle>();
                }
                else
                {
                    eventQueue = eventQueue.GetRange(1, eventQueue.Count - 1);
                }

                if (currentEvent.initiated)
                {
                    currentEvent.active = true;
                    busy = true;
                }
            }
            else
            {
                if (!inTurn)
                {
                    inTurn = true;
                    StartTurn();
                }   
            }
        }
    }

    public List<GameObject> GetAbilityBtn()
    {
        GameObject bottomUICanvas = mainUI.transform.Find("BottomUI").gameObject;
        GameObject abilityPanel = bottomUICanvas.transform.Find("AbilityPanel").gameObject;
        // Debug.Log(abilityPanel.GetComponent<Image>().sprite.name);
        
        List<GameObject> btns = new List<GameObject>();

        for (int i = 1; i < 12; i++)
        {
            try
            {
                btns.Add(abilityPanel.transform.Find(i.ToString()).gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // foreach (GameObject btn in btns)
        // {
        //     Debug.Log(btn.GetComponent<Image>().sprite.name + " " + btn.name);
        // }
        
        return btns;
    }

    public List<GameObject> GetAPPointsBlack()
    {
        GameObject bottomUICanvas = mainUI.transform.Find("BottomUI").gameObject;
        GameObject apPanel = bottomUICanvas.transform.Find("APPanel").gameObject;
        
        List<GameObject> panels = new List<GameObject>();

        for (int i = 1; i < 11; i++)
        {
            try
            {
                panels.Add(apPanel.transform.Find(i.ToString()).gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        // panels[3].SetActive(false);
        
        return panels;
    }
    
    public List<GameObject> GetAPPointsActive(List<GameObject> blackPanels)
    {
        List<GameObject> panels = new List<GameObject>();
        foreach (GameObject blackPanel in blackPanels)
        {
            try
            {
                panels.Add(blackPanel.transform.Find("Panel").gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // GameObject test = panels[6];
        // test.GetComponent<Image>().color = Color.cyan;

        return panels;
    }
    
    public List<GameObject> GetEPPointsBlack()
    {
        GameObject bottomUICanvas = mainUI.transform.Find("BottomUI").gameObject;
        GameObject epPanel = bottomUICanvas.transform.Find("EPPanel").gameObject;
        
        List<GameObject> panels = new List<GameObject>();

        for (int i = 1; i < 6; i++)
        {
            try
            {
                panels.Add(epPanel.transform.Find(i.ToString()).gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        // panels[3].SetActive(false);
        
        return panels;
    }
    
    public List<GameObject> GetEPPointsActive(List<GameObject> blackPanels)
    {
        List<GameObject> panels = new List<GameObject>();
        foreach (GameObject blackPanel in blackPanels)
        {
            try
            {
                panels.Add(blackPanel.transform.Find("Panel").gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // GameObject test = panels[6];
        // test.GetComponent<Image>().color = Color.cyan;

        return panels;
    }

    public GameObject GetHPBar()
    {
        GameObject bottomUICanvas = mainUI.transform.Find("BottomUI").gameObject;
        GameObject hpPanel = bottomUICanvas.transform.Find("HPPanel").gameObject;
        return hpPanel.transform.Find("HP").gameObject;
    }
    
    public bool TrySwapTargetingController(int index)
    {
        currentBattleController.SwapTargeting(index);
        return true;
    }

    public GameObject GetEffectPanel()
    {
        GameObject bottomUICanvas = mainUI.transform.Find("BottomUI").gameObject;
        return bottomUICanvas.transform.Find("Effects").gameObject;
    }
}