using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleSystem.Characters;

namespace BattleSystem.Interface
{
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

        int playerCurrentHealth, playerMaxHealth, playerCurrentMagic, playerMaxMagic;
        string playerName;


        public void LoadAttributes(Player thePlayer)
        {
            playerMagicSlider.gameObject.SetActive(thePlayer.CanUseMagic());

            //Get values from player
            playerCurrentHealth = thePlayer.GetCurrentHealth();
            playerMaxHealth = thePlayer.GetMaxHealth();
            playerCurrentMagic = thePlayer.GetCurrentMagic();
            playerMaxMagic = thePlayer.GetMaxMagic();
            playerName = thePlayer.GetCharacterName();
            playerImage.sprite = thePlayer.GetSprite();

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

        public void UpdateHealth(int newHealth)
        {
            playerHealthSlider.value = newHealth;
            playerHealthText.text = newHealth + "/" + playerMaxHealth.ToString();
        }

        public void UpdateMagic(int newMagic)
        {
            playerMagicSlider.value = newMagic;
            playerMagicText.text = newMagic.ToString() + "/" + playerMaxMagic.ToString();
        }
    }
}
