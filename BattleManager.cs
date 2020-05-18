using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Models;
using Unity.UNetWeaver;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public bool busy;

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

    public Queue<EventBattle> eventQueue;
    

    private void Awake()
    {
        map = new Map(floors, width, length, xOffset, zOffset, yOffset);

        // Это пиздец гениально, оно работает, но я ваще не ебу как...
        Type type = typeof(MoveAndAttackTargetingController);
        currentTargetController = (TargetingController)gameObject.AddComponent(type);
        
        // Don't declare it here!
        //currentTargetController = gameObject.AddComponent(typeof(MoveAndAttackTargetingController)) as MoveAndAttackTargetingController;
        // currentTargetController.battleManager = this;
        // currentTargetController.map = map;
        // currentTargetController.currentBattleController = currentBattleController;

        // Better to instantiate within controller!
        //Leave only map and map

        //Destroy(currentTargetController);

    }


    /// <summary>
    /// Builds queue from given characters and random events, stored here, and organized here?
    /// Climate changes, random events, dialogs etc.
    /// </summary>
    /// <param name="characters"></param>
    public void BuildQueue(List<BattleController> characters)
    {
        // Implement this method if queue should be built in specific way,
        // but now here's placeholder
        // To each BattleController should be binned active instance of BattleManager

        battleControllersQueue = characters;

        foreach (BattleController battleController in battleControllersQueue)
        {
            battleController.battleManager = this;

            if (battleController.GetType() == typeof(CharacterBattleController))
            {
                Debug.Log(battleController.gameObject.name);
            }
        }
        

        StartTurn();
    }

    public void initialOccupation(BattleController battleController)
    {
        
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

        currentTargetController.currentBattleController = currentBattleController;
        currentTargetController.Construct(this, map, currentBattleController);

        currentBattleController.BeginTurn();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}