using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField]
    int ID;  //This allows us to identify each character in the battle

    [SerializeField]
    string characterName;

    [SerializeField]
    protected int currentHP, maxHP, attack, defence, speed;
    //TODO remove serializefield and initialize value using GameData class

    [SerializeField]
    Sprite characterSprite;

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
        FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " gained " + healthToRecover.ToString() + " hit points!");
    }

    protected void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) { currentHP = 0; }

        FindObjectOfType<BattleUI>().UpdateUI();
        //TODO Make this so the system does not have to find UI.

        if(currentHP <= 0)
        {
            //Character dies
            KillCharacter();
        }
    }

    public void Attack(Character target, int attackDamage)
    {
        FindObjectOfType<BattleMessages>().UpdateMessage(this.GetCharacterName() + " has attacked " + target.GetCharacterName() + "!  Dealt " + attackDamage + " damage!");
        target.TakeDamage(attackDamage);
        FindObjectOfType<GameManager>().NextTurn(true);
        //TODO make the damage more varied, and also have it influenced by correct button input timing
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
