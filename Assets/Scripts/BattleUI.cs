using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    [SerializeField]
    PlayerHealthUI player1HealthUI, player2HealthUI, player3HealthUI, player4HealthUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        //TODO perhaps remove this, and only update the health value that needs to be updated
        if (player1HealthUI.gameObject.activeInHierarchy)
        {
            player1HealthUI.UpdateHealth();
        }

        if (player2HealthUI.gameObject.activeInHierarchy)
        {
            player2HealthUI.UpdateHealth();
        }

        if (player3HealthUI.gameObject.activeInHierarchy)
        {
            player3HealthUI.UpdateHealth();
        }

        if (player4HealthUI.gameObject.activeInHierarchy)
        {
            player4HealthUI.UpdateHealth();
        }
    }
}
