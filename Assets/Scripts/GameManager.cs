using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Characters;
using BattleSystem.Data;
using BattleSystem.Interface;

namespace BattleSystem.Gameplay
{
    /// <summary>
    /// The GameManager is the main component of the battle system, which sets up the battle and every creature's turn.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// The player characters that will be used for the battle.
        /// </summary>
        [SerializeField]
        private Player[] players;

        /// <summary>
        /// The types of enemies that can be used for the battle.
        /// </summary>
        [SerializeField]
        private Enemy[] avaliableEnemies;

        [SerializeField]
        private GameObject[] playerSpawnPoints, enemySpawnPoints;

        /// <summary>
        /// The enemies that will be used in the battle.
        /// </summary>
        private Enemy[] enemies;
        /// <summary>
        /// The minimum number of enemies that can appear in a battle.
        /// </summary>
        readonly int minEnemies = 2;
        /// <summary>
        /// The maximum number of enemies that can appear in a battle.
        /// </summary>
        readonly int maxEnemies = 5;

        /// <summary>
        /// A list of every character present in the battle.
        /// </summary>
        private Character[] characters;

        /// <summary>
        /// A reference to the game data.
        /// </summary>
        GameData gameData;

        /// <summary>
        /// The current state of the battle.
        /// </summary>
        BattleState battleState = BattleState.Start;

        /// <summary>
        /// A list of the order in which the characters will act.
        /// </summary>
        ArrayList turnOrder = new ArrayList();
        /// <summary>
        /// The index of the current player's turn.
        /// </summary>
        /// <remarks>This is used with the turnOrder ArrayList to find the current turn player.</remarks>
        int currentPlayerInTurn = 0;

        /// <summary>
        /// A reference to the PlayerUIManager.
        /// </summary>
        PlayerUIManager playerUI;
        /// <summary>
        /// A reference to the BattleMessages.
        /// </summary>
        BattleMessages battleMessenger;

        /// <summary>
        /// The delay between turns.
        /// </summary>
        float cooldown = 0f;

        /// <summary>
        /// On startup, the GameManager sets up the characters and the UI.
        /// </summary>
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

            enemies = new Enemy[Random.Range(minEnemies, maxEnemies+1)];
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
            battleMessenger = FindObjectOfType<BattleMessages>();
            if (battleMessenger == null) { Debug.LogError("Could not find Battle Messenger."); }

            //Sort the players and enemies to form a turn track
            SortCharacters();

            CreateTrack();
        }

        /// <summary>
        /// On update, if it is time to start the next turn, the next character's turn is set up.
        /// </summary>
        /// <remarks>If it's a player's turn, the player menu is set up.
        /// If it's an enemy, the enemy automatically does their turn.</remarks>
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
                            battleMessenger.UpdateMessage("It's " + GetCurrentTurnPlayer().GetCharacterName() + "'s turn!");
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

        /// <summary>
        /// Handles and resolves an attack made by a character.
        /// </summary>
        /// <param name="targetID">The ID of the character that is being attacked.</param>
        /// <param name="attackAll">If attacking all characters, set to true so that the turn doesn't immediately end.</param>
        /// <param name="useMagicAttackStat">If magic is being used, this is set to true.</param>
        /// <param name="magicStrength">Strength of magic being used, if any.</param>
        public void Attack(int targetID, bool attackAll, bool useMagicAttackStat, int magicStrength)
        {
            int currentTurnPlayerID = GetCurrentTurnPlayerID();
            Character attackingCharacter = GetCharacter(currentTurnPlayerID);
            Character defendingCharacter = GetCharacter(targetID);

            if (useMagicAttackStat)
            {
                //Use magic on the target.
                attackingCharacter.Attack(defendingCharacter, CalculateDamage(attackingCharacter.GetMagicAttack() + magicStrength, defendingCharacter.GetMagicDefence()));
            }
            else
            {
                battleMessenger.UpdateMessage(attackingCharacter.GetCharacterName() + " attacks!");
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

        /// <summary>
        /// Attack all characters on the enemy side.
        /// </summary>
        /// <remarks>In the current implementation, this action can only be done by players using magic.</remarks>
        /// <param name="magicStrength">The strength of the magic being used.</param>
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

        /// <summary>
        /// Handles restoring health to a character.
        /// </summary>
        /// <param name="targetID">The ID of the character that is being healed.</param>
        /// <param name="healAll">If healing all characters, set to true so that the turn doesn't immediately end.</param>
        /// <param name="magicStrength">Strength of magic being used to heal.</param>
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

        /// <summary>
        /// Restore health to all players.
        /// </summary>
        /// <remarks>In the current implementation, this action can only be done by players.</remarks>
        /// <param name="magicStrength">Strength of magic being used to heal.</param>
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

        /// <summary>
        /// Revive an unconscious player.
        /// </summary>
        /// <param name="playerID">The ID of the player being revived.</param>
        public void RevivePlayer(int playerID)
        {
            Player revivingPlayer = (Player)GetCharacter(playerID);
            revivingPlayer.Revive();
        }

        /// <summary>
        /// Sets up the order that characters will attack in a round.
        /// </summary>
        /// <remarks>This is called at the start of a new round.
        /// Characters are sorted according to their speed values.</remarks>
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

        /// <summary>
        /// Handles an enemy's turn in combat.
        /// </summary>
        /// <remarks>The enemy will decide who to target and then either attack or heal that target.</remarks>
        private void EnemyTurn()
        {
            int currentTurnPlayerID = GetCurrentTurnPlayerID();
            Enemy attackingEnemy = (Enemy)GetCharacter(currentTurnPlayerID);
            if (attackingEnemy != null)
            {
                int targetID = attackingEnemy.DecideTarget(players, enemies);
                if (attackingEnemy.GetAIComponent().GetState() == EnemyState.Defensive)
                {
                    battleMessenger.UpdateMessage(attackingEnemy.GetCharacterName() + " uses healing magic!");
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

        /// <summary>
        /// Calculates damage dealt by an attack.
        /// </summary>
        /// <remarks>There is some randomness applied to the damage so that it's not always the same.</remarks>
        /// <param name="attack">The attacking character's attack value.</param>
        /// <param name="defence">The defending character's defence value.</param>
        /// <returns></returns>
        int CalculateDamage(int attack, int defence)
        {
            int damage = Mathf.RoundToInt((attack - defence) * Random.Range(0.85f, 1f));
            if (damage < 0) { damage = 0; }
            return damage;
        }

        /// <summary>
        /// Sets up the next character's turn.
        /// </summary>
        /// <remarks>If there are no players or enemies left, the game ends.
        /// If the last character in the turn order has finished their turn, a new round is started.</remarks>
        /// <param name="startCooldown">If true, a delay will be set before the next turn starts.</param>
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

        /// <summary>
        /// Start a new round of combat.
        /// </summary>
        /// <remarks>The turn order is cleared and a new one is made.</remarks>
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

        /// <summary>
        /// Attempt to escape from the battle.
        /// </summary>
        /// <remarks>The odds of escaping are based on the total health values of the enemies.
        /// If an escape is successful, the battle ends.  If the escape fails, the player's turn immediately ends.</remarks>
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
                battleMessenger.UpdateMessage("Escaped successfully!");
                battleState = BattleState.Victory;
                EndBattle();
            }
            else
            {
                battleMessenger.UpdateMessage("You couldn't escape!");
                NextTurn(true);
            }
        }

        /// <summary>
        /// End the battle in a victory.
        /// </summary>
        public void Victory()
        {
            battleState = BattleState.Victory;
            battleMessenger.UpdateMessage("You win!");
            EndBattle();
        }

        /// <summary>
        /// End the battle in defeat.
        /// </summary>
        public void GameOver()
        {
            battleState = BattleState.Defeat;
            battleMessenger.UpdateMessage("Game over.");
            EndBattle();
        }

        /// <summary>
        /// Stop the battle by disabling the battle menus.
        /// </summary>
        private void EndBattle()
        {
            playerUI.HideBattleMenu();
            playerUI.HideTargetMarker();
        }

        /// <summary>
        /// Remove an enemy from the list of enemies in the battle.
        /// </summary>
        /// <param name="enemyID">The ID of the enemy to be removed.</param>
        public void RemoveEnemy(int enemyID)
        {
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

        /// <summary>
        /// Create the turn order track that is displayed in the battle.
        /// </summary>
        /// <remarks>This is called whenever the turn order track needs to be updated.</remarks>
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

        /// <summary>
        /// Get the character sprite that will be used for the track.
        /// </summary>
        /// <remarks>This is used u=in conjunction with the CreateTrack method.</remarks>
        /// <param name="sprites">The current list of sprites.</param>
        /// <param name="currentID">The ID of the character whos sprite is needed.</param>
        /// <param name="i">The current position within the for loop.</param>
        private void GetCharacterSpriteForTrack(ArrayList sprites, int currentID, int i)
        {
            int characterID = characters[i].GetID();
            if (currentID == characterID)
            {
                sprites.Add(characters[i].GetSprite());
            }
        }

        /// <summary>
        /// Retrieve a character from the avaliable characters in the battle.
        /// </summary>
        /// <param name="id">The ID of the character that is needed.</param>
        /// <returns>The character if found, otherwise null.</returns>
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

        /// <summary>
        /// Retrieve the player who's currently having their turn.
        /// </summary>
        /// <returns>The player if found, otherwise null.</returns>
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

        /// <summary>
        /// Retrieve the ID of the player that's currently having their turn.
        /// </summary>
        /// <returns>The player's ID.</returns>
        public int GetCurrentTurnPlayerID()
        {
            return (turnOrder[currentPlayerInTurn] as TurnData).GetID();
        }

        /// <summary>
        /// Get the data of all players that are currently alive.
        /// </summary>
        /// <remarks>This is used for the target marker when using magic that only affects alive players.</remarks>
        /// <returns>An array of all players that are alive.</returns>
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

        /// <summary>
        /// Get the data of all players that are currently unconscious.
        /// </summary>
        /// <remarks>This is used for the target marker when using magic that revives players.</remarks>
        /// <returns>An array of all players that are unconscious.</returns>
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

        /// <summary>
        /// Get the data of all enemies that are currently alive.
        /// </summary>
        /// <remarks>This is used for the target marker when attacking an enemy.</remarks>
        /// <returns>An array of all enemies that are alive.</returns>
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

        /// <summary>
        /// Checks if there are any players who are still alive.
        /// </summary>
        /// <remarks>This is used to check if the battle should end.</remarks>
        /// <returns>False if no players are alive, true if there are.</returns>
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

        /// <summary>
        /// Checks if there are any players who are unconscious.
        /// </summary>
        /// <returns>False if no players are unconscious, true if there are.</returns>
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

        /// <summary>
        /// Checks if there are any enemies who are still alive.
        /// </summary>
        /// <remarks>This is used to check if the battle should end.</remarks>
        /// <returns>False if no enemies are alive, true if there are.</returns>
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
