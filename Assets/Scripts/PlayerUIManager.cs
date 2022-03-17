using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Interface;
using BattleSystem.Characters;

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
            targetMarker.DisplayMarker(false);
            HideBattleMenu();
        }

        private void OpenMagicMenu()
        {
            //Can only use if the character knows magic
            if (gameManager.GetCurrentTurnPlayer().CanUseMagic())
            {
                DisplayMagicMenu();
                battleMenu.DisableMenu();
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

        }
        #endregion

        #region TargetMarker
        //When an attack target has been selected, handle the result based on the action selected.
        private void ConfirmTarget()
        {

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

        public bool MagicMenuActive()
        {
            return magicMenu.gameObject.activeInHierarchy;
        }

        public bool TargetMarkerActive()
        {
            return targetMarker.gameObject.activeInHierarchy;
        }
    }
}
