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
        private Map map;
        List<bool> list = new List<bool>();
        private bool initiated = false;


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

                        StartCoroutine(MoveObject(character,
                            map.GetCoordByTileIndexes(currentTile.x, currentTile.z, currentTile.y), 50, i));
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
                    }
                }
            }

            throw new NotImplementedException();
        }

        IEnumerator MoveObject(CharacterBattleController characterBattleController, Vector3 destination, float speed,
            int index)
        {
            float step = speed * Time.deltaTime;
            while ((characterBattleController.gameObject.transform.position - destination).magnitude <= 0.005f)
            {
                characterBattleController.gameObject.transform.position =
                    Vector3.MoveTowards(characterBattleController.gameObject.transform.position, destination, step);
                yield return null;
            }

            characterBattleController.gameObject.transform.position = destination;
            list[index] = true;
        }

        public override void EndEvent(out bool busy)
        {
            busy = false;
        }
    }
}