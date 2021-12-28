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

    bool isConscious = true;

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = true;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void KillCharacter()
    {
        Debug.Log(this.GetCharacterName() + " has fallen!");
        isConscious = false;
    }

    public bool CanUseMagic()
    {
        return knowsMagic;
    }

    public bool IsConscious()
    {
        return isConscious;
    }
}
