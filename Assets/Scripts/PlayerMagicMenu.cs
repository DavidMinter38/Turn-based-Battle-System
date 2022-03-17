using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleSystem.Gameplay;
using BattleSystem.Data;

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

        bool[] knownMagic;
        ArrayList playerMagicInfomation = new ArrayList();
        int numberOfAvaliableMagic;
        int maxMagicDisplayed = 4;

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
                    if (highlightedButton < lowestViewableButton && highlightedButton >= 0 && playerMagicInfomation.Count > maxMagicDisplayed)
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
                        if (highlightedButton == playerMagicInfomation.Count - 1)
                        {
                            downArrow.gameObject.SetActive(false);
                        }
                    }
                    if (highlightedButton > playerMagicInfomation.Count - 1)
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

        //Scroll to the top of the magic list
        private void GoToTop()
        {
            highlightedButton = 0;
            lowestViewableButton = 0;
            upArrow.gameObject.SetActive(false);
            if (playerMagicInfomation.Count > maxMagicDisplayed)
            {
                highestViewableButton = 3;
                downArrow.gameObject.SetActive(true);
            }
            for (int i = 0; i < magicButtons.Length; i++)
            {
                if (i < maxMagicDisplayed && i <= playerMagicInfomation.Count - 1)
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
            highlightedButton = playerMagicInfomation.Count - 1;
            downArrow.gameObject.SetActive(false);
            if (playerMagicInfomation.Count > maxMagicDisplayed)
            {
                lowestViewableButton = playerMagicInfomation.Count - maxMagicDisplayed;
                highestViewableButton = playerMagicInfomation.Count - 1;
                upArrow.gameObject.SetActive(true);
            }
            for (int i = 0; i < magicButtons.Length; i++)
            {
                if (i <= playerMagicInfomation.Count - 1 && i >= lowestViewableButton)
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
            playerMagicInfomation.Clear();
            GetMagicInfomation();
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
            UpdateDescription();
        }

        private void GetMagicInfomation()
        {
            knownMagic = FindObjectOfType<GameManager>().GetCurrentTurnPlayer().GetKnownMagic();
            int playerMagicAmount = FindObjectOfType<GameManager>().GetCurrentTurnPlayer().GetCurrentMagic();
            numberOfAvaliableMagic = 0;
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
                    } else
                    {
                        magicButtons[numberOfAvaliableMagic].GetComponentInChildren<Text>().color = new Color(0f, 0f, 0f);
                    }
                    numberOfAvaliableMagic++;
                }
            }
            if (numberOfAvaliableMagic <= maxMagicDisplayed)
            {
                upArrow.gameObject.SetActive(false);
                downArrow.gameObject.SetActive(false);
                for (int i = 0; i < maxMagicDisplayed; i++)
                {
                    if (i >= numberOfAvaliableMagic)
                    {
                        magicButtons[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                upArrow.gameObject.SetActive(false);
                downArrow.gameObject.SetActive(true);
                for (int i = 0; i < maxMagicDisplayed; i++)
                {
                    magicButtons[i].gameObject.SetActive(true);
                }
            }

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            //Updates the text box that explains what the selected magic does
            magicDescryptionText.text = "Cost: " + ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicCost.ToString() + "MP\n";
            magicDescryptionText.text += ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicDescription;
        }

        private void SelectMagicButton()
        {
            if (Input.GetButtonDown("Submit"))
            {
                buttonSelected = true;
                //The player needs enough MP in order to use the magic
                if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicCost <= FindObjectOfType<GameManager>().GetCurrentTurnPlayer().GetCurrentMagic())
                {
                    if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsDead && !FindObjectOfType<GameManager>().AnyUnconsciousPlayers()) { return; }
                    FindObjectOfType<PlayerBattleMenu>().HideMagicMenu();
                    if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsAll)
                    {
                        //Automatically apply the effect to either the players or enemies
                        FindObjectOfType<BattleMessages>().UpdateMessage(FindObjectOfType<GameManager>().GetCurrentTurnPlayer().GetCharacterName() + " casts " + ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicName + "!");
                        FindObjectOfType<PlayerBattleMenu>().HideBattleMenu();
                        FindObjectOfType<GameManager>().GetCurrentTurnPlayer().StartCoroutine("LoseMagic", ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicCost);
                        if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsPlayers)
                        {
                            FindObjectOfType<GameManager>().HealAll(((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicStrength);
                        }
                        else
                        {
                            FindObjectOfType<GameManager>().AttackAll(((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicStrength);
                        }
                        return;
                    }

                    if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsPlayers)
                    {
                        //Set up the target marker so that only players can be selected
                        if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsDead)
                        {
                            targetMarker.SetTargets(FindObjectOfType<GameManager>().GetUnconsciousPlayers());
                        }
                        else
                        {
                            targetMarker.SetTargets(FindObjectOfType<GameManager>().GetAlivePlayers());
                        }
                        targetMarker.DisplayMarker(true);
                        targetMarker.SetMagicInfomation((Magic.MagicStats)playerMagicInfomation[highlightedButton]);
                    }
                    else
                    {
                        //Set up the target marker to target enemies as usual
                        targetMarker.SetTargets(FindObjectOfType<GameManager>().GetEnemyData());
                        targetMarker.DisplayMarker(true);
                        targetMarker.SetMagicInfomation((Magic.MagicStats)playerMagicInfomation[highlightedButton]);
                    }
                    FindObjectOfType<PlayerBattleMenu>().HideBattleMenu();
                }
            }
        }

        private void Cancel()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                FindObjectOfType<PlayerBattleMenu>().HideMagicMenu();
            }
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
