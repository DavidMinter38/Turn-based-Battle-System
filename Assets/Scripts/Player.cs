using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    int playerID;  //This will be different for each player prefab, so that we know which player is which

    // Start is called before the first frame update
    void Start()
    {
        GameData theData = FindObjectOfType<GameData>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("This player's health is " + health);
        Debug.Log("This player's attack is " + health);
        Debug.Log("This player's defence is " + health);
        Debug.Log("This player's speed is " + health);
    }
}
