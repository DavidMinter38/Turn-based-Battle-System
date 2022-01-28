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

    public int SelectTarget(Player[] players)
    {
        //TODO make enemy AI more intelligent.  AI should do the following:
        //If a player's hp is less than the enemy's attack-player's defence, he enemy will try to attack them
        //Otherwise, if a player attacked them, attack the player in return (agro)
        //Otherwise, attack the strongest player
        int randomTarget = Random.Range(0, players.Length);
        //If the enemy is targetting an unconscious player, find a new target until the enemy is targetting a conscious player
        while (!players[randomTarget].IsConscious())
        {
            randomTarget = Random.Range(0, players.Length);
        }
        int targetID = players[randomTarget].GetID();
        return targetID;
    }
}
