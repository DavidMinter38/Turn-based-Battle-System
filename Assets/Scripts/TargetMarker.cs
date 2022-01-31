using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetMarker : MonoBehaviour
{
    [SerializeField]
    PlayerBattleMenu playerMenu;

    [SerializeField]
    Text targetName;

    Character[] targets;
    int characterToTarget = 0;
    float distanceAboveTarget = 1.5f;
    bool inputPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        targets = FindObjectOfType<GameManager>().GetEnemies();
        if(targets == null) { Debug.LogError("No enemies are active in the scene."); }

        SetTarget();
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        ConfirmTarget();
        Cancel();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void SetTarget()
    {
        if (targets[characterToTarget] == null) {
            FindNewTarget();
        }
        Vector3 targetMarkerPosition = new Vector3(targets[characterToTarget].transform.position.x, targets[characterToTarget].transform.position.y + distanceAboveTarget, 0);
        transform.position = targetMarkerPosition;
        targetName.text = targets[characterToTarget].GetCharacterName();
    }

    private void FindNewTarget()
    {
        for(int i=0; i<targets.Length; i++)
        {
            if(targets[i] != null)
            {
                characterToTarget = i;
            }
        }
    }

    private void UpdateSelection()
    {
        if (!inputPressed)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                inputPressed = true;
                do
                {
                    characterToTarget--;
                    if (characterToTarget < 0)
                    {
                        characterToTarget = targets.Length - 1;
                    }
                } while (targets[characterToTarget] == null);

            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                inputPressed = true;
                do
                {
                    characterToTarget++;
                    if (characterToTarget > targets.Length - 1)
                    {
                        characterToTarget = 0;
                    }
                } while (targets[characterToTarget] == null);
            }
            SetTarget();
        }
        if (Input.GetAxis("Horizontal") == 0)
        {
            inputPressed = false;
        }
    }

    private void ConfirmTarget()
    {
        if (Input.GetButtonDown("Submit"))
        {
            //TODO change to accomodate magic usage
            FindObjectOfType<GameManager>().Attack(targets[characterToTarget].GetID(), false, false, 0);
            HideMarker();
        }
    }

    private void Cancel()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            playerMenu.DisplayBattleMenu();
            FindObjectOfType<GameManager>().SetStatePlayerSelectMove();
            HideMarker();
        }
    }

    public void DisplayMarker()
    {
        this.gameObject.SetActive(true);
        if(targets[characterToTarget] == null)
        {
            //Find a new target
            for(int i=0; i<targets.Length; i++)
            {
                if(targets[i] != null)
                {
                    characterToTarget = i;
                    SetTarget();
                }
            }
        }
    }

    public void HideMarker()
    {
        this.gameObject.SetActive(false);
    }

    public void SetPlayerTargets(Player[] newTargets)
    {
        targets = newTargets;
        characterToTarget = 0;
        SetTarget();
    }

    public void SetEnemyTargets(Enemy[] newTargets)
    {
        //Updates the avalaible targets
        targets = newTargets;
        characterToTarget = 0;
        SetTarget();
    }
}
