using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Data;
using BattleSystem.Interface;

namespace BattleSystem.Characters
{
    /// <summary>
    /// The Player class inherits from the Character class, and contains additonal values used exclusively by players.
    /// </summary>
    /// <remarks>This class stored values related to the player's magic, and also handles player death and revival.</remarks>
    public class Player : Character
    {
        /// <summary>
        /// A unique ID used to differentiate each player character.
        /// </summary>
        [SerializeField]
        int playerID;

        protected int currentMP, maxMP;

        /// <summary>
        /// If set to false, the player cannot use any magic.
        /// </summary>
        bool knowsMagic;
        /// <summary>
        /// A list of magic that the player can use.
        /// </summary>
        /// <remarks>Each boolean value can be compared with the magic list to see if the player can use that magic.
        /// For example, if the first boolean in the array is true, the first magic in the stored list can be used by the player.</remarks>
        bool[] avaliableMagic;

        /// <summary>
        /// Used to see if the player can be used.
        /// </summary>
        /// <remarks>If set to false, the player will not be instantiated into the scene.</remarks>
        bool isAvaliable = false;
        /// <summary>
        /// Indicates if the player is currently conscious.
        /// </summary>
        /// <remarks>This will be set to false if the player's health drops to 0.</remarks>
        bool isConscious = true;
        /// <summary>
        /// Indicates if the player is currently guarding.
        /// </summary>
        /// <remarks>This will be set to true if the player chooses the guard action.</remarks>
        bool isGuarding = false;

        /// <summary>
        /// Used to see if the player is able to take an action on their turn.
        /// </summary>
        /// <remarks>This is used to prevent revived players from taking action on the same turn that they are revived</remarks>
        bool inCombat = true;


        //UI elements

        /// <summary>
        /// A reference to the player's PlayerHealthUI.
        /// </summary>
        PlayerHealthUI playerUI;
        /// <summary>
        /// The rate that the player's health and magic meters will increase or decrease.
        /// </summary>
        /// <remarks>This is used so that the player's health and magic meters in the the UI can be seen gradually increasing or decreasing over a period of time.</remarks>
        readonly float rateOfUIChange = 0.75f;

        /// <summary>
        /// A sprite that is displayed to indicate that the player is currently guarding.
        /// </summary>
        [SerializeField]
        SpriteRenderer guardIcon;

        /// <summary>
        /// On startup, the Awake function finds the infomation in GameData for the player that matches the player's ID, and then assigns the data to the respective values.
        /// </summary>
        void Awake()
        {
            isPlayer = true;

            GameData.PlayerStats playerStats = FindObjectOfType<GameData>().GetPlayerStats(playerID);
            characterName = playerStats.playerName;
            currentHP = playerStats.currentHP;
            maxHP = playerStats.maxHP;
            currentMP = playerStats.currentMP;
            maxMP = playerStats.maxMP;
            attack = playerStats.attack;
            defence = playerStats.defence;
            magicAttack = playerStats.magicAttack;
            magicDefence = playerStats.magicDefence;
            speed = playerStats.speed;
            knowsMagic = playerStats.knowsMagic;
            avaliableMagic = playerStats.avaliableMagic;
            isAvaliable = playerStats.isAvaliable;
            isConscious = playerStats.isConscious;
            characterSprite = playerStats.playerSprite;
            playerUI = playerStats.playerHealthUI;

            if (!playerStats.isAvaliable)
            {
                this.gameObject.SetActive(false);
            }

            this.GetComponent<SpriteRenderer>().sprite = characterSprite;
            playerUI.LoadAttributes(currentHP, maxHP, currentMP, maxMP, knowsMagic, characterName, characterSprite);
        }

        /// <summary>
        /// Restores the player's health by a certain amount.  
        /// </summary>
        /// <remarks>The player's health cannot go higher than the player's max HP.
        /// This version of the function updates the player's health in the UI gradually over time.</remarks>
        /// <param name="healthToRecover">The amount of health to be recovered.</param>
        /// <returns>A delay that represents the rate that the player UI updates, which varies depending on how much health has been gained.</returns>
        protected override IEnumerator GainHealth(int healthToRecover)
        {
            int displayHP = currentHP;
            float timeDelay = 0;
            if (healthToRecover != 0) { timeDelay = rateOfUIChange / healthToRecover; }
            currentHP += healthToRecover;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }

            while (healthToRecover != 0 && displayHP < maxHP)
            {
                displayHP++;
                healthToRecover--;
                playerUI.UpdateHealth(displayHP);
                yield return new WaitForSeconds(timeDelay);
            }
            playerUI.UpdateHealth(currentHP);
        }

        /// <summary>
        /// Reduces the player's health by a certain amount.
        /// </summary>
        /// <remarks>If the player's health reaches 0, then the player falls unconscious.
        /// This version of the function updates the player's health in the UI gradually over time.</remarks>
        /// <param name="damage">The amount of health to be lost.</param>
        /// <returns>A delay that represents the rate that the player UI updates, which varies depending on how much health has been lost.</returns>
        protected override IEnumerator TakeDamage(int damage)
        {
            int displayHP = currentHP;
            float timeDelay = 0;
            if (damage != 0) { timeDelay = rateOfUIChange / damage; }
            currentHP -= damage;
            if (currentHP <= 0) { currentHP = 0; }

            if (currentHP <= 0)
            {
                //Character dies
                KillCharacter();
            }

            while (damage != 0 && displayHP > 0)
            {
                displayHP--;
                damage--;
                playerUI.UpdateHealth(displayHP);
                yield return new WaitForSeconds(timeDelay);
            }
            playerUI.UpdateHealth(currentHP);
        }

        /// <summary>
        /// Restores the player's magic by a certain amount.  
        /// </summary>
        /// <remarks>The player's magic cannot go higher than the player's max MP.</remarks>
        /// <param name="magicGained">The amount of magic to be recovered.</param>
        /// <returns>A delay that represents the rate that the player UI updates, which varies depending on how much magic has been gained.</returns>
        public IEnumerator GainMagic(int magicGained)
        {
            int displayMagic = currentMP;
            float timeDelay = 0;
            if (magicGained != 0) { timeDelay = rateOfUIChange / magicGained; }
            currentMP += magicGained;
            if (currentMP > maxMP)
            {
                currentMP = maxMP;
            }

            while (magicGained != 0 && displayMagic < maxMP)
            {
                displayMagic++;
                magicGained--;
                playerUI.UpdateMagic(displayMagic);
                yield return new WaitForSeconds(timeDelay);
            }
            playerUI.UpdateMagic(currentMP);
        }

        /// <summary>
        /// Reduces the player's magic by a certain amount.
        /// </summary>
        /// <param name="magicLost">The amount of health to be lost.</param>
        /// <returns>A delay that represents the rate that the player UI updates, which varies depending on how much magic has been lost.</returns>
        public IEnumerator LoseMagic(int magicLost)
        {
            int displayMagic = currentMP;
            float timeDelay = 0;
            if (magicLost != 0) { timeDelay = rateOfUIChange / magicLost; }
            currentMP -= magicLost;
            if (currentMP <= 0) { currentMP = 0; }

            while (magicLost != 0 && displayMagic > 0)
            {
                displayMagic--;
                magicLost--;
                playerUI.UpdateMagic(displayMagic);
                yield return new WaitForSeconds(timeDelay);
            }
            playerUI.UpdateMagic(currentMP);
        }

        /// <summary>
        /// Sets up the player's guard.
        /// </summary>
        public void UseGuard()
        {
            isGuarding = true;
            guardIcon.gameObject.SetActive(true);
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has got their guard up!");
        }

        /// <summary>
        /// Resolves the player's guard.
        /// </summary>
        /// <remarks>The player's guard ends at the start of their turn.</remarks>
        public void FinishGuard()
        {
            isGuarding = false;
            guardIcon.gameObject.SetActive(false);
        }

        /// <summary>
        /// Makes the player character unconscious.
        /// </summary>
        /// <remarks>An unconsious character cannot be atttacked or healed.  Their turn is also skipped.</remarks>
        protected override void KillCharacter()
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has fallen!");
            isGuarding = false;
            guardIcon.gameObject.SetActive(false);
            isConscious = false;
            inCombat = false;
            this.gameObject.transform.Rotate(new Vector3(0, 0, 90));
        }

        /// <summary>
        /// Makes the player conscious.
        /// </summary>
        /// <remarks>This is called when magic is used on the player that revives them.</remarks>
        public void Revive()
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has been revived!");
            isConscious = true;
            this.gameObject.transform.Rotate(new Vector3(0, 0, -90));
        }

        /// <summary>
        /// Allows the player to take actions in combat.
        /// </summary>
        /// <remarks>This function is used to prevent a revived character from acting until the start of a new round.</remarks>
        public void ReturnToCombat()
        {
            inCombat = true;
        }

        /// <summary>
        /// Retrieves the player ID.
        /// </summary>
        /// <remarks>This is different from the ID used in the Character class, as this ID is used to differentiate each player.</remarks>
        /// <returns>The player ID</returns>
        public int GetPlayerID()
        {
            return playerID;
        }

        /// <summary>
        /// Retrieves the character's current magic value.
        /// </summary>
        /// <returns>The character's current magic.</returns>
        public int GetCurrentMagic()
        {
            return currentMP;
        }

        /// <summary>
        /// Retrieves the character's maximum magic value.
        /// </summary>
        /// <returns>The character's maximum magic.</returns>
        public int GetMaxMagic()
        {
            return maxMP;
        }

        /// <summary>
        /// Checks if the player is able to use magic.
        /// </summary>
        /// <returns>A boolean used to indicate if the player can use magic.</returns>
        public bool CanUseMagic()
        {
            return knowsMagic;
        }

        /// <summary>
        /// Retrieves data on what magic the player character can use.
        /// </summary>
        /// <returns>An array of boolean values that correlates to the stored magic list.</returns>
        public bool[] GetKnownMagic()
        {
            return avaliableMagic;
        }

        /// <summary>
        /// Checks if the player is avaliable to be used in battle.
        /// </summary>
        /// <returns>A boolean value used to indicate if the player is avaliable.</returns>
        public bool IsAvaliable()
        {
            return isAvaliable;
        }

        /// <summary>
        /// Checks if the player is conscious.
        /// </summary>
        /// <returns>A boolean value used to indicate if the player is conscious.</returns>
        public bool IsConscious()
        {
            return isConscious;
        }

        /// <summary>
        /// Checks if the player is guarding.
        /// </summary>
        /// <returns>A boolean value used to indicate if the player is guarding.</returns>
        public bool IsGuarding()
        {
            return isGuarding;
        }

        /// <summary>
        /// Checks if the player is in combat.
        /// </summary>
        /// <returns>A boolean value used to indicate if the player is in combat.</returns>
        public bool IsInCombat()
        {
            return inCombat;
        }
    }
}
