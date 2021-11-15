using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Player[] players;
    private Enemy[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        //Find all players an enemies in the game.  If there are no players or enemies, send an error message.
        players = FindObjectsOfType<Player>();
        enemies = FindObjectsOfType<Enemy>();
        if (players.Length <= 0) { Debug.LogError("No player characters are active in the scene."); }
        if (enemies.Length <= 0) { Debug.LogError("No enemies are active in the scene."); }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
