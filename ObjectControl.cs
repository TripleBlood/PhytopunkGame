﻿using System;
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
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity,
                mask);

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
                    Debug.Log("---------");
                    string dictstrCheck = "Adj tiles: ";

                    Tile tile;
                    if (map.mapTileDict.TryGetValue(MapUtils.GetTileHash(array[0], array[1], array[2]), out tile))
                    {
                        List<Tile> list = MapUtils.getTilesByAdjDict(map, tile.x, tile.z, tile.y);
                        foreach (Tile tile1 in list)
                        {
                            dictstrCheck += tile1.x + ", " + tile1.z + ", " + tile1.y + "| ";
                        }
                    }

                    Debug.Log(dictstrCheck);

                    string pathstr = ""; 
                    List<Tile> path = MapUtils.findPath(map, map.findTile(12, 13, 1), map.findTile(12, 16, 0));
                    foreach (Tile VARIABLE in path)
                    {
                        pathstr += VARIABLE.x + ", " + VARIABLE.z + ", " + VARIABLE.y + " ---> ";
                    }
                    Debug.Log(pathstr);
                }
            }

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