using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    int enemyID; //If we are having multiple types of enemies, this will allow us to figure out what type of enemy it is.  Note that the enemyID is not the same as the character ID

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = false;

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

    protected override void KillCharacter()
    {
        FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has been destroyed!");
        FindObjectOfType<GameManager>().RemoveEnemy(this.GetID());
        Destroy(this.gameObject);
    }
}
