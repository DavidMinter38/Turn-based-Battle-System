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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputPressed)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                inputPressed = true;
                highlightedButton--;
                if (highlightedButton < 0)
                {
                    highlightedButton = magicButtons.Length - 1;
                }
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                inputPressed = true;
                highlightedButton++;
                if (highlightedButton > magicButtons.Length - 1)
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

        Cancel();
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
        //TODO change text on buttons
    }

    private void Cancel()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            FindObjectOfType<PlayerBattleMenu>().HideMagicMenu();
        }
    }
}
