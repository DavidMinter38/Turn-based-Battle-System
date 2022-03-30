using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Interface;

namespace BattleSystem.Characters
{
    /// <summary>
    /// The Character class contains functions that are shared between the player characters and the enemies.
    /// </summary>
    /// <remarks>The main features stored in this class are related to basic attributes such as HP, attack, and defence.</remarks>
    public class Character : MonoBehaviour
    {
        /// <summary>
        /// Used to uniquely identify each character in a battle.
        /// </summary>
        [SerializeField]
        int ID;

        [SerializeField]
        protected string characterName;

        protected int currentHP, maxHP, attack, defence, magicAttack, magicDefence, speed;

        /// <summary>
        /// The sprite that will be used to identify the character in the scene.
        /// </summary>
        protected Sprite characterSprite;

        /// <summary>
        /// The player class will set this to true, wheras the enemy class will set this to false.
        /// </summary>
        /// <remarks>This variable is used in order for the GameManager to tell if a character is a player or an enemy.</remarks>
        protected bool isPlayer;

        /// <summary>
        /// Restores the character's health by a certain amount.  
        /// </summary>
        /// <remarks>The character's health cannot go higher than the character's max HP.</remarks>
        /// <param name="healthToRecover">The amount of health to be recovered.</param>
        /// <returns>A delay of 0.5 seconds.</returns>
        protected virtual IEnumerator GainHealth(int healthToRecover)
        {
            currentHP += healthToRecover;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
            yield return new WaitForSeconds(0.5f);
        }

        /// <summary>
        /// Reduces the character's health by a certain amount.
        /// </summary>
        /// <remarks>If the character's health reaches 0, then the character is killed, with the method varying depending on if it's a player or an enemy.</remarks>
        /// <param name="damage">The amount of health to be lost.</param>
        /// <returns>A delay of 0.5 seconds.</returns>
        protected virtual IEnumerator TakeDamage(int damage)
        {
            currentHP -= damage;
            if (currentHP <= 0) { currentHP = 0; }

            if (currentHP <= 0)
            {
                //Character dies
                KillCharacter();
            }

            yield return new WaitForSeconds(0.5f);
        }

        /// <summary>
        /// Resolve attacking the targetted character.
        /// </summary>
        /// <param name="target">The character that is being attacked.</param>
        /// <param name="attackDamage">The amount of damage that will be dealt.</param>
        public void Attack(Character target, int attackDamage)
        {
            FindObjectOfType<BattleMessages>().UpdateMessage("Dealt " + attackDamage + " damage to " + target.GetCharacterName() + "!");
            target.StartCoroutine("TakeDamage", attackDamage);
        }

        /// <summary>
        /// Resolve healing the targetted character.
        /// </summary>
        /// <param name="target">The character that is being healed.</param>
        /// <param name="healthRestored">The amount of health that will be restored.</param>
        public void Heal(Character target, int healthRestored)
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(target.GetCharacterName() + " has regained " + healthRestored + " hit points!");
            target.StartCoroutine("GainHealth", healthRestored);
        }

        /// <summary>
        /// Kills the character.
        /// <remarks>This function should not be called, as the function is overidden in the child classes.</remarks>
        /// </summary>
        protected virtual void KillCharacter()
        {
            Debug.Log("This message should not appear!");
        }

        /// <summary>
        /// Retrives the ID used to identify each character in the battle. 
        /// </summary>
        /// <returns>The character ID.</returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Changes the character ID to a new one.
        /// </summary>
        /// <remarks>This is called when the battle starts, and is used to give each character in the battle a unique ID.</remarks>
        /// <param name="newID">The new ID to be assigned.</param>
        public void SetID (int newID)
        {
            ID = newID;
        }

        /// <summary>
        /// Retrives the name of the character.
        /// </summary>
        /// <returns>The character's name.</returns>
        public string GetCharacterName()
        {
            return characterName;
        }

        /// <summary>
        /// Retrieves the character's current health value.
        /// </summary>
        /// <returns>The character's current health.</returns>
        public int GetCurrentHealth()
        {
            return currentHP;
        }

        /// <summary>
        /// Retrieves the character's maximum health value.
        /// </summary>
        /// <returns>The character's maximum health.</returns>
        public int GetMaxHealth()
        {
            return maxHP;
        }

        /// <summary>
        /// Retrieves the character's attack value.
        /// </summary>
        /// <returns>The character's attack.</returns>
        public int GetAttack()
        {
            return attack;
        }

        /// <summary>
        /// Retrieves the character's defence value.
        /// </summary>
        /// <returns>The character's defence.</returns>
        public int GetDefence()
        {
            return defence;
        }

        /// <summary>
        /// Retrieves the character's magic attack value, used for when dealing damage with magic.
        /// </summary>
        /// <returns>The character's magic attack.</returns>
        public int GetMagicAttack()
        {
            return magicAttack;
        }

        /// <summary>
        /// Retrieves the character's magic defence value, used for when taking damage from magic.
        /// </summary>
        /// <returns>The character's magic defence.</returns>
        public int GetMagicDefence()
        {
            return magicDefence;
        }

        /// <summary>
        /// Retrieves the character's speed value.
        /// </summary>
        /// <returns>The character's speed.</returns>
        public int GetSpeed()
        {
            return speed;
        }

        /// <summary>
        /// Retrieves the character's sprite.
        /// </summary>
        /// <returns>The character's sprite.</returns>
        public Sprite GetSprite()
        {
            return characterSprite;
        }

        /// <summary>
        /// Checks if the character is a player.
        /// </summary>
        /// <returns>True if the character is a player, and false if the character is an enemy.</returns>
        public bool IsPlayer()
        {
            return isPlayer;
        }
    }
}
