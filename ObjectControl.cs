using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Utils;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private BattleManager currentBattleManager;

    public GameObject lightSource;

    public List<GameObject> lightSources;

    // Start is called before the first frame update

    void Start()
    {
        destination = transform.position;

        int battlemask = 1 << 10;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 50, battlemask);

        if (hitColliders.Length > 0)
        {
            currentBattleManager = hitColliders[0].gameObject.GetComponent<BattleManager>();
        }

        try
        {
            this.map = currentBattleManager.map;
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
        if (Input.GetKey(KeyCode.L))
        {
            Vector3 origin = new Vector3(0, 10, 9);
            destination = new Vector3(0, 0, 0);
            Debug.DrawLine(origin, destination, Color.blue, 5);
            map.UpdateIllumination(lightSources);

            // Debug.Log(Vector3.SignedAngle(origin, destination, Vector3.up));
            // Debug.Log(Vector3.SignedAngle(origin, destination, Vector3.forward));
            // Debug.Log(Vector3.SignedAngle(origin, destination, Vector3.right));
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
                Vector3 pointToLight = hitInfo.point - lightSource.transform.position;
                Debug.DrawLine(lightSource.transform.position, lightSource.transform.position + pointToLight,
                    Color.blue, 5);

                // Debug.Log(Vector3.SignedAngle(lightSource.transform.forward, pointToLight, Vector3.up));
                // Debug.Log(Vector3.SignedAngle(lightSource.transform.forward, pointToLight, Vector3.forward));
                // Debug.Log(Vector3.SignedAngle(lightSource.transform.forward, pointToLight, Vector3.right));


                if (hitInfo.transform.gameObject.tag.Equals("Floor"))
                {
                    if (EventSystem.current.IsPointerOverGameObject()) return;

                    Vector3 point = hitInfo.point;
                    array = map.GetTileIndexesByCoords(point);
                    // Debug.Log(array[0] + ", " + array[1] + ", " + array[2] + ", ");

                    Tile tile = map.FindTile(array[0], array[1], array[2]);

                    Debug.Log("Tile " + tile.x + ", " + tile.z + ", " + tile.y + " illumination is " +
                              tile.IsIlluminated);


                    // ---------------------

                    foreach (Vector3 pointForCheck in map.GetPointsForIlluminationCheck(tile))
                    {
                        int illuminationBuffer = 0;
                        int sourcesRequired = 2;

                        foreach (GameObject lightSource in lightSources)
                        {
                            Light lightComponent = lightSource.GetComponent<Light>();

                            Vector3 tileToLightSource = lightSource.transform.position - pointForCheck;

                            if (lightComponent.type == LightType.Directional)
                            {
                                RaycastHit hitInfoLight = new RaycastHit();
                                bool hitLight = Physics.Raycast(pointForCheck, -lightSource.transform.forward,
                                    out hitInfoLight, Single.PositiveInfinity);

                                if (!hitLight)
                                {
                                    illuminationBuffer++;
                                }
                            }
                            else if (lightComponent.type == LightType.Spot)
                            {
                                if (tileToLightSource.magnitude <= lightComponent.range)
                                {
                                    if (Math.Abs(Vector3.SignedAngle(lightSource.transform.forward, -tileToLightSource,
                                            Vector3.up)) <= lightComponent.spotAngle / 2 &&
                                        Math.Abs(Vector3.SignedAngle(lightSource.transform.forward, -tileToLightSource,
                                            Vector3.forward)) <= lightComponent.spotAngle / 2 &&
                                        Math.Abs(Vector3.SignedAngle(lightSource.transform.forward, -tileToLightSource,
                                            Vector3.right)) <= lightComponent.spotAngle / 2)
                                    {
                                        RaycastHit hitInfoLight = new RaycastHit();
                                        bool hitLight = Physics.Raycast(pointForCheck, tileToLightSource,
                                            out hitInfoLight, tileToLightSource.magnitude);
                                        if (!hitLight)
                                        {
                                            illuminationBuffer++;

                                            Debug.DrawLine(pointForCheck, pointForCheck + Vector3.up, Color.white, 5);
                                            Debug.DrawLine(pointForCheck, pointForCheck + tileToLightSource,
                                                Color.green, 5);
                                        }
                                    }
                                }
                            }
                            else if (lightComponent.type == LightType.Rectangle)
                            {
                            }
                            else if (lightComponent.type == LightType.Point)
                            {
                            }
                        }
                    }

                    // ---------------------

                    if (tile.x != -1)
                    {
                        string logger = "North:" + tile.CoverArray[0] +
                                        " East: " + tile.CoverArray[1] +
                                        " South: " + tile.CoverArray[2] +
                                        " West: " + tile.CoverArray[3]
                            ;
                        //Debug.Log(str);
                        // Debug.Log(logger);
                        // Debug.Log("---------");

                        string dictstrCheck = "Adj tiles: ";


                        if (
                            true /*map.mapTileDict.TryGetValue(MapUtils.GetTileHash(array[0], array[1], array[2]), out tile)*/
                        )
                        {
                            List<Tile> list = MapUtils.GetAdjTilesWithOccCheck(map, tile.x, tile.z, tile.y);
                            foreach (Tile tile1 in list)
                            {
                                dictstrCheck += tile1.x + ", " + tile1.z + ", " + tile1.y + "| ";
                            }
                        }
                    }


                    //Debug.Log(dictstrCheck);

                    map.OccupyTile(ring, map.FindTile(11, 13, 1));

                    string pathstr = "";
                    List<Tile> path = MapUtils.FindPath(map, map.FindTile(12, 13, 1), map.FindTile(12, 16, 0));
                    foreach (Tile VARIABLE in path)
                    {
                        pathstr += VARIABLE.x + ", " + VARIABLE.z + ", " + VARIABLE.y + " ---> ";
                    }

                    //Debug.Log(pathstr);
                }
            }

            if (moving)
            {
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
}