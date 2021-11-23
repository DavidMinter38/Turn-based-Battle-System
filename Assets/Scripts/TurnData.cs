using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnData
{
    int ID;
    float speedValue;

    public TurnData(int inputID, float inputSpeedValue)
    {
        ID = inputID;
        speedValue = inputSpeedValue;
    }

    public int GetID()
    {
        return ID;
    }

    public float GetSpeed()
    {
        return speedValue;
    }
}
