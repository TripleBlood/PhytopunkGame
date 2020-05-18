using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public abstract class BattleController  : MonoBehaviour
{
    public BattleManager battleManager;
    // This is very strange thing I'm doing
    // I want to put into BattleQueue different type controllers
    // for example: basic Character controller, end of turn cycle,
    // different climatic changes and random events, that will control stage, until endTurn()
    // will be evoked, and it will be disabled (for characters. To prevent control issues), or destroyed
    
    // Behavior Delegation at its finest, lul

    /// <summary>
    /// End of turn Behavior
    /// I suppose, that disabling/destruction should be here, but final thing should be executed
    /// in additional coroutine.
    /// </summary>
    public abstract void EndTurnBC();

    /// <summary>
    /// Start of turn behavior
    /// </summary>
    public abstract void BeginTurn();

    /// <summary>
    /// Current Controller for ui, Targeting Systems. NEed to block Input access if not User's unit turn
    /// </summary>
    public TargetingController currentTargetingControl;
    
}
