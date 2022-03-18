using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
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

        void Start()
        {
            SetUI();
        }

        void Update()
        {
            UpdateSelection();
            SelectButton();
        }

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

        private void SetUI()
        {
            //Indicate which button is highlighted
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

        private void SelectButton()
        {
            if (Input.GetButtonDown("Submit"))
            {
                buttonSelected = true;
            }
        }

        public void HideMagicMenu()
        {
            magicMenu.gameObject.SetActive(false);
            menuDisabled = false;
        }

        public void DisplayBattleMenu()
        {
            this.gameObject.SetActive(true);
            magicMenu.gameObject.SetActive(false);
            highlightedButton = 0;
            menuDisabled = false;
        }

        public void HideBattleMenu()
        {
            this.gameObject.SetActive(false);
        }

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

        public int GetHighlightedButton()
        {
            return highlightedButton;
        }

        public void ResetHighlightedButton()
        {
            highlightedButton = 0;
        }

        public void DisableMenu()
        {
            menuDisabled = true;
        }

        public bool IsButtonSelected()
        {
            return buttonSelected;
        }

        public void ButtonConfirmed()
        {
            buttonSelected = false;
        }

    }
}
