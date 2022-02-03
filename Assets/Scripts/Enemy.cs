using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Neutral,
    Aggressive,
    Finishing,
    Defensive
}

public class Enemy : Character
{
    [SerializeField]
    int enemyID; //If we are having multiple types of enemies, this will allow us to figure out what type of enemy it is.  Note that the enemyID is not the same as the character ID

    int playerToAttackID = -1; //Remembers the player that it wants to attack next

    public EnemyState enemyState;

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = false;
        enemyState = EnemyState.Neutral;

        GameData.EnemyStats enemyStats = FindObjectOfType<GameData>().GetEnemyStats(enemyID);
        characterName = enemyStats.enemyName;
        currentHP = enemyStats.currentHP;
        maxHP = enemyStats.maxHP;
        attack = enemyStats.attack;
        defence = enemyStats.defence;
        magicAttack = enemyStats.magicAttack;
        magicDefence = enemyStats.magicDefence;
        speed = enemyStats.speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(EnemyState newState)
    {
        enemyState = newState;
    }

    protected override void KillCharacter()
    {
        FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has been destroyed!");
        FindObjectOfType<GameManager>().RemoveEnemy(this.GetID());
        Destroy(this.gameObject);
    }

    public void ObserveStatusOfBattle(Player[] players, Enemy[] enemies)
    {
        //Checks the health of the players and enemies in the battle, and updates its state accordingly

        //Check for defensive state
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                if (enemies[i].GetCurrentHealth() <= (enemies[i].GetMaxHealth() * 0.3))
                {
                    enemyState = EnemyState.Defensive;
                    return;
                }
            }
        }

        //If in aggressive state, ignore all other conditions
        //Check for finishing state
        if (enemyState != EnemyState.Aggressive)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsConscious())
                {
                    if (players[i].GetCurrentHealth() <= (players[i].GetMaxHealth() * 0.1))
                    {
                        playerToAttackID = players[i].GetID();
                        enemyState = EnemyState.Finishing;
                        return;
                    }
                }
            }

            //Defaults back to neutral if no other criteria is fufilled
            enemyState = EnemyState.Neutral;
        }
    }

    public void MarkAttacker(int playerID)
    {
        playerToAttackID = playerID;
        enemyState = EnemyState.Aggressive;
    }

    public void ResetMarker()
    {
        playerToAttackID = -1;
        enemyState = EnemyState.Neutral;
    }

    public int GetAttackMarker()
    {
        return playerToAttackID;
    }
}
