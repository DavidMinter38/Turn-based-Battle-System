using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    /// <summary>
    /// The magic menu is used when the player chooses to use magic from the battle menu.
    /// </summary>
    /// <remarks>The magic menu displays a list of magic that the player is able to use.</remarks>
    public class PlayerMagicMenu : MonoBehaviour
    {
        /// <summary>
        /// A list of buttons used for the magic menu.
        /// </summary>
        [SerializeField]
        Image[] magicButtons;

        /// <summary>
        /// The text field that contains infomation on what the selected magic does.
        /// </summary>
        [SerializeField]
        Text magicDescryptionText;

        [SerializeField]
        Image upArrow, downArrow;

        /// <summary>
        /// A reference to the TargetMarker.
        /// </summary>
        [SerializeField]
        TargetMarker targetMarker;

        /// <summary>
        /// The ID of the button that is currently being highlighted.
        /// </summary>
        int highlightedButton = 0;
        /// <summary>
        /// Indicates if an input has been pressed.
        /// </summary>
        /// <remarks>This is used for menu navigation.</remarks>
        bool inputPressed = false;
        /// <summary>
        /// Used to tell the PlayerUIManager that a button has been selected.
        /// </summary>
        bool buttonSelected = false;
        /// <summary>
        /// Used to tell the PlayerUIManager that the player wishes to exit out of the menu.
        /// </summary>
        bool cancelled = false;

        /// <summary>
        /// The amount of magic that the current turn player has.
        /// </summary>
        int numberOfAvaliableMagic;

        /// <summary>
        /// The maximum number of buttons that are displayed on screen at a time.
        /// </summary>
        readonly int maxMagicDisplayed = 4;

        /// <summary>
        /// The cost of each magic that the current turn player has.
        /// </summary>
        string[] magicCosts;
        /// <summary>
        /// The descryptions of each magic that the current turn player has.
        /// </summary>
        string[] magicDescryptions;

        /// <summary>
        /// The id of the firstmost button in the array that can currently be seen.
        /// </summary>
        int lowestViewableButton = 0;

        /// <summary>
        /// The id of the last button in the array that can currently be seen.
        /// </summary>
        int highestViewableButton = 3;

        /// <summary>
        /// Update checks for any input from the player.
        /// </summary>
        void Update()
        {
            UpdateSelection();
            SelectMagicButton();
            Cancel();
        }

        /// <summary>
        /// If up or down is pressed, change the selected button to the next or previous one respectively.
        /// </summary>
        /// <remarks>Only a certain number of buttons are shown on screen at a time, with arrows being used to indicate if the player can scroll further up or down.</remarks>
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

        /// <summary>
        /// Stores infomation on each avaliable magic that the player has.
        /// </summary>
        /// <remarks>This is used to show the user what each magic does, as well as how much MP is required to use them.</remarks>
        /// <param name="costs">The amount of MP that is needed to use each magic.</param>
        /// <param name="descryptions">The infomation on each magic.</param>
        public void UpdateMagicInfomation(string[] costs, string[] descryptions)
        {
            magicCosts = costs;
            magicDescryptions = descryptions;
        }

        /// <summary>
        /// Go to the topmost button in the list.
        /// </summary>
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

        /// <summary>
        /// Go to the bottommost button in the list.
        /// </summary>
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

        /// <summary>
        /// When the menu is displayed, go to the top of the list.
        /// </summary>
        private void OnEnable()
        {
            GoToTop();
        }

        /// <summary>
        /// Indicate which button is highlighted.
        /// </summary>
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

        /// <summary>
        /// Update how much magic the player has.
        /// </summary>
        /// <param name="magicSize">The amount of magic the player has.</param>
        public void UpdateNumberOfAvaliableMagic(int magicSize)
        {
            numberOfAvaliableMagic = magicSize;
        }

        /// <summary>
        /// Update the text box which shows the user infomation about the magic they have selected.
        /// </summary>
        /// <param name="cost">The amount of MP required to use the selected magic.</param>
        /// <param name="descryption">The infomation about the selected magic.</param>
        public void UpdateDescription(string cost, string descryption)
        {
            //Updates the text box that explains what the selected magic does
            magicDescryptionText.text = "Cost: " + cost + "MP\n";
            magicDescryptionText.text += descryption;
        }

        /// <summary>
        /// Confirm that a button has been selected, so that the UI Manager can perform the appropriate action.
        /// </summary>
        private void SelectMagicButton()
        {
            if (Input.GetButtonDown("Submit"))
            {
                buttonSelected = true;
            }
        }

        /// <summary>
        /// Confirm that the cancellation button has been pressed, so that the UI Manager can exit out of the magic menu.
        /// </summary>
        private void Cancel()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                cancelled = true;
            }
        }

        /// <summary>
        /// Set if the up and down arrows should be active or not.
        /// </summary>
        /// <param name="upArrowActive">True if the up arrow should be active, false if it should not.</param>
        /// <param name="downArrowActive">True if the down arrow should be active, false if it should not.</param>
        public void ActiveArrows(bool upArrowActive, bool downArrowActive)
        {
            upArrow.gameObject.SetActive(upArrowActive);
            downArrow.gameObject.SetActive(downArrowActive);
        }

        /// <summary>
        /// Retrieve the array of buttons used in the magic menu.
        /// </summary>
        /// <returns>An array of buttons.</returns>
        public Image[] GetMagicButtons()
        {
            return magicButtons;
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
        /// Check if a button has been selected by the user.
        /// </summary>
        /// <returns>True if a button has been selected, false if a button has not been selected.</returns>
        public bool IsButtonSelected()
        {
            return buttonSelected;
        }

        /// <summary>
        /// Checks if the user has chosen to cancel out of the magic menu.
        /// </summary>
        /// <returns>True if the cancel button had been pressed, false if it had not.</returns>
        public bool HasCancelled()
        {
            return cancelled;
        }

        /// <summary>
        /// Confirms button selection and allows for another button to be selected in the future.
        /// </summary>
        public void ButtonConfirmed()
        {
            buttonSelected = false;
        }

        /// <summary>
        /// Confirms cancellation and allows the menu to be cancelled out of in the future.
        /// </summary>
        public void CancelConfirmed()
        {
            cancelled = false;
        }
    }
}
