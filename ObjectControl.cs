using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Utils;
using Models;
using Unity.UNetWeaver;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectControl : MonoBehaviour
{
    private bool casting = false;
    private bool moving = false;

    private float speed = 15f;

    public bool Focused { get; set; }
    public bool Controlled { get; set; } = false;

    public Map map;

    private Vector3 destination;
    private List<int[]> currentPathList;
    public GameObject ring;

    // Start is called before the first frame update

    void Start()
    {
        destination = transform.position;

        try
        {
            Map mapp = new Map(2, 28, 27, -11, 7, 0);
            this.map = mapp;
        }
        catch (MissingComponentException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
        }

        int mask = 1 << 9;
        mask = ~mask;
        if (Input.GetMouseButton(0))
        {
            Debug.DrawLine(Camera.main.ScreenPointToRay(Input.mousePosition).origin,
                Camera.main.ScreenPointToRay(Input.mousePosition).origin +
                Camera.main.ScreenPointToRay(Input.mousePosition).direction * 100, Color.cyan, 15);

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, mask);

            Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin,
                Camera.main.ScreenPointToRay(Input.mousePosition).direction, Color.cyan);
            int[] array = new int[3];
            if (hit)
            {
                if (hitInfo.transform.gameObject.tag.Equals("Floor"))
                {
                    Vector3 point = hitInfo.point;
                    array = map.GetTileIndexesByCoords(point);
                    Debug.Log(array[0] + ", " + array[1] + ", " + array[2] + ", ");
                    string str = "";
                    foreach (var VARIABLE in map.tiles[array[0], array[1], array[2]].AdjacencyArray)
                    {
                        str += VARIABLE + ", ";
                    }

                    string logger = "North:" + map.tiles[array[0], array[1], array[2]].CoverArray[0] +
                                    " East: " + map.tiles[array[0], array[1], array[2]].CoverArray[1] +
                                    " South: " + map.tiles[array[0], array[1], array[2]].CoverArray[2] +
                                    " West: " + map.tiles[array[0], array[1], array[2]].CoverArray[3]
                        ;
                    Debug.Log(str);
                    Debug.Log(logger);
                }
            }

            // hitInfo = new RaycastHit();
            // hit = Physics.Raycast(map.GetOriginPointForTopDownVect(array[0], array[1], array[2]), Vector3.down,
            //     out hitInfo);
            // if (hit)
            // {
            //     int[] hitpointCoord = map.GetTileIndexesByCoords(hitInfo.point);
            //     int i = hitpointCoord[0];
            //     int j = hitpointCoord[1];
            //     int k = hitpointCoord[2];
            //     //Debug.Log(hitpointCoord[0] + ", " + hitpointCoord[1] + ", " + hitpointCoord[2] + ", ");
            //     if (hitInfo.transform.gameObject.tag.Equals("Floor") ||
            //         hitInfo.transform.gameObject.tag
            //             .Equals("Stairs") && //TODO: check whether i need "Stairs" tag
            //         map.GetTileIndexesByCoords(hitInfo.point).SequenceEqual(new int[] {array[0], array[1], array[2]})
            //     ) //TODO: check whether it works
            //     {
            //         Vector3 pointForAdj = hitInfo.point + map.upVect;
            //         for (int l = 0; l < 4; l++)
            //         {
            //             RaycastHit adjHitInfo;
            //             bool adjHit = Physics.Raycast(pointForAdj, map.checkVectorsArrLowCover[l],
            //                 out adjHitInfo, 3.65f);
            //             if (adjHit)
            //             {
            //                 Debug.DrawLine(pointForAdj, adjHitInfo.point, Color.magenta, 3);
            //
            //                 int[] hitPointCoord =
            //                     map.GetTileIndexesByCoords(adjHitInfo.point); //Where raycast hit
            //                 int[] adjPointCoord =
            //                     map.GetTileIndexesByAdjIndex(l + 5, i, j,
            //                         k); //Where raycast "supposed" to hit
            //
            //                 //Debug.Log(hitpointCoord[0] + ", " + hitpointCoord[1] + ", " + hitpointCoord[2] + ", ");
            //                 //Debug.Log(adjPointCoord[0] + ", " + adjPointCoord[1] + ", " + adjPointCoord[2] + ", ");
            //
            //                 if (adjHitInfo.transform.gameObject.tag.Equals("Ladder") &&
            //                     hitPointCoord.SequenceEqual(new int[] {i, j, k})
            //                 ) // If hit ladder withing tile
            //                 {
            //                     map.tiles[i, j, k].AdjacencyArray[l] = true;
            //                     map.tiles[i, j, k].AdjacencyArray[4] = true;
            //                 }
            //                 else if (adjHitInfo.transform.gameObject.tag.Equals("Floor") &&
            //                          hitPointCoord[0] == adjPointCoord[0] &&
            //                          hitPointCoord[1] == adjPointCoord[1] &&
            //                          Math.Abs(hitPointCoord[2] - adjPointCoord[2]) < 2
            //                     // Same floor or 1 floor higher/lower
            //                 ) // Not sure whether I check all possible clauses...
            //                 {
            //                     if (hitPointCoord[2] > adjPointCoord[2]) //1 floor higher
            //                     {
            //                         map.tiles[i, j, k].AdjacencyArray[l] = true;
            //                     }
            //                     else if (hitPointCoord[2] == adjPointCoord[2]) //same floor
            //                     {
            //                         map.tiles[i, j, k].AdjacencyArray[l + 5] = true;
            //                     }
            //                     else //floor lower
            //                     {
            //                         map.tiles[i, j, k].AdjacencyArray[l + 9] = true;
            //                     }
            //                 }
            //                 else if (Math.Abs(hitPointCoord[0] - adjPointCoord[0]) < 2 &&
            //                          Math.Abs(hitPointCoord[1] - adjPointCoord[1]) < 2 &&
            //                          Math.Abs(hitPointCoord[2] - adjPointCoord[2]) < 2
            //                 ) //any other cases should be covers...
            //                 {
            //                     //Check floor //TODO: FLOOR DIFFERENCE IS WORKING WRONG!
            //                     int floorDifference =
            //                         hitPointCoord[2] - k; // -1 lower, 0 same, +1 higher
            //                     int[] probablyAdjTile = new int[]
            //                         {adjPointCoord[0], adjPointCoord[1], floorDifference};
            //
            //                     RaycastHit highHitInfo;
            //                     bool hitHigh = false;
            //
            //                     hitHigh = Physics.Raycast(pointForAdj, map.checkVectorsArrHighCover[l],
            //                         out highHitInfo, 1.5f);
            //                     
            //                     Debug.DrawLine(pointForAdj, highHitInfo.point, Color.magenta, 3);
            //                     if (hitHigh)
            //                     {
            //                         map.tiles[i, j, k].CoverArray[l] = 2;
            //                     }
            //                     else
            //                     {
            //                         if (adjPointCoord[2] + floorDifference > -1 &&
            //                             adjPointCoord[2] + floorDifference < map.floorCount &&
            //                             map.tiles[adjPointCoord[0], adjPointCoord[1],
            //                                 adjPointCoord[2] + floorDifference].traversable)
            //                         {
            //                             if (floorDifference >= 0)
            //                             {
            //                                 map.tiles[i, j, k].AdjacencyArray[5 + l - 5 * floorDifference] =
            //                                     true;
            //                             }
            //                             else if (floorDifference == -1)
            //                             {
            //                                 map.tiles[i, j, k].AdjacencyArray[l + 9] = true;
            //                             }
            //
            //                             map.tiles[i, j, k].CoverArray[l] = 1;
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }
            //     //
            //     // string str = "";
            //     // foreach (var VARIABLE in map.tiles[i, j, k].AdjacencyArray)
            //     // {
            //     //     str += VARIABLE + ", ";
            //     // }
            //     //
            //     // string logger = "North:" + map.tiles[i, j, k].AdjacencyArray[5] +
            //     //                 " East: " + map.tiles[i, j, k].AdjacencyArray[6] +
            //     //                 " South: " + map.tiles[i, j, k].AdjacencyArray[7] +
            //     //                 " West: " + map.tiles[i, j, k].AdjacencyArray[8]
            //     //     ;
            //     // Debug.Log(str);
            //     // Debug.Log(logger);
            //     // Debug.Log(i + ", " + j + ", " + k);
            // }
        }

        if (moving)
        {
            // TODO: Movement should be by tiles!
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            if (Vector3.Distance(transform.position, destination) < 0.001f)
            {
                transform.position = destination;
                Debug.Log("Stop!");

                if (currentPathList.Count > 0)
                {
                }
                else
                {
                    moving = false;
                }
            }
        }

        // if (Controlled && !moving && !casting && Input.GetKeyDown(KeyCode.Tab))
        // {
        //     Controlled = false;
        //     _mapTerrainControl.queue.RemoveAt(0);
        //     string str = _mapTerrainControl.queue[0].name;
        //     Debug.Log("Assuming direct control! " + str);
        //     _mapTerrainControl.queue.Add(gameObject);
        //
        //     StartCoroutine(coEnumerator()); // Костыль?
        // }
        //
        // // Является ли это костылём?
        // IEnumerator coEnumerator()
        // {
        //     yield return new WaitForEndOfFrame();
        //     _mapTerrainControl.queue[0].GetComponent<ObjectControl>().Controlled = true;
        //     yield return null;
        // }
    }
}