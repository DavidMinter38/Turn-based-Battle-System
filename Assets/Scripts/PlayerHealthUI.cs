using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    /// <summary>
    /// The PlayerHealthUI script handles the UI that displays player health and magic values.
    /// </summary>
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField]
        Slider playerHealthSlider, playerMagicSlider;

        [SerializeField]
        Text playerHealthText, playerMagicText, playerNameText;

        /// <summary>
        /// Used to indicate which player the health UI belongs to.
        /// </summary>
        [SerializeField]
        Image playerImage;

        int playerCurrentHealth, playerMaxHealth, playerCurrentMagic, playerMaxMagic;
        string playerName;

        /// <summary>
        /// Sets up the player infomation at the start of the battle.
        /// </summary>
        /// <param name="iCurrentHP">The player's current HP value.</param>
        /// <param name="iMaxHP">The player's maximum HP value.</param>
        /// <param name="iCurrentMP">The player's current MP value.</param>
        /// <param name="iMaxMP">The player's max MP value.</param>
        /// <param name="iUseMagic">True if the player can use magic, false if they can not.</param>
        /// <param name="iName">The player's name.</param>
        /// <param name="iSprite">The player's sprite.</param>
        public void LoadAttributes(int iCurrentHP, int iMaxHP, int iCurrentMP, int iMaxMP, bool iUseMagic, string iName, Sprite iSprite)
        {
            playerMagicSlider.gameObject.SetActive(iUseMagic);

            //Get values from player
            playerCurrentHealth = iCurrentHP;
            playerMaxHealth = iMaxHP;
            playerCurrentMagic = iCurrentMP;
            playerMaxMagic = iMaxMP;
            playerName = iName;
            playerImage.sprite = iSprite;

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

        /// <summary>
        /// Update's the player's health UI.
        /// </summary>
        /// <param name="newHealth">The new value of the player's current health.</param>
        public void UpdateHealth(int newHealth)
        {
            playerHealthSlider.value = newHealth;
            playerHealthText.text = newHealth + "/" + playerMaxHealth.ToString();
        }

        /// <summary>
        /// Updates the player's magic UI.
        /// </summary>
        /// <param name="newMagic">The new value of the player's current magic.</param>
        public void UpdateMagic(int newMagic)
        {
            playerMagicSlider.value = newMagic;
            playerMagicText.text = newMagic.ToString() + "/" + playerMaxMagic.ToString();
        }
    }
}
