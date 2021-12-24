using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    int playerID;  //This will be different for each player prefab, so that we know which player is which

    [SerializeField]
    protected int currentMP, maxMP;

    [SerializeField]
    bool knowsMagic;

    bool isConscious;

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = true;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected new void KillCharacter()
    {
        Debug.Log(this.GetCharacterName() + " has fallen!");
        isConscious = false;
        //TODO check how many players are conscious.  If 0, game ends.
    }

    public bool CanUseMagic()
    {
        return knowsMagic;
    } 
}
