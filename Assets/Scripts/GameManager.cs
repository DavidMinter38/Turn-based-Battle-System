using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Characters;
using BattleSystem.Data;
using BattleSystem.Interface;

namespace BattleSystem.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Player[] players;

        [SerializeField]
        private Enemy[] avaliableEnemies;

        [SerializeField]
        private GameObject[] playerSpawnPoints, enemySpawnPoints;

        private Enemy[] enemies;
        int minEnemies = 2;
        int maxEnemies = 6;

        private Character[] characters;

        GameData gameData;

        BattleState battleState = BattleState.Start;

        ArrayList turnOrder = new ArrayList();
        int currentPlayerInTurn = 0;

        PlayerUIManager playerUI;
        BattleMessages battleMessegner;

        float cooldown = 0f;

        // Start is called before the first frame update
        void Start()
        {
            gameData = FindObjectOfType<GameData>();
            if(gameData == null) { Debug.LogError("Could not find game data."); }
            int characterID = 0;

            //If there are no players, send an error message.
            if (players.Length <= 0) { Debug.LogError("Could not find player characters."); }

            for(int i=0; i < players.Length; i++)
            {
                GameData.PlayerStats currentPlayerStats = gameData.GetPlayerStats(players[i].GetPlayerID());
                if (currentPlayerStats.isAvaliable)
                {
                    Player thePlayer = Instantiate(players[i], playerSpawnPoints[i].transform.position, Quaternion.identity);
                    thePlayer.SetID(characterID);
                    characterID++;
                    players[i] = thePlayer;
                }
            }

            //Create a random selection of enemies from a pool and spawn them into the scene
            if (avaliableEnemies.Length <= 0) { Debug.LogError("No enemies are avaliable."); }

            enemies = new Enemy[Random.Range(minEnemies, maxEnemies)];
            for(int i=0; i<enemies.Length; i++)
            {
                Enemy theEnemy = Instantiate(avaliableEnemies[Random.Range(0, avaliableEnemies.Length)], enemySpawnPoints[i].transform.position, Quaternion.identity);
                theEnemy.SetID(characterID);
                characterID++;
                enemies[i] = theEnemy;
            }

            characters = FindObjectsOfType<Character>();

            //Find the UI used in the battle
            playerUI = FindObjectOfType<PlayerUIManager>();
            if (playerUI == null) { Debug.LogError("Could not find Player UI Manager."); }
            playerUI.HideBattleMenu();
            battleMessegner = FindObjectOfType<BattleMessages>();
            if (battleMessegner == null) { Debug.LogError("Could not find Battle Messenger."); }

            //Sort the players and enemies to form a turn track
            SortCharacters();

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
                        if (!currentTurnPlayer.IsConscious() || !currentTurnPlayer.IsInCombat())
                        {
                            //If the player is unconscious, skip their turn and move on to the next character
                            NextTurn(false);
                            return;
                        }
                        if (!playerUI.BattleMenuActive() && !playerUI.TargetMarkerActive() && battleState != BattleState.PlayerTurnAttack)
                        {
                            if (currentTurnPlayer.IsGuarding())
                            {
                                currentTurnPlayer.FinishGuard();
                            }
                            playerUI.DisplayBattleMenu(currentTurnPlayer);
                            battleState = BattleState.PlayerTurnSelectMove;
                            battleMessegner.UpdateMessage("It's " + GetCurrentTurnPlayer().GetCharacterName() + "'s turn!");
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
            else
            {
                cooldown -= Time.deltaTime;
            }
        }

        public void Attack(int targetID, bool attackAll, bool useMagicAttackStat, int magicStrength)
        {
            int currentTurnPlayerID = GetCurrentTurnPlayerID();
            Character attackingCharacter = GetCharacter(currentTurnPlayerID);
            Character defendingCharacter = GetCharacter(targetID);

            if (useMagicAttackStat)
            {
                attackingCharacter.Attack(defendingCharacter, CalculateDamage(attackingCharacter.GetMagicAttack() + magicStrength, defendingCharacter.GetMagicDefence()));
            }
            else
            {
                battleMessegner.UpdateMessage(attackingCharacter.GetCharacterName() + " attacks!");
                //Check if attacking a guarding player
                if (defendingCharacter.IsPlayer())
                {
                    Player defendingPlayer = (Player)defendingCharacter;
                    if (defendingPlayer.IsGuarding())
                    {
                        attackingCharacter.Attack(defendingCharacter, (CalculateDamage(attackingCharacter.GetAttack(), defendingCharacter.GetDefence()) / 2));
                    }
                    else
                    {
                        attackingCharacter.Attack(defendingCharacter, CalculateDamage(attackingCharacter.GetAttack(), defendingCharacter.GetDefence()));
                    }
                }
                else
                {
                    attackingCharacter.Attack(defendingCharacter, CalculateDamage(attackingCharacter.GetAttack(), defendingCharacter.GetDefence()));
                }
            }

            if (!defendingCharacter.IsPlayer())
            {
                //Mark the player so that the enemy knows who to target
                Enemy defendingEnemy = (Enemy)defendingCharacter;
                defendingEnemy.GetAIComponent().MarkAttacker(currentTurnPlayerID);
            }

            if (!attackAll)
            {
                NextTurn(true);
            }
        }

        public void AttackAll(int magicStrength)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    Attack(enemy.GetID(), true, true, magicStrength);
                }
            }
            NextTurn(true);
        }

        public void Heal(int targetID, bool healAll, int magicStrength)
        {
            int currentTurnPlayerID = GetCurrentTurnPlayerID();
            Character healingCharacter = GetCharacter(currentTurnPlayerID);
            Character targettedCharacter = GetCharacter(targetID);

            healingCharacter.Heal(targettedCharacter, magicStrength);

            if (!healAll)
            {
                NextTurn(true);
            }
        }

        public void HealAll(int magicStrength)
        {
            foreach (Player player in players)
            {
                if (player.IsConscious())
                {
                    Heal(player.GetID(), true, magicStrength);
                }
            }
            NextTurn(true);
        }

        public void RevivePlayer(int playerID)
        {
            Player revivingPlayer = (Player)GetCharacter(playerID);
            revivingPlayer.Revive();
        }

        //Sorts each character accoriding to their speed values.  This is done so that the turn order is not always the same.
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
                int targetID = attackingEnemy.DecideTarget(players, enemies);
                if (attackingEnemy.GetAIComponent().GetState() == EnemyState.Defensive)
                {
                    battleMessegner.UpdateMessage(attackingEnemy.GetCharacterName() + " uses healing magic!");
                    Heal(targetID, false, attackingEnemy.GetMagicAttack());
                }
                else
                {
                    Attack(targetID, false, false, 0);
                }
            }
            else
            {
                //Enemy does not exist and should be ignored
                NextTurn(false);
            }
        }

        int CalculateDamage(int attack, int defence)
        {
            int damage = Mathf.RoundToInt((attack - defence) * Random.Range(0.85f, 1f));
            if (damage < 0) { damage = 0; }
            return damage;
        }

        public void NextTurn(bool startCooldown)
        {
            //Remove any enemies from the scene that have 0 hit points
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    if (!enemies[i].IsAlive())
                    {
                        RemoveEnemy(enemies[i].GetID());
                    }
                }
            }

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

            //The cooldown timer allows for a delay between each turn.  The timer is not used if a character is being skipped.
            if (startCooldown)
            {
                cooldown = 1f;
            }
        }

        public void NewRound()
        {
            //Allow any players who were revived this round to fight in the next round
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsConscious() && !players[i].IsInCombat())
                {
                    players[i].ReturnToCombat();
                }
            }

            currentPlayerInTurn = 0;
            //Reset the turn order so that it's not always the same
            turnOrder.Clear();
            SortCharacters();
        }

        public void AttemptEscape()
        {
            //Get the percentage of enemy hp remaining and multiply by 0.25
            //Then, use a random number between 0 and 1
            //If higher than the resulting percentage, escape successfull.  If not, escape fails.
            float totalEnemyCurrentHP = 0;
            float totalEnemyMaxHP = 0;
            float numberOfDefeatedEnemies = 0;
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    totalEnemyCurrentHP += enemies[i].GetCurrentHealth();
                    totalEnemyMaxHP += enemies[i].GetMaxHealth();
                }
                else
                {
                    numberOfDefeatedEnemies++;
                }
            }

            float percentageOfEscape = (totalEnemyCurrentHP / totalEnemyMaxHP) * (0.25f / (numberOfDefeatedEnemies + 1));
            float randomNumber = Random.Range(0f, 1f);
            if (randomNumber > percentageOfEscape)
            {
                battleMessegner.UpdateMessage("Escaped successfully!");
                battleState = BattleState.Victory;
                EndBattle();
            }
            else
            {
                battleMessegner.UpdateMessage("You couldn't escape!");
                NextTurn(true);
            }
        }

        public void Victory()
        {
            battleState = BattleState.Victory;
            battleMessegner.UpdateMessage("You win!");
            EndBattle();
        }

        public void GameOver()
        {
            battleState = BattleState.Defeat;
            battleMessegner.UpdateMessage("Game over.");
            EndBattle();
        }

        private void EndBattle()
        {
            playerUI.HideBattleMenu();
            playerUI.HideTargetMarker();
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
                        Destroy(characters[i].gameObject);
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

            for (int i = 0; i < turnOrder.Count; i++)
            {
                if ((turnOrder[i] as TurnData).GetID() == enemyID)
                {
                    turnOrder.RemoveAt(i);
                }
            }

            if (AreThereEnemiesRemaining())
            {
                playerUI.GetTargetMarker().SetTargets(GetEnemyData());
            }

            CreateTrack();
        }

        public void CreateTrack()
        {
            ArrayList sprites = new ArrayList();
            int counter = 0;  //Used so that only the characters that have not taken a turn this round are added to the track.
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
                                if (currentPlayer.IsConscious() && currentPlayer.IsInCombat())
                                {
                                    GetCharacterSpriteForTrack(sprites, currentID, i);
                                }
                            }
                            else
                            {
                                GetCharacterSpriteForTrack(sprites, currentID, i);
                            }
                        }
                    }
                }
                counter++;
            }
            FindObjectOfType<TurnOrderTrack>().UpdateTrack(sprites);
        }

        private void GetCharacterSpriteForTrack(ArrayList sprites, int currentID, int i)
        {
            int characterID = characters[i].GetID();
            if (currentID == characterID)
            {
                sprites.Add(characters[i].GetSprite());
            }
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

        public TargetData[] GetAlivePlayers()
        {
            TargetData[] alivePlayers = new TargetData[players.Length];
            int counter = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsConscious())
                {
                    alivePlayers[counter] = new TargetData(new Vector3(players[i].transform.position.x, players[i].transform.position.y, players[i].transform.position.z), players[i].GetID(), players[i].GetCharacterName(), players[i].IsPlayer());
                    counter++;
                }
            }
            //There should be at least one player alive, otherwise the game is over, so log an error if no players could be found
            if(alivePlayers.Length <= 0) { Debug.LogError("Could not find any alive players."); }
            return alivePlayers;
        }

        public TargetData[] GetUnconsciousPlayers()
        {
            TargetData[] unconsciousPlayers = new TargetData[players.Length];
            int counter = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].IsConscious())
                {
                    unconsciousPlayers[counter] = new TargetData(new Vector3(players[i].transform.position.x, players[i].transform.position.y, players[i].transform.position.z), players[i].GetID(), players[i].GetCharacterName(), players[i].IsPlayer()); ;
                    counter++;
                }
            }
            return unconsciousPlayers;
        }

        public TargetData[] GetEnemyData()
        {
            TargetData[] enemyData = new TargetData[enemies.Length];
            for(int i=0; i<enemies.Length; i++)
            {
                if(enemies[i] != null)
                {
                    enemyData[i] = new TargetData(new Vector3(enemies[i].transform.position.x, enemies[i].transform.position.y, enemies[i].transform.position.z), enemies[i].GetID(), enemies[i].GetCharacterName(), enemies[i].IsPlayer());
                }
            }
            return enemyData;
        }

        private bool AnyAlivePlayers()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsConscious())
                {
                    return true;
                }
            }
            return false;
        }

        public bool AnyUnconsciousPlayers()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].IsConscious())
                {
                    return true;
                }
            }
            return false;
        }

        private bool AreThereEnemiesRemaining()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
