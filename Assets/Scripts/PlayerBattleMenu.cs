using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattleMenu : MonoBehaviour
{
    [SerializeField]
    Image[] buttons;

    [SerializeField]
    TargetMarker targetMarker;

    int highlightedButton = 0;
    bool inputPressed = false;

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
        if (!inputPressed)
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
        if(Input.GetAxis("Vertical") == 0)
        {
            inputPressed = false;
        }
    }

    private void SetUI()
    {
        for(int i=0; i<buttons.Length; i++)
        {
            if(i == highlightedButton)
            {
                buttons[i].GetComponent<Image>().color = new Color(0.7f, 0.7f, 1);
            } else
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
                    SelectTarget();
                    break;
                case 1:
                    //Magic
                    break;
                case 2:
                    //Guard
                    break;
                case 3:
                    //Flee
                    break;
            }
        }
    }

    private void SelectTarget()
    {
        targetMarker.DisplayMarker();
        this.gameObject.SetActive(false);
    }

    public void DisplayBattleMenu()
    {
        this.gameObject.SetActive(true);
    }

}
