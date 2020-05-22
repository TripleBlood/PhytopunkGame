using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class TargetingController : MonoBehaviour
    {
        public Ability ability;
        public BattleManager battleManager;
        public Map map;
        public Canvas mainUI;
        public bool initiated = false;

        public static int cost;

        /// <summary>
        /// Method should create new instance of Event/Ability class and send it to BattleManager
        /// </summary>
        public abstract void ConfirmTarget();

        public BattleController currentBattleController;

        public abstract void Construct(BattleManager battleManager, 
            Map map, BattleController currentBattleController);
        public abstract void EndTargeting();

    }
}

