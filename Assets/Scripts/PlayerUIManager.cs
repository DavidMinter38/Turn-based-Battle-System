using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleSystem.Interface;
using BattleSystem.Characters;
using BattleSystem.Data;

namespace BattleSystem.Gameplay
{
    //Handles navigation through the player menus, and stores data based on what the player has selected.
    public class PlayerUIManager : MonoBehaviour
    {
        GameManager gameManager;

        [SerializeField]
        PlayerBattleMenu battleMenu;

        [SerializeField]
        PlayerMagicMenu magicMenu;

        TargetMarker targetMarker;

        Player currentPlayer;

        //Magic
        ArrayList playerMagicInfomation = new ArrayList();
        Magic.MagicStats selectedMagic;
        int maxMagicButtonsDisplayed = 4;
        bool isUsingMagic = false;

        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            if(gameManager == null) { Debug.LogError("Could not find the Game Manager"); }
            if (battleMenu == null) { Debug.LogError("Could not find the Battle Menu"); }
            if (magicMenu == null) { Debug.LogError("Could not find the Magic Menu"); }
            targetMarker = FindObjectOfType<TargetMarker>();
            if (targetMarker == null) { Debug.LogError("Could not find the Target Marker"); }
        }

        void Update()
        {
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
        }

        #region BattleMenu
        private void HandleBattleMenuSelection()
        {
            int highlightedButton = battleMenu.GetHighlightedButton();
            switch (highlightedButton)
            {
                case 0:
                    //Attack
                    SelectTarget();
                    break;
                case 1:
                    //Magic
                    OpenMagicMenu();
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

        private void SelectTarget()
        {
            targetMarker.SetTargets(gameManager.GetEnemyData());
            isUsingMagic = false;
            targetMarker.DisplayMarker();
            HideBattleMenu();
        }

        private void OpenMagicMenu()
        {
            //Can only use if the character knows magic
            if (gameManager.GetCurrentTurnPlayer().CanUseMagic())
            {
                DisplayMagicMenu();
                battleMenu.DisableMenu();
                playerMagicInfomation.Clear();
                GetMagicInfomation();
            }
        }

        private void SelectGuard()
        {
            gameManager.GetCurrentTurnPlayer().UseGuard();
            gameManager.NextTurn(true);
            HideBattleMenu();
        }

        private void SelectFlee()
        {
            gameManager.AttemptEscape();
            HideBattleMenu();
        }
        #endregion

        #region MagicMenu
        //Either use magic if it affects everyone, or store magic data and allow the player to select a target.
        private void HandleMagicMenuSelection()
        {
            int highlightedButton = magicMenu.GetHighlightedButton();
            //The player needs enough MP in order to use the magic
            if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicCost <= currentPlayer.GetCurrentMagic())
            {
                if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsDead && !gameManager.AnyUnconsciousPlayers()) { return; }
                battleMenu.HideMagicMenu();
                if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsAll)
                {
                    //Automatically apply the effect to either the players or enemies
                    FindObjectOfType<BattleMessages>().UpdateMessage(currentPlayer.GetCharacterName() + " casts " + ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicName + "!");
                    battleMenu.HideBattleMenu();
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
                    targetMarker.DisplayMarker();
                    selectedMagic = (Magic.MagicStats)playerMagicInfomation[highlightedButton];
                }
                else
                {
                    //Set up the target marker to target enemies as usual
                    targetMarker.SetTargets(gameManager.GetEnemyData());
                    isUsingMagic = true;
                    targetMarker.DisplayMarker();
                    selectedMagic = (Magic.MagicStats)playerMagicInfomation[highlightedButton];
                }
                battleMenu.HideBattleMenu();
            }
        }

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
            magicMenu.UpdateMagicInfomation(playerMagicInfomation.Count);
        }

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
        //When an attack target has been selected, handle the result based on the action selected.
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

        public void DisplayBattleMenu(Player player)
        {
            battleMenu.gameObject.SetActive(true);
            battleMenu.ResetHighlightedButton();
            battleMenu.SetMagicButtonText(player.CanUseMagic());
            HideMagicMenu();
            currentPlayer = player;
        }

        public void HideBattleMenu()
        {
            battleMenu.gameObject.SetActive(false);
        }

        public void DisplayMagicMenu()
        {
            magicMenu.gameObject.SetActive(true);
        }

        public void HideMagicMenu()
        {
            magicMenu.gameObject.SetActive(false);
        }

        public void HideTargetMarker()
        {
            targetMarker.gameObject.SetActive(false);
        }

        public TargetMarker GetTargetMarker()
        {
            return targetMarker;
        }

        public bool BattleMenuActive()
        {
            return battleMenu.gameObject.activeInHierarchy;
        }

        public bool TargetMarkerActive()
        {
            return targetMarker.gameObject.activeInHierarchy;
        }
    }
}
