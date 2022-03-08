using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem.Characters
{
    public enum EnemyState
    {
        Neutral,
        Aggressive,
        Finishing,
        Defensive
    }

    public class EnemyAI : MonoBehaviour
    {
        private EnemyState enemyState;
        int targetID = -1; //Remembers the next target

        void Start()
        {
            enemyState = EnemyState.Neutral;
        }

        public void ObserveStatusOfBattle(Enemy attackingEnemy, Player[] players, Enemy[] enemies)
        {
            //Checks the health of the players and enemies in the battle, and updates its state accordingly

            //Check for defensive state
            int lowestEnemyHealth = -1;
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    int enemyHealth = enemies[i].GetCurrentHealth();
                    if (enemyHealth <= (enemies[i].GetMaxHealth() * 0.3) && attackingEnemy.IsAbleToHeal())
                    {
                        if (lowestEnemyHealth == -1 || lowestEnemyHealth >= enemyHealth)
                        {
                            SetMarker(enemies[i].GetID());
                            lowestEnemyHealth = enemyHealth;
                            ChangeState(EnemyState.Defensive);
                        }
                    }
                }
            }
            if (lowestEnemyHealth != -1) { return; }

            //If the target that was drawing aggro from the enemy is unconscious, return to netural
            if (GetState() == EnemyState.Aggressive)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetID() == GetAttackMarker())
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
            if (GetState() != EnemyState.Aggressive)
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
                                SetMarker(players[i].GetID());
                                currentPlayerTarget = i;
                            }
                            else if ((players[i].GetCurrentHealth() + players[i].GetDefence()) < (players[currentPlayerTarget].GetCurrentHealth() + players[currentPlayerTarget].GetDefence()))
                            {
                                SetMarker(players[i].GetID());
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

        public int SelectTarget(Enemy attackingEnemy, Player[] players, Enemy[] enemies)
        {
            //The AI will behave under the following conditions:
            //If a player's hp is less than the enemy's attack-player's defence, the enemy will try to attack them
            //Otherwise, if a player attacked them, attack the player in return (agro)
            //Otherwise, attack the strongest player
            int targetID = -1;

            EnemyState state = GetState();

            switch (state)
            {
                case EnemyState.Neutral:
                    targetID = NeutralState(targetID, players);
                    break;
                case EnemyState.Aggressive:
                    targetID = AggressiveState(targetID, attackingEnemy, players);
                    break;
                case EnemyState.Finishing:
                    targetID = FinishingState(targetID, attackingEnemy, players);
                    break;
                case EnemyState.Defensive:
                    targetID = DefensiveState(targetID, attackingEnemy, enemies);
                    break;
                default:
                    break;
            }

            if (targetID == -1)
            {
                Debug.LogError("Enemy could not find a target.");
            }

            return targetID;
        }

        private int NeutralState(int targetID, Player[] players)
        {
            int highestHealthAndAttack = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsConscious())
                {
                    int playerHealthAndAttack = players[i].GetCurrentHealth() + players[i].GetAttack();
                    if (playerHealthAndAttack > highestHealthAndAttack)
                    {
                        highestHealthAndAttack = playerHealthAndAttack;
                        targetID = players[i].GetID();
                    }
                }
            }
            return targetID;
        }

        private int AggressiveState(int targetID, Enemy attackingEnemy, Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetID() == GetAttackMarker())
                {
                    if (players[i].IsConscious())
                    {
                        targetID = GetAttackMarker();
                    }
                }
            }
            ResetMarker();
            return targetID;
        }

        private int FinishingState(int targetID, Enemy attackingEnemy, Player[] players)
        {
            //Similar to the Aggressive state, except the enemy does not revert back to a neutral state after attacking the player
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetID() == GetAttackMarker())
                {
                    if (players[i].IsConscious())
                    {
                        targetID = GetAttackMarker();
                    }
                }
            }
            return targetID;
        }

        private int DefensiveState(int targetID, Enemy attackingEnemy, Enemy[] enemies)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    if (enemies[i].GetID() == GetAttackMarker())
                    {

                        targetID = GetAttackMarker();
                    }
                }
            }
            return targetID;
        }

        public void MarkAttacker(int playerID)
        {
            SetMarker(playerID);
            ChangeState(EnemyState.Aggressive);
        }

        public void SetMarker(int newID)
        {
            targetID = newID;
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

        public void ChangeState(EnemyState newState)
        {
            enemyState = newState;
        }
    }
}

