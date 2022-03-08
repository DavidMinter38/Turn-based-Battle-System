using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleSystem.Gameplay;

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
        bool menuDisabled = false;

        // Start is called before the first frame update
        void Start()
        {
            SetUI();
        }

        // Update is called once per frame
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
        }

        private void SelectTarget()
        {
            targetMarker.SetEnemyTargets(FindObjectOfType<GameManager>().GetEnemies());
            targetMarker.DisplayMarker(false);
            HideBattleMenu();
        }

        private void OpenMagicMenu()
        {
            //Can only use if the character knows magic
            if (FindObjectOfType<GameManager>().GetCurrentTurnPlayer().CanUseMagic())
            {
                magicMenu.gameObject.SetActive(true);
                menuDisabled = true;
            }
        }

        public void HideMagicMenu()
        {
            magicMenu.gameObject.SetActive(false);
            menuDisabled = false;
        }

        private void SelectGuard()
        {
            FindObjectOfType<GameManager>().GetCurrentTurnPlayer().UseGuard();
            FindObjectOfType<GameManager>().NextTurn(true);
            HideBattleMenu();
        }

        private void SelectFlee()
        {
            FindObjectOfType<GameManager>().AttemptEscape();
            HideBattleMenu();
        }

        public void DisplayBattleMenu()
        {
            this.gameObject.SetActive(true);
            magicMenu.gameObject.SetActive(false);
            highlightedButton = 0;
        }

        public void HideBattleMenu()
        {
            this.gameObject.SetActive(false);
        }

    }
}
