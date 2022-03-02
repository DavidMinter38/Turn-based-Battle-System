using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Gameplay;
using BattleSystem.Data;
using BattleSystem.Interface;

namespace BattleSystem.Characters
{
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

        bool canHeal;

        int targetID = -1; //Remembers the next target

        EnemyState enemyState;

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
            canHeal = enemyStats.canHeal;
            characterSprite = enemyStats.enemySprite;

            this.GetComponent<SpriteRenderer>().sprite = characterSprite;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ChangeState(EnemyState newState)
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
            int lowestEnemyHealth = -1;
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    int enemyHealth = enemies[i].GetCurrentHealth();
                    if (enemyHealth <= (enemies[i].GetMaxHealth() * 0.3) && canHeal)
                    {
                        if (lowestEnemyHealth == -1 || lowestEnemyHealth >= enemyHealth)
                        {
                            targetID = enemies[i].GetID();
                            lowestEnemyHealth = enemyHealth;
                            ChangeState(EnemyState.Defensive);
                        }
                    }
                }
            }
            if (lowestEnemyHealth != -1) { return; }

            //If the target that was drawing aggro from the enemy is unconscious, return to netural
            if (enemyState == EnemyState.Aggressive)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetID() == targetID)
                    {
                        if (!players[i].IsConscious())
                        {
                            ChangeState(EnemyState.Neutral);
                        }
                    }
                }
            }
            //If in aggressive state, ignore all other conditions
            //Check for finishing state
            if (enemyState != EnemyState.Aggressive)
            {
                int currentPlayerTarget = -1;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].IsConscious())
                    {
                        if (players[i].GetCurrentHealth() <= (players[i].GetMaxHealth() * 0.1))
                        {
                            if (currentPlayerTarget == -1)
                            {
                                targetID = players[i].GetID();
                                currentPlayerTarget = i;
                            }
                            else if ((players[i].GetCurrentHealth() + players[i].GetDefence()) < (players[currentPlayerTarget].GetCurrentHealth() + players[currentPlayerTarget].GetDefence()))
                            {
                                targetID = players[i].GetID();
                                currentPlayerTarget = i;
                            }
                            ChangeState(EnemyState.Finishing);
                        }
                    }
                }
                if (currentPlayerTarget != -1) { return; }

                //Defaults back to neutral if no other criteria is fufilled
                ChangeState(EnemyState.Neutral);
            }
        }

        public void MarkAttacker(int playerID)
        {
            targetID = playerID;
            ChangeState(EnemyState.Aggressive);
        }

        public void ResetMarker()
        {
            targetID = -1;
            ChangeState(EnemyState.Neutral);
        }

        public int GetAttackMarker()
        {
            return targetID;
        }

        public EnemyState GetState()
        {
            return enemyState;
        }
    }
}
