using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem.Gameplay;
using BattleSystem.Data;
using BattleSystem.Interface;

namespace BattleSystem.Characters
{
    public class Player : Character
    {
        [SerializeField]
        int playerID;  //This will be different for each player prefab, so that we know which player is which

        protected int currentMP, maxMP;

        bool knowsMagic;
        bool[] avaliableMagic;

        bool isAvaliable = false;
        bool isConscious = true;
        bool isGuarding = false;

        bool inCombat = true; //Used to prevent revived players from taking action on the same turn that they are revived

        PlayerHealthUI playerUI;
        float rateOfUIChange = 0.75f;

        [SerializeField]
        SpriteRenderer guardIcon;

        // Start is called before the first frame update
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

        // Update is called once per frame
        void Update()
        {

        }

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

        public void UseGuard()
        {
            isGuarding = true;
            FindObjectOfType<GameManager>().NextTurn(true);
            guardIcon.gameObject.SetActive(true);
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has got their guard up!");
        }

        public void FinishGuard()
        {
            isGuarding = false;
            guardIcon.gameObject.SetActive(false);
        }

        protected override void KillCharacter()
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has fallen!");
            isGuarding = false;
            guardIcon.gameObject.SetActive(false);
            isConscious = false;
            inCombat = false;
            FindObjectOfType<GameManager>().CreateTrack();
            this.gameObject.transform.Rotate(new Vector3(0, 0, 90));
        }

        public void Revive()
        {
            FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has been revived!");
            isConscious = true;
            this.gameObject.transform.Rotate(new Vector3(0, 0, -90));
        }

        public void ReturnToCombat()
        {
            inCombat = true;
        }

        public int GetPlayerID()
        {
            return playerID;
        }

        public int GetCurrentMagic()
        {
            return currentMP;
        }

        public int GetMaxMagic()
        {
            return maxMP;
        }

        public bool CanUseMagic()
        {
            return knowsMagic;
        }

        public bool[] GetKnownMagic()
        {
            return avaliableMagic;
        }

        public bool IsAvaliable()
        {
            return isAvaliable;
        }

        public bool IsConscious()
        {
            return isConscious;
        }

        public bool IsGuarding()
        {
            return isGuarding;
        }

        public bool IsInCombat()
        {
            return inCombat;
        }
    }
}
