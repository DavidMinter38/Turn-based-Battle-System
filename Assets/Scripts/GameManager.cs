using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Player[] players;
    private Enemy[] enemies;
    private Character[] characters;

    // Start is called before the first frame update
    void Start()
    {
        //Find all players an enemies in the game.  If there are no players or enemies, send an error message.
        players = FindObjectsOfType<Player>();
        enemies = FindObjectsOfType<Enemy>();
        if (players.Length <= 0) { Debug.LogError("No player characters are active in the scene."); }
        if (enemies.Length <= 0) { Debug.LogError("No enemies are active in the scene."); }

        characters = FindObjectsOfType<Character>();

        //Sort the players and enemies to form a turn track
        ArrayList turnOrder = new ArrayList();
        SortCharacters(turnOrder);

        
        foreach(TurnData listEntry in turnOrder)
        {
            int currentID = listEntry.GetID();
            for(int i=0; i<characters.Length; i++)
            {
                int characterID = characters[i].GetID();
                if(currentID == characterID)
                {
                    Debug.Log(characters[i].GetCharacterName());
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SortCharacters(ArrayList turnOrder)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            Character currentCharacter = characters[i];
            float currentCharacterSpeed = currentCharacter.GetSpeed();
            currentCharacterSpeed += Random.Range(-10, 10);
            TurnData characterTurnData = new TurnData(currentCharacter.GetID(), currentCharacterSpeed);
            if (turnOrder.Count == 0)
            {
                turnOrder.Add(characterTurnData);
            }
            else
            {
                int position = 0;
                foreach(TurnData data in turnOrder)
                {
                    if (currentCharacterSpeed >= data.GetSpeed())
                    {
                        break;
                    }
                    else
                    {
                        position++;
                    }
                }
                turnOrder.Insert(position, characterTurnData);
            }
        }
    }

}
