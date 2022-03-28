using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    /// <summary>
    /// The battle menu is what the player uses at the start of each of their turns.
    /// </summary>
    /// <remarks>The player uses the menu to select an action, such as 'attack' or 'guard'.</remarks>
    public class PlayerBattleMenu : MonoBehaviour
    {
        [SerializeField]
        Image[] buttons;

        [SerializeField]
        PlayerMagicMenu magicMenu;

        [SerializeField]
        TargetMarker targetMarker;

        int highlightedButton = 0;
        bool inputPressed = false;
        bool menuDisabled = false; //Used to prevent the player from navigating through both the battle menu and the magic menu at the same time
        bool buttonSelected = false;  //Used so that the UI manager can tell if a button has been pressed.

        /// <summary>
        /// When the menu is opened, set up the UI.
        /// </summary>
        void Start()
        {
            SetUI();
        }

        /// <summary>
        /// Update checks for any input from the player.
        /// </summary>
        void Update()
        {
            UpdateSelection();
            SelectButton();
        }

        /// <summary>
        /// If up or down is pressed, change the selected button to the next or previous one respectively.
        /// </summary>
        private void UpdateSelection()
        {
            if (!inputPressed && !menuDisabled)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    inputPressed = true;
                    highlightedButton--;
                    if (highlightedButton < 0)
                    {
                        highlightedButton = buttons.Length - 1;
                    }
                }
                if (Input.GetAxis("Vertical") < 0)
                {
                    inputPressed = true;
                    highlightedButton++;
                    if (highlightedButton > buttons.Length - 1)
                    {
                        highlightedButton = 0;
                    }
                }
                SetUI();
            }
            if (Input.GetAxis("Vertical") == 0)
            {
                inputPressed = false;
            }
        }

        /// <summary>
        /// Indicate which button is highlighted.
        /// </summary>
        private void SetUI()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == highlightedButton)
                {
                    buttons[i].GetComponent<Image>().color = new Color(0.7f, 0.7f, 1);
                }
                else
                {
                    buttons[i].GetComponent<Image>().color = new Color(1, 1, 1);
                }
            }
        }

        /// <summary>
        /// Confirm that a button has been selected, so that the UI Manager can perform the appropriate action.
        /// </summary>
        private void SelectButton()
        {
            if (Input.GetButtonDown("Submit"))
            {
                buttonSelected = true;
            }
        }

        /// <summary>
        /// If the current turn player cannot use magic, change the colour of the magic button to indicate that it cannot be used.
        /// </summary>
        /// <param name="canUseMagic">True if the current turn player can use magic, false if they can not.</param>
        public void SetMagicButtonText(bool canUseMagic)
        {
            if (canUseMagic)
            {
                buttons[1].GetComponentInChildren<Text>().color = new Color(0f, 0f, 0f);
            }
            else
            {
                buttons[1].GetComponentInChildren<Text>().color = new Color(1f, 0.5f, 0.5f);
            }
        }

        /// <summary>
        /// Retrieve the ID of the button that has been hgihlighted.
        /// </summary>
        /// <returns>The highlighted button's ID.</returns>
        public int GetHighlightedButton()
        {
            return highlightedButton;
        }

        /// <summary>
        /// Reset the highlighted button to be the first one in the list.
        /// </summary>
        public void ResetHighlightedButton()
        {
            highlightedButton = 0;
        }

        /// <summary>
        /// Check if a button has been selected by the user.
        /// </summary>
        /// <returns>True if a button has been selected, false if a button has not been selected.</returns>
        public bool IsButtonSelected()
        {
            return buttonSelected;
        }

        /// <summary>
        /// Either enable or disable the battle menu to determine if it can be controlled by the player or not.
        /// </summary>
        /// <remarks>This is used to prevent the player from navigating the battle menu while also navigating the magic menu.</remarks>
        /// <param name="disable">True to disable the battle menu, false to enable it.</param>
        public void DisableMenu(bool disable)
        {
            menuDisabled = disable;
        }

        /// <summary>
        /// Confirms button selection and allows for another button to be selected in the future.
        /// </summary>
        public void ButtonConfirmed()
        {
            buttonSelected = false;
        }

    }
}
