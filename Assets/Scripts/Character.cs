using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    int ID;  //This allows us to identify each character in the battle

    [SerializeField]
    string characterName;

    [SerializeField]
    protected int currentHP, maxHP, attack, defence, speed;
    //TODO remove serializefield and initialize value using GameData class

    protected bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }

    void GainHealth(int healthToRecover)
    {
        currentHP += healthToRecover;
        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    protected void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) { currentHP = 0; }

        FindObjectOfType<BattleUI>().UpdateHealth();
        //TODO Make this so the system does not have to find UI.

        if(currentHP <= 0)
        {
            //Character dies
            KillCharacter();
        }
    }

    public IEnumerator Attack(Character target, int attackDamage)
    {
        Debug.Log(this.GetCharacterName() + " has attacked " + target.GetCharacterName() + "!  Dealt " + attackDamage + " damage!");
        target.TakeDamage(attackDamage);
        FindObjectOfType<GameManager>().NextTurn();
        //TODO make the damage more varied, and also have it influenced by correct button input timing
        yield return new WaitForSeconds(1f);
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

    public bool IsPlayer()
    {
        return isPlayer;
    }
}
