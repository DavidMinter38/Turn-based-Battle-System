using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Player[] players;
    private Enemy[] enemies;
    private Character[] characters;

    BattleState battleState = BattleState.Start;

    ArrayList turnOrder = new ArrayList();
    int currentPlayerInTurn = 0;

    PlayerBattleMenu playerMenu;
    TargetMarker targetMarker;

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

        playerMenu = FindObjectOfType<PlayerBattleMenu>();
        if (playerMenu == null) { Debug.LogError("Player menu is not in the scene."); }
        playerMenu.HideBattleMenu();

        targetMarker = FindObjectOfType<TargetMarker>();
        if (targetMarker == null) { Debug.LogError("Target marker is not in the scene."); }

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
        if (battleState != BattleState.Victory && battleState != BattleState.Defeat)
        {
            if ((turnOrder[currentPlayerInTurn] as TurnData).IsPlayer())
            {
                //Stop, and let the player choose who to attack
                int currentTurnPlayerID = (turnOrder[currentPlayerInTurn] as TurnData).GetID();
                Player currentTurnPlayer = (Player)GetCharacter(currentTurnPlayerID);
                if (!currentTurnPlayer.IsConscious())
                {
                    //If the player is unconscious, skip their turn and move on to the next character
                    NextTurn();
                    return;
                }
                if (!playerMenu.gameObject.activeInHierarchy && battleState != BattleState.PlayerTurnSelectTarget && battleState != BattleState.PlayerTurnAttack)
                {
                    playerMenu.DisplayBattleMenu();
                    battleState = BattleState.PlayerTurnSelectMove;
                }
            }
            else
            {
                //The next character is an enemy.  Attack a player character
                battleState = BattleState.EnemyTurn;
                EnemyTurn();
            }
        }
    }

    public IEnumerator Attack(int targetID)
    {
        int currentTurnPlayerID = (turnOrder[currentPlayerInTurn] as TurnData).GetID();
        Character attackingCharacter = GetCharacter(currentTurnPlayerID);
        Character defendingCharacter = GetCharacter(targetID);

        StartCoroutine(attackingCharacter.Attack(defendingCharacter, CalculateDamage(attackingCharacter.GetAttack(), defendingCharacter.GetDefence())));
        yield return new WaitForSeconds(1f);
    }

    private void SortCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if(characters[i] == null) { return; }
            //Goes through each character in the battle and adds their ID and speed values to the turn order.
            Character currentCharacter = characters[i];
            float currentCharacterSpeed = currentCharacter.GetSpeed();
            bool currentCharacterIsPlayer = currentCharacter.IsPlayer();
            currentCharacterSpeed += Random.Range(-10, 10);
            TurnData characterTurnData = new TurnData(currentCharacter.GetID(), currentCharacterSpeed, currentCharacterIsPlayer);
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

    private Character GetCharacter(int id)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null)
            {
                int characterID = characters[i].GetID();
                if (id == characterID)
                {
                    return characters[i];
                }
            }
        }
        return null;
    }

    private void EnemyTurn()
    {
        int currentTurnPlayerID = (turnOrder[currentPlayerInTurn] as TurnData).GetID();
        Enemy attackingEnemy = (Enemy)GetCharacter(currentTurnPlayerID);
        if (attackingEnemy != null)
        {
            int targetID = players[Random.Range(0, players.Length)].GetID();
            StartCoroutine("Attack", targetID);
        } else
        {
            //Enemy does not exist and should be ignored.
            NextTurn();
        }
    }

    int CalculateDamage(int attack, int defence)
    {
        int damage = (attack - defence) + Random.Range(-5, 5);
        if(damage < 0) { damage = 0; }
        return damage;
    }

    public void NextTurn()
    {
        //If no players are alive, game ends.
        if (!AnyAlivePlayers())
        {
            GameOver();
            return;
        }

        //Check if any enemies are still alive
        if (!AreThereEnemiesRemaining())
        {
            Victory();
            return;
        }

        currentPlayerInTurn++;

        if (currentPlayerInTurn > turnOrder.Count - 1)
        {
            //Round of combat has ended
            NewRound();
        }
    }

    public void NewRound()
    {
        currentPlayerInTurn = 0;
        battleState = BattleState.Start;
        //Reset the turn order so that it's not always the same
        turnOrder.Clear();
        SortCharacters();
    }

    public void Victory()
    {
        battleState = BattleState.Victory;
        Debug.Log("You Win!");
        EndBattle();
    }

    public void GameOver()
    {
        battleState = BattleState.Defeat;
        Debug.Log("Game Over");
        EndBattle();
    }

    private void EndBattle()
    {
        playerMenu.gameObject.SetActive(false);
        targetMarker.gameObject.SetActive(false);
    }

    public void RemoveEnemy(int enemyID)
    {
        //Remove a defeated enemy from the array of characters that are in the battle
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null)
            {
                int characterID = characters[i].GetID();
                if (enemyID == characterID)
                {
                    characters[i] = null;
                    Debug.Log("Enemy removed from character array");
                }
            }
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                int characterID = enemies[i].GetID();
                if (enemyID == characterID)
                {
                    enemies[i] = null;
                    Debug.Log("Enemy removed from enemy array");
                }
            }
        }

        targetMarker.SetEnemyTargets(enemies);
    }

    public void SetStatePlayerSelectMove()
    {
        battleState = BattleState.PlayerTurnSelectMove;
    }

    public void SetStatePlayerSelectTarget()
    {
        battleState = BattleState.PlayerTurnSelectTarget;
    }

    public Enemy[] GetEnemies()
    {
        return enemies;
    }

    private bool AnyAlivePlayers()
    {
        for(int i=0; i<players.Length; i++)
        {
            if (players[i].IsConscious())
            {
                return true;
            }
        }
        return false;
    }

    private bool AreThereEnemiesRemaining()
    {
        for(int i = 0; i<enemies.Length; i++)
        {
            if(enemies[i] != null)
            {
                return true;
            }
        }
        return false;
    }

}
