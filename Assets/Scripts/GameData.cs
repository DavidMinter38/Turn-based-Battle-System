using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains all the game data, including player and enemy stats.  These values will be assigned to the corresponding character in the game.

public class GameData : MonoBehaviour
{
    //Player data
    Player[] playerData = new Player[3];

    //Player 1

    int player1ID = 1;
    int player1MaxHealth = 100;
    int player1CurrentHealth = 100;
    int player1Attack = 12;
    int player1Defence = 10;
    int player1Speed = 15;


    //Enemy data

    Enemy enemy1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
