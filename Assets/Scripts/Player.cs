using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    int playerID;  //This will be different for each player prefab, so that we know which player is which

    protected int currentMP, maxMP;

    bool knowsMagic;
    bool[] avaliableMagic;

    bool isConscious = true;
    bool isGuarding = false;

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = true;

        GameData.PlayerStats playerStats = FindObjectOfType<GameData>().GetPlayerStats(playerID);
        characterName = playerStats.playerName;
        currentHP = playerStats.currentHP;
        maxHP = playerStats.maxHP;
        currentMP = playerStats.currentMP;
        maxMP = playerStats.maxMP;
        attack = playerStats.attack;
        defence = playerStats.defence;
        magicAttack = playerStats.magicAttack;
        magicDefence = playerStats.magicDefence;
        speed = playerStats.speed;
        knowsMagic = playerStats.knowsMagic;
        avaliableMagic = playerStats.avaliableMagic;
        isConscious = playerStats.isConscious;

        if (!playerStats.isAvaliable)
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GainMagic(int magicGained)
    {
        currentMP += magicGained;
        if (currentMP > maxMP)
        {
            currentMP = maxMP;
        }
        FindObjectOfType<BattleUI>().UpdateUI();
    }

    public void LoseMagic(int magicLost)
    {
        currentMP -= magicLost;
        if (currentMP <= 0) { currentMP = 0; }
        FindObjectOfType<BattleUI>().UpdateUI();
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

    public bool[] GetKnownMagic()
    {
        return avaliableMagic;
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
