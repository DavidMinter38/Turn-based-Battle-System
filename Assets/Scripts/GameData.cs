using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains all the game data, including player and enemy stats.  These values will be assigned to the corresponding character in the game.

public class GameData : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStats
    {
        [Header("Player infomation")]
        [Tooltip("The unique ID used to identify the player.")]
        public int playerID;
        [Tooltip("The name that will be used for the player in game.")]
        public string playerName;
        [Header("Player attributes")]
        public int currentHP;
        public int maxHP;
        public int currentMP;
        public int maxMP;
        public int attack;
        public int defence;
        public int magicAttack;
        public int magicDefence;
        public int speed;
        [Header("Player features")]
        public bool knowsMagic;
        public bool[] avaliableMagic;
        [Header("Player status")]
        [Tooltip("If unmarked, the player is not currently a member of the party.")]
        public bool isAvaliable;
        public bool isConscious;
    }

    [System.Serializable]
    public struct EnemyStats
    {
        [Header("Enemy infomation")]
        [Tooltip("The unique ID used to identify the enemy.")]
        public int enemyID;
        [Tooltip("The name that will be used for the enemy in game.")]
        public string enemyName;
        [Header("Enemy attributes")]
        public int currentHP;
        public int maxHP;
        public int attack;
        public int defence;
        public int magicAttack;
        public int magicDefence;
        public int speed;
    }

    [SerializeField]
    PlayerStats[] playerList;

    [SerializeField]
    EnemyStats[] enemyTypes;

    public PlayerStats GetPlayerStats(int playerID)
    {
        return playerList[playerID];
    }

    public EnemyStats GetEnemyStats(int enemyID)
    {
        return enemyTypes[enemyID];
    }
}
