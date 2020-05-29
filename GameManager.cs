using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool inBattle;

    public GameObject controlledCharacter;
    public BattleManager currentBattleManager;


    // Start is called before the first frame update
    void Start()
    {
        inBattle = false;
    }

    // Update is called once per frame
    void Update()
    {
        int battlemask = 1 << 10;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider[] hitColliders = Physics.OverlapSphere(controlledCharacter.transform.position, 50, battlemask);

            if (hitColliders.Length > 0)
            {
                currentBattleManager = hitColliders[0].gameObject.GetComponent<BattleManager>();
                currentBattleManager.enabled = true;

                int characterMask = 1 << 9;
                Collider[] characterColliders =
                    Physics.OverlapSphere(controlledCharacter.transform.position, 50, characterMask);
                
                List<CharacterBattleController> characters = new List<CharacterBattleController>();

                foreach (Collider characterCollider in characterColliders) 
                {
                    //Expensive
                    characters.Add(characterCollider.gameObject.GetComponent<CharacterBattleController>());
                }
                currentBattleManager.BuildQueue(characters);

                inBattle = true; //TODO: get 
            }
        }
    }
    
    public void AbilityBtnPressed(int index)
    {
        if (inBattle)
        {
            currentBattleManager.TrySwapTargetingController(index);
        }
        // return true;
    }
}