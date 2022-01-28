using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagicMenu : MonoBehaviour
{
    [SerializeField]
    Image[] magicButtons;

    [SerializeField]
    Text magicDescryptionText;

    [SerializeField]
    Image upArrow, downArrow;

    int highlightedButton = 0;
    bool inputPressed = false;

    bool[] knownMagic;
    ArrayList playerMagicInfomation = new ArrayList();
    int numberOfAvaliableMagic;
    int maxMagicDisplayed = 4;

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
                do
                {
                    highlightedButton--;
                    if (highlightedButton < 0)
                    {
                        highlightedButton = magicButtons.Length - 1;
                    }
                } while (!magicButtons[highlightedButton].IsActive());
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                inputPressed = true;
                do
                {
                    highlightedButton++;
                    if (highlightedButton > magicButtons.Length - 1)
                    {
                        highlightedButton = 0;
                    }
                } while (!magicButtons[highlightedButton].IsActive());
            }
            SetUI();
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            inputPressed = false;
        }
    }

    private void OnEnable()
    {
        GetMagicInfomation();
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
        //TODO change text on buttons when arrows are used
    }

    private void GetMagicInfomation()
    {
        Player currentPlayer = FindObjectOfType<GameManager>().GetCurrentTurnPlayer();
        knownMagic = currentPlayer.GetKnownMagic();
        numberOfAvaliableMagic = 0;
        for(int i=0; i<knownMagic.Length; i++)
        {
            if (knownMagic[i])
            {
                Magic.MagicStats magic = FindObjectOfType<Magic>().GetMagicInfomation(i);
                playerMagicInfomation.Add(magic);
                magicButtons[numberOfAvaliableMagic].GetComponentInChildren<Text>().text = ((Magic.MagicStats)playerMagicInfomation[numberOfAvaliableMagic]).magicName;
                
                numberOfAvaliableMagic++;
            }
        }
        if(numberOfAvaliableMagic <= maxMagicDisplayed)
        {
            upArrow.gameObject.SetActive(false);
            downArrow.gameObject.SetActive(false);
            for(int i=0; i < maxMagicDisplayed; i++)
            {
                if(i >= numberOfAvaliableMagic)
                {
                    magicButtons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            //TODO handle code for when the arrows are needed.
        }

        UpdateDescription();
    }

    private void UpdateDescription()
    {
        magicDescryptionText.text = ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicDescription;
    }

    private void SelectMagicButton()
    {
        if (Input.GetButtonDown("Submit"))
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(FindObjectOfType<GameManager>().GetCurrentTurnPlayer().GetCharacterName() + " casts " + ((Magic.MagicStats)playerMagicInfomation[highlightedButton]).magicName + "!");
            if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsAll)
            {
                //Automatically apply the effect to either the players or enemies
                return;
            }

            if (((Magic.MagicStats)playerMagicInfomation[highlightedButton]).affectsPlayers)
            {
                //Set up the target marker so that only players can be selected
            } else
            {
                //Set up the target marker to target enemies as usual
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
}
