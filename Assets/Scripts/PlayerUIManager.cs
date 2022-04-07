using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleSystem.Interface;
using BattleSystem.Characters;
using BattleSystem.Data;

namespace BattleSystem.Gameplay
{
    /// <summary>
    /// The PlayerUIManager handles navigation through the player menus, and stores data based on what the player has selected.
    /// </summary>
    public class PlayerUIManager : MonoBehaviour
    {
        /// <summary>
        /// A reference to the GameManager.
        /// </summary>
        GameManager gameManager;

        /// <summary>
        /// A reference to the PlayerBattleMenu.
        /// </summary>
        [SerializeField]
        PlayerBattleMenu battleMenu;

        /// <summary>
        /// A reference to the PlayerMagicMenu.
        /// </summary>
        [SerializeField]
        PlayerMagicMenu magicMenu;

        /// <summary>
        /// A reference to the TargetMarker.
        /// </summary>
        TargetMarker targetMarker;

        /// <summary>
        /// The current turn player.
        /// </summary>
        Player currentPlayer;

        //Magic
        /// <summary>
        /// The magic that the current turn player has access to.
        /// </summary>
        ArrayList playerMagicInfomation = new ArrayList();
        /// <summary>
        /// The magic that has been selected by the player.
        /// </summary>
        Magic.MagicStats selectedMagic;
        /// <summary>
        /// The maximum amount of magic buttons that will be displayed in the PlayerMagicMenu at one time.
        /// </summary>
        readonly int maxMagicButtonsDisplayed = 4;
        /// <summary>
        /// Indicates if the player is using magic or not.
        /// </summary>
        bool isUsingMagic = false;

        /// <summary>
        /// On Start, set up the separate menus.
        /// </summary>
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            if(gameManager == null) { Debug.LogError("Could not find the Game Manager"); }
            if (battleMenu == null) { Debug.LogError("Could not find the Battle Menu"); }
            if (magicMenu == null) { Debug.LogError("Could not find the Magic Menu"); }
            targetMarker = FindObjectOfType<TargetMarker>();
            if (targetMarker == null) { Debug.LogError("Could not find the Target Marker"); }

            HideTargetMarker();
        }

        /// <summary>
        /// Update checks for player input from any of the menus.
        /// </summary>
        void Update()
        {
            ///Check if a selection has been made in one of the menus.
            if (battleMenu.IsButtonSelected())
            {
                battleMenu.ButtonConfirmed();
                HandleBattleMenuSelection();
            } else if (magicMenu.IsButtonSelected())
            {
                magicMenu.ButtonConfirmed();
                HandleMagicMenuSelection();
            } else if (targetMarker.IsTargetSelected())
            {
                targetMarker.TargetConfirmed();
                ConfirmTarget();
            }

            //Check if a menu has cancelled selection
            if (magicMenu.HasCancelled())
            {
                magicMenu.CancelConfirmed();
                HideMagicMenu();
            } else if (targetMarker.HasCancelled())
            {
                targetMarker.CancelConfirmed();
                DisplayBattleMenu();
                HideTargetMarker();
            }
        }

        #region BattleMenu
        /// <summary>
        /// Performs an action depending on which button from the battle menu was selected.
        /// </summary>
        private void HandleBattleMenuSelection()
        {
            int highlightedButton = battleMenu.GetHighlightedButton();
            switch (highlightedButton)
            {
                case 0:
                    //Attack
                    SelectAttack();
                    break;
                case 1:
                    //Magic
                    SelectMagic();
                    break;
                case 2:
                    //Guard
                    SelectGuard();
                    break;
                case 3:
                    //Flee
                    SelectFlee();
                    break;
            }
        
        }

        /// <summary>
        /// Sets up the target marker for selecting an enemy to attack.
        /// </summary>
        /// <remarks>This is called if the attack button is selected in the battle menu.</remarks>
        private void SelectAttack()
        {
            targetMarker.SetTargets(gameManager.GetEnemyData());
            isUsingMagic = false;
            DisplayMarker();
            HideBattleMenu();
        }

        /// <summary>
        /// Sets up the magic menu for selecting which magic to use.
        /// </summary>
        /// <remarks>This is called if the magic button is selected in the battle menu.</remarks>
        private void SelectMagic()
        {
            //Can only use if the character knows magic
            if (gameManager.GetCurrentTurnPlayer().CanUseMagic())
            {
                DisplayMagicMenu();
                battleMenu.DisableMenu(true);
                playerMagicInfomation.Clear();
                GetMagicInfomation();
            }
        }

        /// <summary>
        /// Makes the player perform a guard action.
        /// </summary>
        /// <remarks>This is called if the guard button is selected in the battle menu.</remarks>
        private void SelectGuard()
        {
            gameManager.GetCurrentTurnPlayer().UseGuard();
            gameManager.NextTurn(true);
            HideBattleMenu();
        }

        /// <summary>
        /// Makes an attempt to escape from the battle.
        /// </summary>
        /// <remarks>This is called if the flee button is selected in the battle menu.</remarks>
        private void SelectFlee()
        {
            gameManager.AttemptEscape();
            HideBattleMenu();
        }
        #endregion

        #region MagicMenu
        /// <summary>
        /// Resolves selection of magic.
        /// </summary>
        /// <remarks>If the magic affects everyone, use it immediately on all targets.  If it targets one character, store the data and allow the player to select a target.</remarks>
        private void HandleMagicMenuSelection()
        {
            int highlightedButton = magicMenu.GetHighlightedButton();
            //The player needs enough MP in order to use the magic
            if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicCost <= currentPlayer.GetCurrentMagic())
            {
                if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsDead && !gameManager.AnyUnconsciousPlayers()) { return; }
                HideMagicMenu();
                if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsAll)
                {
                    //Automatically apply the effect to either the players or enemies
                    FindObjectOfType<BattleMessages>().UpdateMessage(currentPlayer.GetCharacterName() + " casts " + ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicName + "!");
                    HideBattleMenu();
                    currentPlayer.StartCoroutine("LoseMagic", ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicCost);
                    if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsPlayers)
                    {
                        gameManager.HealAll(((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicStrength);
                    }
                    else
                    {
                        gameManager.AttackAll(((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicStrength);
                    }
                    return;
                }

                if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsPlayers)
                {
                    //Set up the target marker so that only players can be selected
                    if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsDead)
                    {
                        targetMarker.SetTargets(gameManager.GetUnconsciousPlayers());
                    }
                    else
                    {
                        targetMarker.SetTargets(gameManager.GetAlivePlayers());
                    }
                    isUsingMagic = true;
                    DisplayMarker();
                    selectedMagic = (Magic.MagicStats)playerMagicInfomation[highlightedButton];
                }
                else
                {
                    //Set up the target marker to target enemies as usual
                    targetMarker.SetTargets(gameManager.GetEnemyData());
                    isUsingMagic = true;
                    DisplayMarker();
                    selectedMagic = (Magic.MagicStats)playerMagicInfomation[highlightedButton];
                }
                HideBattleMenu();
            }
        }

        /// <summary>
        /// Gets infomation on what magic the player can use, and stores the infomation.
        /// </summary>
        private void GetMagicInfomation()
        {
            Image[] magicButtons = magicMenu.GetMagicButtons();
            bool[] knownMagic = gameManager.GetCurrentTurnPlayer().GetKnownMagic();
            int playerMagicAmount = gameManager.GetCurrentTurnPlayer().GetCurrentMagic();
            int numberOfAvaliableMagic = 0;
            for (int i = 0; i < knownMagic.Length; i++)
            {
                if (knownMagic[i])
                {
                    //Obtain infomation about the magic and set it to the text
                    Magic.MagicStats magic = FindObjectOfType<Magic>().GetMagicInfomation(i);
                    playerMagicInfomation.Add(magic);
                    if (magicButtons[numberOfAvaliableMagic] != null)
                    {
                        magicButtons[numberOfAvaliableMagic].GetComponentInChildren<Text>().text = ((Magic.MagicStats)playerMagicInfomation[numberOfAvaliableMagic]).magicName;
                    }
                    else
                    {
                        Debug.LogError("Not enough magic buttons in the menu.");
                    }
                    if (magic.magicCost > playerMagicAmount)
                    {
                        //Indicate that the player does not have enough mp to use the magic
                        magicButtons[numberOfAvaliableMagic].GetComponentInChildren<Text>().color = new Color(1f, 0.5f, 0.5f);
                    }
                    else
                    {
                        magicButtons[numberOfAvaliableMagic].GetComponentInChildren<Text>().color = new Color(0f, 0f, 0f);
                    }
                    numberOfAvaliableMagic++;
                }
            }
            //If there is less avaliable magic than the max amount of buttons that can be displayed, disable the arrows.
            if (numberOfAvaliableMagic <= maxMagicButtonsDisplayed)
            {
                magicMenu.ActiveArrows(false, false);
                for (int i = 0; i < maxMagicButtonsDisplayed; i++)
                {
                    if (i >= numberOfAvaliableMagic)
                    {
                        magicButtons[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                magicMenu.ActiveArrows(false, true);
                for (int i = 0; i < maxMagicButtonsDisplayed; i++)
                {
                    magicButtons[i].gameObject.SetActive(true);
                }
            }

            magicMenu.UpdateDescription(((Magic.MagicStats)playerMagicInfomation[magicMenu.GetHighlightedButton()]).magicCost.ToString(), ((Magic.MagicStats)playerMagicInfomation[magicMenu.GetHighlightedButton()]).magicDescription);
            UpdateMagicInfomation();
            magicMenu.UpdateNumberOfAvaliableMagic(playerMagicInfomation.Count);
        }

        /// <summary>
        /// Compiles infomation on each magic the player has and sends it to the magic menu.
        /// </summary>
        private void UpdateMagicInfomation()
        {
            string[] magicCostString = new string[playerMagicInfomation.Count];
            string[] magicDescryptionString = new string[playerMagicInfomation.Count];
            for(int i=0; i<playerMagicInfomation.Count; i++)
            {
                magicCostString[i] = ((Magic.MagicStats)playerMagicInfomation[i]).magicCost.ToString();
                magicDescryptionString[i] = ((Magic.MagicStats)playerMagicInfomation[i]).magicDescription;
            }
            magicMenu.UpdateMagicInfomation(magicCostString, magicDescryptionString);
        }

        #endregion

        #region TargetMarker
        /// <summary>
        /// Resolves chosen player action on the selected target.
        /// </summary>
        /// <remarks>Depending on the action chosen, the target will either be attacked, healed, or revived.</remarks>
        private void ConfirmTarget()
        {
            TargetData chosenTarget = targetMarker.GetChosenTarget();
            if (isUsingMagic)
            {
                currentPlayer.StartCoroutine("LoseMagic", selectedMagic.magicCost);
                FindObjectOfType<BattleMessages>().UpdateMessage(currentPlayer.GetCharacterName() + " casts " + (selectedMagic.magicName + "!"));
                if (selectedMagic.restores)
                {
                    if (selectedMagic.affectsDead && chosenTarget.IsTargetPlayer())
                    {
                        gameManager.RevivePlayer(chosenTarget.GetTargetID());
                    }
                    gameManager.Heal(chosenTarget.GetTargetID(), false, selectedMagic.magicStrength);
                }
                else
                {
                    gameManager.Attack(chosenTarget.GetTargetID(), false, true, selectedMagic.magicStrength);
                }
            }
            else
            {
                gameManager.Attack(chosenTarget.GetTargetID(), false, false, 0);
            }
            HideTargetMarker();
        }
        #endregion

        /// <summary>
        /// Display the battle menu.
        /// </summary>
        public void DisplayBattleMenu()
        {
            battleMenu.gameObject.SetActive(true);
            battleMenu.ResetHighlightedButton();
            battleMenu.ResetHighlightedButton();
            battleMenu.DisableMenu(false);
            HideMagicMenu();
        }

        /// <summary>
        /// Display the battle menu.
        /// </summary>
        /// <param name="player">The current turn player.</param>
        public void DisplayBattleMenu(Player player)
        {
            battleMenu.gameObject.SetActive(true);
            battleMenu.ResetHighlightedButton();
            battleMenu.SetMagicButtonText(player.CanUseMagic());
            battleMenu.ResetHighlightedButton();
            battleMenu.DisableMenu(false);
            HideMagicMenu();
            currentPlayer = player;
        }

        /// <summary>
        /// Hide the battle menu.
        /// </summary>
        public void HideBattleMenu()
        {
            battleMenu.gameObject.SetActive(false);
        }

        /// <summary>
        /// Display the magic menu.
        /// </summary>
        public void DisplayMagicMenu()
        {
            magicMenu.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide the magic menu.
        /// </summary>
        public void HideMagicMenu()
        {
            magicMenu.gameObject.SetActive(false);
            battleMenu.DisableMenu(false);
        }

        /// <summary>
        /// Display the target marker.
        /// </summary>
        public void DisplayMarker()
        {
            targetMarker.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide the target marker.
        /// </summary>
        public void HideTargetMarker()
        {
            targetMarker.gameObject.SetActive(false);
        }

        /// <summary>
        /// Retrieve the target marker from the UI Manager.
        /// </summary>
        /// <returns>The target marker.</returns>
        public TargetMarker GetTargetMarker()
        {
            return targetMarker;
        }

        /// <summary>
        /// Checks to see if the battle menu is currently active.
        /// </summary>
        /// <returns>True if the battle menu is active, false if it is not.</returns>
        public bool BattleMenuActive()
        {
            return battleMenu.gameObject.activeInHierarchy;
        }

        /// <summary>
        /// Checks to see if the target marker is currently active.
        /// </summary>
        /// <returns>True if the target marker is active, false if it is not.</returns>
        public bool TargetMarkerActive()
        {
            return targetMarker.gameObject.activeInHierarchy;
        }
    }
}
