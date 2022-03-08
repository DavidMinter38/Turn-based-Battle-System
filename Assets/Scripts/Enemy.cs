using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Data;
using BattleSystem.Interface;

namespace BattleSystem.Characters
{

    public class Enemy : Character
    {
        [SerializeField]
        int enemyID; //If we are having multiple types of enemies, this will allow us to figure out what type of enemy it is.  Note that the enemyID is not the same as the character ID

        bool canHeal;
        bool alive = true;

        EnemyAI enemyAI;  //Contains code that decides how the enemy should attack

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

        protected override void KillCharacter()
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has been destroyed!");
            alive = false;
        }

        public int DecideTarget(Player[] players, Enemy[] enemies)
        {
            enemyAI.ObserveStatusOfBattle(this, players, enemies);
            return enemyAI.SelectTarget(this, players, enemies);
        }

        public bool IsAbleToHeal()
        {
            return canHeal;
        }

        public bool IsAlive()
        {
            return alive;
        }

        public EnemyAI GetAIComponent()
        {
            return enemyAI;
        }
    }
}
