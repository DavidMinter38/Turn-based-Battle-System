using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    public class PlayerMagicMenu : MonoBehaviour
    {
        [SerializeField]
        Image[] magicButtons;

        [SerializeField]
        Text magicDescryptionText;

        [SerializeField]
        Image upArrow, downArrow;

        [SerializeField]
        TargetMarker targetMarker;

        int highlightedButton = 0;
        bool inputPressed = false;
        bool buttonSelected = false;  //Used so that the UI manager can tell if a button has been pressed.

        int numberOfAvaliableMagic;
        int maxMagicDisplayed = 4;
        string[] magicCosts;
        string[] magicDescryptions;

        int lowestViewableButton = 0;
        int highestViewableButton = 3;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSelection();
            SelectMagicButton();
            Cancel();
        }

        private void UpdateSelection()
        {
            if (!inputPressed)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    inputPressed = true;
                    highlightedButton--;
                    if (highlightedButton < lowestViewableButton && highlightedButton >= 0 && numberOfAvaliableMagic > maxMagicDisplayed)
                    {
                        magicButtons[highestViewableButton].gameObject.SetActive(false);
                        magicButtons[highlightedButton].gameObject.SetActive(true);
                        lowestViewableButton--;
                        highestViewableButton--;
                        downArrow.gameObject.SetActive(true);
                    }
                    if (highlightedButton == 0)
                    {
                        upArrow.gameObject.SetActive(false);
                    }
                    if (highlightedButton < 0)
                    {
                        GoToBottom();
                    }
                }
                if (Input.GetAxis("Vertical") < 0)
                {
                    inputPressed = true;
                    highlightedButton++;
                    if (highlightedButton > highestViewableButton)
                    {
                        magicButtons[lowestViewableButton].gameObject.SetActive(false);
                        magicButtons[highlightedButton].gameObject.SetActive(true);
                        lowestViewableButton++;
                        highestViewableButton++;
                        upArrow.gameObject.SetActive(true);
                        if (highlightedButton == numberOfAvaliableMagic - 1)
                        {
                            downArrow.gameObject.SetActive(false);
                        }
                    }
                    if (highlightedButton > numberOfAvaliableMagic - 1)
                    {
                        GoToTop();
                    }
                }
                SetUI();
            }
            if (Input.GetAxis("Vertical") == 0)
            {
                inputPressed = false;
            }
        }

        public void UpdateMagicInfomation(string[] costs, string[] descryptions)
        {
            magicCosts = costs;
            magicDescryptions = descryptions;
        }

        //Scroll to the top of the magic list
        private void GoToTop()
        {
            highlightedButton = 0;
            lowestViewableButton = 0;
            upArrow.gameObject.SetActive(false);
            if (numberOfAvaliableMagic > maxMagicDisplayed)
            {
                highestViewableButton = 3;
                downArrow.gameObject.SetActive(true);
            }
            for (int i = 0; i < magicButtons.Length; i++)
            {
                if (i < maxMagicDisplayed && i <= numberOfAvaliableMagic - 1)
                {
                    magicButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    magicButtons[i].gameObject.SetActive(false);
                }
            }
        }

        //Scroll to the bottom of the magic list
        private void GoToBottom()
        {
            highlightedButton = numberOfAvaliableMagic - 1;
            downArrow.gameObject.SetActive(false);
            if (numberOfAvaliableMagic > maxMagicDisplayed)
            {
                lowestViewableButton = numberOfAvaliableMagic - maxMagicDisplayed;
                highestViewableButton = numberOfAvaliableMagic - 1;
                upArrow.gameObject.SetActive(true);
            }
            for (int i = 0; i < magicButtons.Length; i++)
            {
                if (i <= numberOfAvaliableMagic - 1 && i >= lowestViewableButton)
                {
                    magicButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    magicButtons[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            GoToTop();
        }

        private void SetUI()
        {
            for (int i = 0; i < magicButtons.Length; i++)
            {
                if (i == highlightedButton)
                {
                    magicButtons[i].GetComponent<Image>().color = new Color(0.7f, 0.7f, 1);
                }
                else
                {
                    magicButtons[i].GetComponent<Image>().color = new Color(1, 1, 1);
                }
            }
            UpdateDescription(magicCosts[highlightedButton], magicDescryptions[highlightedButton]);
        }

        public void UpdateMagicInfomation(int magicSize)
        {
            numberOfAvaliableMagic = magicSize;
        }

        public void UpdateDescription(string cost, string descryption)
        {
            //Updates the text box that explains what the selected magic does
            magicDescryptionText.text = "Cost: " + cost + "MP\n";
            magicDescryptionText.text += descryption;
        }

        private void SelectMagicButton()
        {
            if (Input.GetButtonDown("Submit"))
            {
                buttonSelected = true;
            }
        }

        private void Cancel()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                FindObjectOfType<PlayerBattleMenu>().HideMagicMenu();
            }
        }

        public void ActiveArrows(bool upArrowActive, bool downArrowActive)
        {
            upArrow.gameObject.SetActive(upArrowActive);
            downArrow.gameObject.SetActive(downArrowActive);
        }

        public Image[] GetMagicButtons()
        {
            return magicButtons;
        }

        public int GetHighlightedButton()
        {
            return highlightedButton;
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
