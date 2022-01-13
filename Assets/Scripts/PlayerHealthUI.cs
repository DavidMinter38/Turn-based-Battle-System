using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField]
    Slider playerHealthSlider, playerMagicSlider;

    [SerializeField]
    Text playerHealthText, playerMagicText, playerNameText;

    [SerializeField]
    Image playerImage;

    [SerializeField]
    int playerHealthID;

    [SerializeField]
    Player thePlayer;

    int playerCurrentHealth, playerMaxHealth, playerCurrentMagic, playerMaxMagic;
    string playerName;

    // Start is called before the first frame update
    void Start()
    {
        playerMagicSlider.gameObject.SetActive(thePlayer.CanUseMagic());

        //Get values from player
        playerCurrentHealth = thePlayer.GetCurrentHealth();
        playerMaxHealth = thePlayer.GetMaxHealth();
        playerCurrentMagic = thePlayer.GetCurrentMagic();
        playerMaxMagic = thePlayer.GetMaxMagic();
        playerName = thePlayer.GetCharacterName();

        //Set up health slider
        playerHealthSlider.maxValue = playerMaxHealth;
        playerHealthSlider.value = playerCurrentHealth;
        playerHealthText.text = playerCurrentHealth.ToString() + "/" + playerMaxHealth.ToString();

        //Set up magic slider
        playerMagicSlider.maxValue = playerMaxMagic;
        playerMagicSlider.value = playerCurrentMagic;
        playerMagicText.text = playerCurrentMagic.ToString() + "/" + playerMaxMagic.ToString();


        playerNameText.text = playerName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth()
    {
        playerCurrentHealth = thePlayer.GetCurrentHealth();
        playerHealthSlider.value = playerCurrentHealth;
        playerHealthText.text = playerCurrentHealth.ToString() + "/" + playerMaxHealth.ToString();
    }

    public void UpdateMagic()
    {
        playerCurrentMagic = thePlayer.GetCurrentMagic();
        playerMagicSlider.value = playerCurrentMagic;
        playerMagicText.text = playerCurrentMagic.ToString() + "/" + playerMaxMagic.ToString();
    }
}
