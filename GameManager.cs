using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public bool inBattle;
    // Start is called before the first frame update
    void Start()
    {
        inBattle = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inBattle = true; //TODO: get 
            
        }
    }
}
