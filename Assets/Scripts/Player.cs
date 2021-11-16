using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    int playerID;  //This will be different for each player prefab, so that we know which player is which

    [SerializeField]
    protected int currentMP, maxMP;

    [SerializeField]
    bool knowsMagic;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(30);
        }
    }

    public bool CanUseMagic()
    {
        return knowsMagic;
    } 
}
