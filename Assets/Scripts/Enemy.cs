using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Data;
using BattleSystem.Interface;

namespace BattleSystem.Characters
{
    /// <summary>
    /// The Enemy class inherits from the Character class, and contains features related to enemy AI.
    /// </summary>
    public class Enemy : Character
    {
        /// <summary>
        /// An ID used to uniquely identify each type of enemy that can be fought.
        /// </summary>
        [SerializeField]
        int enemyID;

        /// <summary>
        /// Indicates if the enemy is able to heal its allies.
        /// </summary>
        bool canHeal;
        /// <summary>
        /// Indicates if the enemy is currently alive.
        /// </summary>
        bool alive = true;

        /// <summary>
        /// A reference to the EnemyAI.
        /// </summary>
        EnemyAI enemyAI;

        /// <summary>
        /// On startup, the Awake function finds the infomation in GameData for the enemy that matches the enemy's ID, and then assigns the data to the respective values.
        /// </summary>
        void Awake()
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
            canHeal = enemyStats.canHeal;
            characterSprite = enemyStats.enemySprite;

            this.GetComponent<SpriteRenderer>().sprite = characterSprite;

            //Attach the EnemyAI script to the enemy game object
            enemyAI = this.gameObject.AddComponent<EnemyAI>();
        }

        /// <summary>
        /// Declares that the enemy is no longer alive.
        /// </summary>
        /// <remarks>At the end of the round, the GameManager will remove any enemies from the battle that have been killed.</remarks>
        protected override void KillCharacter()
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has been destroyed!");
            alive = false;
        }

        /// <summary>
        /// Uses it's AI component to decide which character in the battle is going to be the target of its next action.
        /// </summary>
        /// <param name="players">A list of all the players in the battle.</param>
        /// <param name="enemies">A list of all the enemies in the battle.  This is in case the enemy decides to heal an enemy.</param>
        /// <returns>The ID of the enemy's target</returns>
        public int DecideTarget(Player[] players, Enemy[] enemies)
        {
            enemyAI.ObserveStatusOfBattle(this, players, enemies);
            return enemyAI.SelectTarget(this, players, enemies);
        }

        /// <summary>
        /// Checks if the enemy is able to heal.
        /// </summary>
        /// <returns>The boolean value used to indicate if the enemy is able to heal.</returns>
        public bool IsAbleToHeal()
        {
            return canHeal;
        }

        /// <summary>
        /// Checks if the enemy is alive.
        /// </summary>
        /// <returns>The boolean value used to indicate if the enemy is alive.</returns>
        public bool IsAlive()
        {
            return alive;
        }

        /// <summary>
        /// Retrieve's the enemy's AI component.
        /// </summary>
        /// <returns>The AI component included in the Enemy class.</returns>
        public EnemyAI GetAIComponent()
        {
            return enemyAI;
        }
    }
}
