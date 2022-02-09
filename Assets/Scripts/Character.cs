using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField]
    int ID;  //This allows us to identify each character in the battle

    [SerializeField]
    protected string characterName;

    protected int currentHP, maxHP, attack, defence, magicAttack, magicDefence, speed;

    protected Sprite characterSprite;

    protected bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }

    protected void GainHealth(int healthToRecover)
    {
        currentHP += healthToRecover;
        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        FindObjectOfType<BattleUI>().UpdateUI();
    }

    protected void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) { currentHP = 0; }

        FindObjectOfType<BattleUI>().UpdateUI();

        if(currentHP <= 0)
        {
            //Character dies
            KillCharacter();
        }
    }

    public void Attack(Character target, int attackDamage)
    {
        FindObjectOfType<BattleMessages>().UpdateMessage("Dealt " + attackDamage + " damage to " + target.GetCharacterName() + "!");
        target.TakeDamage(attackDamage);
        //TODO make the damage more varied, and also have it influenced by correct button input timing
    }

    public void Heal(Character target, int healthRestored)
    {
        FindObjectOfType<BattleMessages>().UpdateMessage(target.GetCharacterName() + " has regained " + healthRestored + " hit points!");
        target.GainHealth(healthRestored);
    }

    protected virtual void KillCharacter()
    {
        Debug.Log("This message should not appear!");
    }

    public int GetID()
    {
        return ID;
    }

    public string GetCharacterName()
    {
        return characterName;
    }

    public int GetCurrentHealth()
    {
        return currentHP;
    }

    public int GetMaxHealth()
    {
        return maxHP;
    }

    public int GetAttack()
    {
        return attack;
    }

    public int GetDefence()
    {
        return defence;
    }
    public int GetMagicAttack()
    {
        return magicAttack;
    }

    public int GetMagicDefence()
    {
        return magicDefence;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public Sprite GetSprite()
    {
        return characterSprite;
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }
}
