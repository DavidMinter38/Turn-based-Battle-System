using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem.Data
{
    public class Magic : MonoBehaviour
    {
        [System.Serializable]
        public struct MagicStats
        {
            public int magicID;
            public string magicName;
            public string magicDescription;
            public int magicCost;
            public int magicStrength;
            public bool affectsAll;
            public bool affectsPlayers;
            public bool restores;
            public bool affectsDead;
        }

        [SerializeField]
        MagicStats[] magicList;

        public MagicStats GetMagicInfomation(int magicID)
        {
            return magicList[magicID];
        }
    }
}
