using System;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class AdrenalineTargetingController : TargetingController
    {
        private CharacterBattleController currentCharControl;
        private CharacterDataComponent _currentCharacterDataComponent;
        
        private GameObject confirmButtonGameObject;
        private Button confirmButton;

        private float timeHoldingBtn;

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                timeHoldingBtn += Time.deltaTime;
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (timeHoldingBtn <= 0.3f)
                {
                    // SwapTargetingController
                    // EndTargeting();
                    currentCharControl.SwapTargeting(0);
                }
                timeHoldingBtn = 0;
            }
        }

        public override void ConfirmTarget()
        {
            try
            {
                currentCharControl.DeltaAP(2);
                currentCharControl.SetCD(typeof(AdrenalineTargetingController) , 3);
                AdrenalineEffect adrenalineEffect = new AdrenalineEffect(currentCharControl);
                
                currentCharControl.TryAddEffect(adrenalineEffect, currentCharControl.characterDataComponent.effects);
                // StartCoroutine(adrenalineEffect.ApplyEffect(_currentCharacterDataComponent.effects));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Deselect();
            currentCharControl.SwapTargeting(0);
        }

        public override void Construct(BattleManager battleManager, Map map, BattleController currentBattleController)
        {
            this.battleManager = battleManager;
            this.map = map;
            this.currentBattleController = currentBattleController;
            mainUI = battleManager.mainUI;

            currentCharControl = (CharacterBattleController) currentBattleController;
            _currentCharacterDataComponent = currentCharControl.characterDataComponent;
            
            Tile curTile = _currentCharacterDataComponent.position;

            Vector3 curTilePoint =
                map.GetCoordByTileIndexes(curTile.x, curTile.z, curTile.y);
            Vector3 vect = curTilePoint + new Vector3(0, 0.7f, 0);
            Vector2 vect2 = Camera.main.WorldToScreenPoint(vect);

            confirmButtonGameObject =
                Instantiate(Resources.Load("UIElements/ConfirmBtn")) as GameObject;
            confirmButtonGameObject.transform.SetParent(mainUI.transform, false);
            confirmButton = confirmButtonGameObject.GetComponent<Button>();
            confirmButton.onClick.AddListener(ConfirmTarget);
            confirmButtonGameObject.GetComponent<UIinSpace>()
                .Initiate(vect, new Vector2(20, 45));
        }

        public override void EndTargeting()
        {
            initiated = false;
            Destroy(confirmButtonGameObject);
            Destroy(this);
        }
    }
}