using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    float cooldown = 0f;

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

        CreateTrack();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown <= 0)
        {
            CreateTrack();
            if (battleState != BattleState.Victory && battleState != BattleState.Defeat)
            {
                if ((turnOrder[currentPlayerInTurn] as TurnData).IsPlayer())
                {
                    //Stop, and let the player choose who to attack
                    int currentTurnPlayerID = GetCurrentTurnPlayerID();
                    Player currentTurnPlayer = (Player)GetCharacter(currentTurnPlayerID);
                    if (!currentTurnPlayer.IsConscious())
                    {
                        //If the player is unconscious, skip their turn and move on to the next character
                        NextTurn(false);
                        return;
                    }
                    if (!playerMenu.gameObject.activeInHierarchy && battleState != BattleState.PlayerTurnSelectTarget && battleState != BattleState.PlayerTurnAttack)
                    {
                        if (currentTurnPlayer.IsGuarding())
                        {
                            currentTurnPlayer.FinishGuard();
                        }
                        playerMenu.DisplayBattleMenu();
                        battleState = BattleState.PlayerTurnSelectMove;
                        FindObjectOfType<BattleMessages>().UpdateMessage("It's " + GetCurrentTurnPlayer().GetCharacterName() + "'s turn!");
                    }
                }
                else
                {
                    //The next character is an enemy.  Attack a player character
                    battleState = BattleState.EnemyTurn;
                    EnemyTurn();
                }
            }
        } else
        {
            cooldown -= Time.deltaTime;
        }
    }

    public void Attack(int targetID)
    {
        int currentTurnPlayerID = GetCurrentTurnPlayerID();
        Character attackingCharacter = GetCharacter(currentTurnPlayerID);
        Character defendingCharacter = GetCharacter(targetID);
        if (defendingCharacter.IsPlayer())
        {
            Player defendingPlayer = (Player)defendingCharacter;
            if (defendingPlayer.IsGuarding())
            {
                Debug.Log("Guarded!");
                attackingCharacter.Attack(defendingCharacter, CalculateDamage(attackingCharacter.GetAttack(), defendingCharacter.GetDefence() * 2));
                return;
            }
        }
        attackingCharacter.Attack(defendingCharacter, CalculateDamage(attackingCharacter.GetAttack(), defendingCharacter.GetDefence()));
    }

    private void SortCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == null) { continue; }
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
                foreach (TurnData data in turnOrder)
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
        int currentTurnPlayerID = GetCurrentTurnPlayerID();
        Enemy attackingEnemy = (Enemy)GetCharacter(currentTurnPlayerID);
        if (attackingEnemy != null)
        {
            int randomTarget = Random.Range(0, players.Length);
            //If the enemy is targetting an unconscious player, find a new target until the enemy is targetting a conscious player
            while (!players[randomTarget].IsConscious())
            {
                randomTarget = Random.Range(0, players.Length);
            }
            int targetID = players[randomTarget].GetID();
            Attack(targetID);
        } else
        {
            //Enemy does not exist and should be ignored.
            NextTurn(false);
        }
    }

    int CalculateDamage(int attack, int defence)
    {
        int damage = (attack - defence) + Random.Range(-5, 5);
        if (damage < 0) { damage = 0; }
        return damage;
    }

    public void NextTurn(bool startCooldown)
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
        battleState = BattleState.Start;

        if (currentPlayerInTurn > turnOrder.Count - 1)
        {
            //Round of combat has ended
            NewRound();
        }
        if (startCooldown)
        {
            cooldown = 1f;
        }
        //TODO find a way to use a coroutine instead
        //The cooldown timer is still used even when the enemy has been defeated
    }

    public void NewRound()
    {
        currentPlayerInTurn = 0;
        //Reset the turn order so that it's not always the same
        turnOrder.Clear();
        SortCharacters();
    }

    public void Victory()
    {
        battleState = BattleState.Victory;
        FindObjectOfType<BattleMessages>().UpdateMessage("You win!");
        EndBattle();
    }

    public void GameOver()
    {
        battleState = BattleState.Defeat;
        FindObjectOfType<BattleMessages>().UpdateMessage("Game over.");
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
                }
            }
        }

        for (int i=0; i<turnOrder.Count; i++)
        {
            if((turnOrder[i] as TurnData).GetID() == enemyID)
            {
                turnOrder.RemoveAt(i);
            }
        }

        targetMarker.SetEnemyTargets(enemies);

        CreateTrack();
    }

    public void CreateTrack()
    {
        ArrayList sprites = new ArrayList();
        int counter = 0;
        foreach (TurnData listEntry in turnOrder)
        {
            if (counter >= currentPlayerInTurn)
            {
                int currentID = listEntry.GetID();
                for (int i = 0; i < characters.Length; i++)
                {
                    if (characters[i] != null)
                    {
                        if (characters[i].IsPlayer())
                        {
                            Player currentPlayer = (Player)characters[i];
                            if (currentPlayer.IsConscious())
                            {
                                int characterID = characters[i].GetID();
                                if (currentID == characterID)
                                {
                                    sprites.Add(characters[i].GetSprite());
                                }
                                //TODO refactor to remove duplicate code
                            }
                        }
                        else 
                        {
                            int characterID = characters[i].GetID();
                            if (currentID == characterID)
                            {
                                sprites.Add(characters[i].GetSprite());
                            }
                        }
                    }
                }
            }
            counter++;
        }
        FindObjectOfType<TurnOrderTrack>().UpdateTrack(sprites);
    }

    public void SetStatePlayerSelectMove()
    {
        battleState = BattleState.PlayerTurnSelectMove;
    }

    public void SetStatePlayerSelectTarget()
    {
        battleState = BattleState.PlayerTurnSelectTarget;
    }

    public Player GetCurrentTurnPlayer()
    {
        if ((turnOrder[currentPlayerInTurn] as TurnData).IsPlayer())
        {
            int currentTurnPlayerID = GetCurrentTurnPlayerID();
            Player currentTurnPlayer = (Player)GetCharacter(currentTurnPlayerID);
            return currentTurnPlayer;
        }
        return null;
    }

    public int GetCurrentTurnPlayerID()
    {
        return (turnOrder[currentPlayerInTurn] as TurnData).GetID();
    }

    public Player[] GetPlayers()
    {
        return players;
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
