using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveAbility : Ability
    {
        public List<Tile> path;
        public GameObject movingObject;
        public CharacterBattleController movingObjBattleController;


        public MoveAbility(List<Tile> path, int APcost)
        {
            this.name = "Move";
            this.description = "Move character to tile";
            this.APcost = APcost;
            this.EPcost = 0;
            this.baseCooldown = 0;
            this.cooldown = 0;
            this.targetType = true;
            this.path = path;
        }

        public override void EndEvent(out bool busy)
        {
            Destroy(this);
            busy = false;
        }

        private void Update()
        {
            if (initiated && active)
            {
                StartCoroutine(Placeholder());
                active = false;
                return;
            }
        }

        IEnumerator Placeholder()
        {
            float speed = movingObjBattleController.characterDataComponent.speed * 0.05f;
            float step = speed * Time.time;

            // foreach (Tile tile in path)
            // {
            //     Debug.Log(tile.x + ", " + tile.z + ", " + tile.y + " to —>");
            // }

            // for (int i = 0; i < 3; i++)
            // {
            //     Debug.Log("Fuck me, " + movingObject.name + " " + i);
            //     yield return new WaitForSeconds(2);
            // }

            battleManager.map.LeaveTile(movingObjBattleController.characterDataComponent.position,
                movingObjBattleController.gameObject);

            Tile endPoint = path.Last();

            battleManager.map.OccupyTile(movingObject, endPoint);

            foreach (Tile pathTilePoint in path)
            {
                Tile currentDestination = pathTilePoint;
                Vector3 destinationPoint = battleManager.map.GetCoordByTileIndexes(currentDestination.x,
                    currentDestination.z, currentDestination.y);
                while ((movingObject.transform.position -  new Vector3(0, 0.7f, 0) - destinationPoint).magnitude >= 0.0005f)
                {
                    movingObjBattleController.gameObject.transform.position =
                        Vector3.MoveTowards(movingObjBattleController.gameObject.transform.position, destinationPoint,
                            step) + new Vector3(0, 0.7f, 0);
                    yield return null;
                }
                
                movingObjBattleController.gameObject.transform.position = destinationPoint + new Vector3(0, 0.7f, 0);
            }
            EndEvent(out battleManager.busy);
        }
    }
}