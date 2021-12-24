using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    int enemyID; //If we are having multiple types of enemies, this will allow us to figure out what type of enemy it is.

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int SelectAttackTarget(Character[] characters)
    {
        //TODO remove this function, as it is now redundant
        ArrayList avaliabletargets = new ArrayList();
        //Enemies can only attack players
        for(int i=0; i<characters.Length; i++)
        {
            if (characters[i].IsPlayer())
            {
                avaliabletargets.Add(characters[i]);
            }
        }
        Character targetCharacter = (Character)avaliabletargets[Random.Range(0, avaliabletargets.Count)];
        int targetID = targetCharacter.GetID();
        for(int i=0; i<characters.Length; i++)
        {
            int characterID = characters[i].GetID();
            if(characterID == targetID)
            {
                return i;
            }
        }

        return 0;
    }

    protected new void KillCharacter()
    {
        Debug.Log(this.GetCharacterName() + " has been destroyed!");
        Destroy(this.gameObject);
    }
}
