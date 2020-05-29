using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Utils;
using Models;
using Priority_Queue;

namespace UnityEngine.Rendering
{
    public class InitiatePositions : EventBattle
    {
        private List<CharacterBattleController> characters;
        public Map map;
        public List<bool> list = new List<bool>();
        private bool initiated = false;

        public void SetCharacters(List<CharacterBattleController> listOfChar)
        {
            characters = listOfChar;
        }


        private void Update()
        {
            if (active)
            {
                if (!initiated)
                {
                    for (int i = 0; i < characters.Count; i++)
                    {
                        list.Add(false);

                        CharacterBattleController character = characters[i];

                        Tile currentTile;
                        currentTile = map.GetTileByVectorPoint(character.gameObject.transform.position);

                        Queue<Tile> queue = new Queue<Tile>();
                        List<Tile> adjTiles = new List<Tile>();

                        while (currentTile.occupied)
                        {
                            adjTiles =
                                MapUtils.GetAdjTilesWithOccCheck(map, currentTile.x, currentTile.z, currentTile.y);

                            foreach (Tile adjTile in adjTiles)
                            {
                                queue.Enqueue(adjTile);
                            }

                            currentTile = queue.Dequeue();
                        }

                        StartCoroutine(MoveObject(character, currentTile, 0.5f, i));

                        initiated = true;
                    }
                }
                else
                {
                    bool ready = true;
                    foreach (bool b in list)
                    {
                        ready = ready & b;
                    }

                    if (ready)
                    {
                        EndEvent(out battleManager.busy);
                        Destroy(this);
                    }
                }
            }
        }

        IEnumerator MoveObject(CharacterBattleController characterBattleController, Tile currentTile, float speed,
            int index)
        {
            Vector3 destination = map.GetCoordByTileIndexes(currentTile.x, currentTile.z, currentTile.y);
            float step = speed * Time.deltaTime;
            while ((characterBattleController.gameObject.transform.position - destination).magnitude <= 0.005f)
            {
                characterBattleController.gameObject.transform.position =
                    Vector3.MoveTowards(characterBattleController.gameObject.transform.position, destination, step) +
                    new Vector3(0, 0.7f, 0);
                yield return null;
            }

            characterBattleController.gameObject.transform.position = destination + new Vector3(0, 0.7f, 0);

            //Debug.Log(map.FindTile(currentTile.x, currentTile.z, currentTile.y).occupied);

            map.OccupyTile(characterBattleController.gameObject, currentTile);

            //Debug.Log(map.FindTile(currentTile.x, currentTile.z, currentTile.y).occupied + " " + map.FindTile(currentTile.x, currentTile.z, currentTile.y).characterOnTile.name);

            Tile tile = characterBattleController.characterDataComponent.position; 
            Debug.Log(characterBattleController.characterDataComponent.nameData + " is on tile: " + tile.x + ", "  + tile.z + ", " + tile.y);
            
            //Debug.Log(battleManager.map == map);

            list[index] = true;
        }

        public override void EndEvent(out bool busy)
        {
            busy = false;
        }
    }
}