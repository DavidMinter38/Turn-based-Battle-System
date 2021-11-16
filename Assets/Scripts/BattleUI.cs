using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField]
    Slider player1HealthSlider, player1MagicSlider;

    [SerializeField]
    Text player1HealthText, player1MagicText, player1NameText;

    [SerializeField]
    Image player1Image;

    int player1CurrentHealth;
    int player1MaxHealth;
    string player1Name;

    // Start is called before the first frame update
    void Start()
    {
        Player player1 = FindObjectOfType<Player>();

        player1MagicSlider.gameObject.SetActive(player1.CanUseMagic());

        //Get values from player
        player1CurrentHealth = player1.GetCurrentHealth();
        player1MaxHealth = player1.GetMaxHealth();
        player1Name = player1.GetCharacterName();

        //Set up health slider
        player1HealthSlider.maxValue = player1MaxHealth;
        player1HealthSlider.value = player1CurrentHealth;
        player1HealthText.text = player1CurrentHealth.ToString() + "/" + player1MaxHealth.ToString();


        player1NameText.text = player1Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(int newHealth)
    {
        player1CurrentHealth = newHealth;
        player1HealthSlider.value = player1CurrentHealth;
        player1HealthText.text = player1CurrentHealth.ToString() + "/" + player1MaxHealth.ToString();
    }
}
