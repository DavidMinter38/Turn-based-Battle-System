using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnData
{
    int ID;
    float speedValue;
    bool player;

    public TurnData(int inputID, float inputSpeedValue, bool inputPlayer)
    {
        ID = inputID;
        speedValue = inputSpeedValue;
        player = inputPlayer;
    }

    public int GetID()
    {
        return ID;
    }

    public float GetSpeed()
    {
        return speedValue;
    }

    public bool IsPlayer()
    {
        return player;
    }
}
