using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DefaultNamespace
{
    public class CharacterBattleController : BattleController
    {
        private void Start()
        {
            // throw new NotImplementedException();
        }

        void Update()
        {
            Debug.Log("I am " + gameObject.name + "!");

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                this.battleManager.EndTurnPassive();
                
            }
        }

        public override void EndTurnMethod()
        {
            // throw new NotImplementedException();
            
            
        }
        
    }
}