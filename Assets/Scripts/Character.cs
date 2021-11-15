using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    int ID;

    protected int health, attack, defence, speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            //Character dies
        }
    }

    IEnumerator Attack(Character target, int attack)
    {
        target.TakeDamage(attack);
        //TODO make the damage more varied, and also have it influenced by correct button input timing
        yield return new WaitForSeconds(1f);
    }
}
