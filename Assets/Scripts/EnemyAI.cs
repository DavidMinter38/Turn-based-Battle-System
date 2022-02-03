using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public int SelectTarget(Enemy attackingEnemy, Player[] players)
    {
        //The AI will behave under the following conditions:
        //If a player's hp is less than the enemy's attack-player's defence, the enemy will try to attack them
        //Otherwise, if a player attacked them, attack the player in return (agro)
        //Otherwise, attack the strongest player
        int targetID = -1;

        switch (attackingEnemy.enemyState)
        {
            case EnemyState.Defensive:
                targetID = DefensiveState(targetID, attackingEnemy, players);
                break;
            case EnemyState.Aggressive:
                targetID = AggressiveState(targetID, attackingEnemy, players);
                break;
            case EnemyState.Finishing:
                targetID = FinishingState(targetID, attackingEnemy, players);
                break;
            case EnemyState.Neutral:
                targetID = NeutralState(targetID, players);
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
            if (players[i].GetID() == attackingEnemy.GetAttackMarker())
            {
                if (players[i].IsConscious())
                {
                    targetID = attackingEnemy.GetAttackMarker();
                }
            }
        }
        attackingEnemy.ResetMarker();
        return targetID;
    }

    private int FinishingState(int targetID, Enemy attackingEnemy, Player[] players)
    {
        //Similar to the Aggressive state, except the enemy does not revert back to a neutral state after attacking the player
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetID() == attackingEnemy.GetAttackMarker())
            {
                if (players[i].IsConscious())
                {
                    targetID = attackingEnemy.GetAttackMarker();
                }
            }
        }
        return targetID;
    }

    private int DefensiveState(int targetID, Enemy attackingEnemy, Player[] players)
    {
        //Debug code to prevent errors
        //TODO implement defensive state code
        return 1;
    }
}

