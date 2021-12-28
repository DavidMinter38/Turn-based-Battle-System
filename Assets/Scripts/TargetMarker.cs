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

    Enemy[] targets;
    int enemyToTarget = 0;
    float distanceAboveTarget = 1.5f;
    bool inputPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        targets = FindObjectOfType<GameManager>().GetEnemies();
        if(targets == null) { Debug.LogError("No enemies are active in the scene."); }

        SetTarget(enemyToTarget);
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        ConfirmTarget();
        Cancel();
    }

    private void SetTarget(int target)
    {
        Vector3 targetMarkerPosition = new Vector3(targets[target].transform.position.x, targets[target].transform.position.y + distanceAboveTarget, 0);
        transform.position = targetMarkerPosition;
        targetName.text = targets[enemyToTarget].GetCharacterName();
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
                    enemyToTarget--;
                    if (enemyToTarget < 0)
                    {
                        enemyToTarget = targets.Length - 1;
                    }
                } while (targets[enemyToTarget] == null);

            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                inputPressed = true;
                do
                {
                    enemyToTarget++;
                    if (enemyToTarget > targets.Length - 1)
                    {
                        enemyToTarget = 0;
                    }
                } while (targets[enemyToTarget] == null);
            }
            SetTarget(enemyToTarget);
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
            FindObjectOfType<GameManager>().StartCoroutine("Attack", targets[enemyToTarget].GetID());
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
        if(targets[enemyToTarget] == null)
        {
            //Find a new target
            for(int i=0; i<targets.Length; i++)
            {
                if(targets[i] != null)
                {
                    enemyToTarget = i;
                    SetTarget(enemyToTarget);
                }
            }
        }
    }

    public void HideMarker()
    {
        this.gameObject.SetActive(false);
    }

    public void SetEnemyTargets(Enemy[] newTargets)
    {
        //Updates the avalaible targets
        targets = newTargets;
    }
}
