using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Player[] players;
    private Enemy[] enemies;
    private Character[] characters;

    ArrayList turnOrder = new ArrayList();
    int currentPlayerInTurn = 0;

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
        SortCharacters();


        //Debug code to check that the turn order is listed correctly.
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
        //Debug code to check that the game progresses through each character in a round of combat
        if (Input.GetKeyDown(KeyCode.A))
        {
            int currentTurnPlayerID = (turnOrder[currentPlayerInTurn] as TurnData).GetID();
            for (int i = 0; i < characters.Length; i++)
            {
                int characterID = characters[i].GetID();
                if (currentTurnPlayerID == characterID)
                {
                    Character attackingCharacter = characters[i];
                    StartCoroutine(attackingCharacter.Attack(characters[0], 10));
                }
            }
        }
    }

    private void SortCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            //Goes through each character in the battle and adds their ID and speed values to the turn order.
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
                //Search through the turn order and place the character according to their speed value.
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
                //Add the data to the array
                turnOrder.Insert(position, characterTurnData);
            }
        }
    }

    public void NextTurn()
    {
        currentPlayerInTurn++;
        if(currentPlayerInTurn > turnOrder.Count - 1)
        {
            //Round of combat has ended
            NewRound();
            
        }
    }

    public void NewRound()
    {
        currentPlayerInTurn = 0;
        //Reset the turn order so that it's not always the same
        turnOrder.Clear();
        SortCharacters();
    }

}
