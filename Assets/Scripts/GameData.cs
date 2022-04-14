using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Interface;

namespace BattleSystem.Data
{
    /// <summary>
    /// Contains all the game data, including player and enemy values.
    /// </summary>
    /// <remarks>There are values stored for each type of player and enemy.  These values will be assigned to the respective character in the game.</remarks>
    public class GameData : MonoBehaviour
    {
        /// <summary>
        /// Infomation about the players.
        /// </summary>
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
            public bool[] availableMagic;
            [Header("Player status")]
            [Tooltip("If unmarked, the player is not currently a member of the party.")]
            public bool isAvailable;
            public bool isConscious;
            [Header("Graphics")]
            public Sprite playerSprite;
            public PlayerHealthUI playerHealthUI;
        }

        /// <summary>
        /// Infomation about the various enemy types that can be encountered.
        /// </summary>
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
            public bool canHeal;
            [Header("Graphics")]
            public Sprite enemySprite;
        }

        /// <summary>
        /// A list of all the player characters that are available.
        /// </summary>
        [SerializeField]
        PlayerStats[] playerList;

        /// <summary>
        /// A list of all the various types of enemies that could be encountered.
        /// </summary>
        [SerializeField]
        EnemyStats[] enemyTypes;

        /// <summary>
        /// Retrieves the types of enemies that are available.
        /// </summary>
        /// <returns>The array of enemy types.</returns>
        public EnemyStats[] GetEnemyTypes()
        {
            return enemyTypes;
        }

        /// <summary>
        /// Retrieves the stored data on a particular player.
        /// </summary>
        /// <remarks>Each data on a player has a unique ID.  This is used to figure out which player is which.</remarks>
        /// <param name="playerID">The player ID.</param>
        /// <returns>The player data with the ID that matches the player ID.</returns>
        public PlayerStats GetPlayerStats(int playerID)
        {
            return playerList[playerID];
        }

        /// <summary>
        /// Retrieves the stored data on a particular enemy.
        /// </summary>
        /// <param name="enemyID">The enemy ID.</param>
        /// <returns>The enemy data with the ID that matches the enemy ID.</returns>
        public EnemyStats GetEnemyStats(int enemyID)
        {
            return enemyTypes[enemyID];
        }
    }
}
