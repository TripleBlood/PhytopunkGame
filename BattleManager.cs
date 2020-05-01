using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public List<CharacterControl> characterQueue;
    
    public string message;

    public void BuildQueue(List<CharacterControl> characters)
    {
        // Implement this method if queue should be built in specific way,
        // but now here's placeholder
        characterQueue = characters;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
