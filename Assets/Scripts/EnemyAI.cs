using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem.Characters
{
    /// <summary>
    /// The EnemyState enum contains the four states that an enemy can be in.
    /// </summary>
    /// <remarks>The enemy can be in one of four states:</remarks>
    /// <para>Neutral: Simply atttacks the player with the highest combined health and attack values.
    /// Changes to this state if the conditions for the other states are not fufilled.</para>
    /// <para>Aggresive: If attacked, the enemy changes to this state, and then attacks the player that attacked them on their next turn.
    /// This state takes priority over all other states.</para>
    /// <para>Finishing: Attempts to finish a player off.  Changes to this state if a player has 10% of their health remaining.</para>
    /// <para>Defensive: Heals one of the enemy's allies.  Changes to this state if the enemy is able to heal and an enemy has 30% of their health remaining.</para>
    public enum EnemyState
    {
        Neutral,
        Aggressive,
        Finishing,
        Defensive
    }

    /// <summary>
    /// The EnemyAI class handles enemy behaviour.
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        /// <summary>
        /// Contains the enemy's current state.
        /// </summary>
        private EnemyState enemyState;

        /// <summary>
        /// The ID of the character that the enemy intends to use an action on next.
        /// </summary>
        int targetID = -1;

        /// <summary>
        /// At the start of the battle, the enemy's state is set to neutral.
        /// </summary>
        void Start()
        {
            enemyState = EnemyState.Neutral;
        }

        /// <summary>
        /// Checks the state of the battle, and then changes the enemy's state accordingly.
        /// </summary>
        /// <param name="attackingEnemy">The enemy that is attacking.</param>
        /// <param name="players">The players in the battle.</param>
        /// <param name="enemies">The enemies in the battle.</param>
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

        /// <summary>
        /// Decides which character in the battle the enemy is going to use an action on.
        /// </summary>
        /// <param name="attackingEnemy">The enemy that is attacking.</param>
        /// <param name="players">The players in the battle.</param>
        /// <param name="enemies">The enemies in the battle.</param>
        /// <returns></returns>
        public int SelectTarget(Enemy attackingEnemy, Player[] players, Enemy[] enemies)
        {
            int targetID = -1;

            EnemyState state = GetState();

            switch (state)
            {
                case EnemyState.Neutral:
                    targetID = NeutralState(targetID, players);
                    break;
                case EnemyState.Aggressive:
                    targetID = AggressiveState(targetID, players);
                    break;
                case EnemyState.Finishing:
                    targetID = FinishingState(targetID, players);
                    break;
                case EnemyState.Defensive:
                    targetID = DefensiveState(targetID, enemies);
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

        /// <summary>
        /// Finds the player with the highest combined health and attack values.
        /// </summary>
        /// <param name="targetID">The value that the player's ID will be assigned to.</param>
        /// <param name="players">The players in the battle.</param>
        /// <returns>The player's ID.</returns>
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

        /// <summary>
        /// Finds the player that had attacked the enemy previously.
        /// </summary>
        /// <param name="targetID">The value that the player's ID will be assigned to.</param>
        /// <param name="players">The players in the battle.</param>
        /// <returns>The player's ID.</returns>
        private int AggressiveState(int targetID, Player[] players)
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

        /// <summary>
        /// Finds the ID of the player that the enemy intends to finish off.
        /// </summary>
        /// <param name="targetID">The value that the player's ID will be assigned to.</param>
        /// <param name="players">The players in the battle.</param>
        /// <returns>The player's ID.</returns>
        private int FinishingState(int targetID, Player[] players)
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

        /// <summary>
        /// Finds the ID of the enemy that is intended to be healed.
        /// </summary>
        /// <param name="targetID">The value that the enemy's ID will be assigned to.</param>
        /// <param name="enemies">The enemies in the battle.</param>
        /// <returns>The enemy's ID.</returns>
        private int DefensiveState(int targetID, Enemy[] enemies)
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

        /// <summary>
        /// Stores the ID of the player that attacked the enemy, and then changes the enemy's state to aggressive.
        /// </summary>
        /// <remarks>This is stored so that, when the enemy next chooses an attack target, it will know which player to attack.</remarks>
        /// <param name="playerID">The ID of the player that attacked the enemy.</param>
        public void MarkAttacker(int playerID)
        {
            SetMarker(playerID);
            ChangeState(EnemyState.Aggressive);
        }

        /// <summary>
        /// Stores the ID of the character the enemy intends to use an action on.
        /// </summary>
        /// <param name="newID">The ID of the character.</param>
        public void SetMarker(int newID)
        {
            targetID = newID;
        }

        /// <summary>
        /// Resets the enemy's state back to neutral.
        /// </summary>
        public void ResetMarker()
        {
            targetID = -1;
            ChangeState(EnemyState.Neutral);
        }

        /// <summary>
        /// Gets the ID of the enemy's target.
        /// </summary>
        /// <returns>The ID of the character the enemy intends to use an action on.</returns>
        public int GetAttackMarker()
        {
            return targetID;
        }

        /// <summary>
        /// Retrieves the enemy's current state.
        /// </summary>
        /// <returns>The enemy's state.</returns>
        public EnemyState GetState()
        {
            return enemyState;
        }

        /// <summary>
        /// Changes the enemy's state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        public void ChangeState(EnemyState newState)
        {
            enemyState = newState;
        }
    }
}

