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
    bool isGuarding = false;

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UseGuard()
    {
        isGuarding = true;
        FindObjectOfType<GameManager>().NextTurn(true);
        FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has got their guard up!");
    }

    public void FinishGuard()
    {
        isGuarding = false;
    }

    protected override void KillCharacter()
    {
        FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has fallen!");
        isConscious = false;
        FindObjectOfType<GameManager>().CreateTrack();
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    public int GetCurrentMagic()
    {
        return currentMP;
    }

    public int GetMaxMagic()
    {
        return maxMP;
    }

    public bool CanUseMagic()
    {
        return knowsMagic;
    }

    public bool IsConscious()
    {
        return isConscious;
    }

    public bool IsGuarding()
    {
        return isGuarding;
    }
}
