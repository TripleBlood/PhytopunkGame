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


    private void Awake()
    {
        map = new Map(floors, width, length, xOffset, zOffset, yOffset);

        abilityBtnList = GetAbilityBtn();

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


        StartTurn();
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
        StartTurn();
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

    // Start is called before the first frame update
    void Start()
    {
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
        }
    }

    public List<GameObject> GetAbilityBtn()
    {
        GameObject abilityPanel = mainUI.transform.Find("AbilityPanel").gameObject;
        Debug.Log(abilityPanel.GetComponent<Image>().sprite.name);
        
        List<GameObject> btns = new List<GameObject>();

        for (int i = 1; i < 11; i++)
        {
            
            btns.Add(abilityPanel.transform.Find(i.ToString()).gameObject);
        }

        foreach (GameObject btn in btns)
        {
            Debug.Log(btn.GetComponent<Image>().sprite.name + " " + btn.name);
        }
        
        return btns;

    }

    public bool TrySwapTargetingController(int index)
    {
        currentBattleController.SwapTargeting(index);
        
        
            
        return true;
    }
}