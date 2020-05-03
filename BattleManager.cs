using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private bool busy;
    
    
    public List<BattleController> battleControllersQueue;
    private BattleController currentBattleController;

    public string message;

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
        }
        StartTurn();
    }

    /// <summary>
    /// Evoked by player, or controllable character/
    /// By pressing "EndTurn" button or stuff...
    /// </summary>
    public void EndTurnActive()
    {
        
    }

    /// <summary>
    /// Evoked by current active BattleController...
    /// Not sure if this is correct way to do this operation
    /// </summary>
    public void EndTurnPassive()
    {
        currentBattleController.enabled = false;
        battleControllersQueue.Add(currentBattleController);
        battleControllersQueue.RemoveAt(0);
        StartTurn();
    }

    void StartTurn()
    {
        currentBattleController = battleControllersQueue[0];
        currentBattleController.enabled = true;
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
