using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int SelectTarget(Enemy attackingEnemy, Player[] players)
    {
        //TODO make enemy AI more intelligent.  AI should do the following:
        //If a player's hp is less than the enemy's attack-player's defence, the enemy will try to attack them
        //Otherwise, if a player attacked them, attack the player in return (agro)
        //Otherwise, attack the strongest player
        int targetID = -1;
        if(attackingEnemy.GetAttackMarker() != -1)
        {
            for (int i = 0; i<players.Length; i++)
            {
                if(players[i].GetID() == attackingEnemy.GetAttackMarker())
                {
                    if (players[i].IsConscious())
                    {
                        targetID = attackingEnemy.GetAttackMarker();
                    }
                }
            }
            attackingEnemy.ResetMarker();
        }
        if (targetID == -1)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsConscious())
                {
                    if (players[i].GetCurrentHealth() <= (players[i].GetMaxHealth() * 0.1))
                    {
                        targetID = players[i].GetID();
                        break;
                    }
                }
            }
        }
        if (targetID == -1)
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
        }
        return targetID;
    }
}

