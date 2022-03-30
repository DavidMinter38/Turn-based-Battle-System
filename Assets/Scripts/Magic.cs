using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem.Data
{
    /// <summary>
    /// Contains data on magic, such as its cost and wether it affects players or enemies.
    /// </summary>
    public class Magic : MonoBehaviour
    {
        /// <summary>
        /// Infomation about magic.
        /// </summary>
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

        /// <summary>
        /// A list of all the magic that is avaliable to be used.
        /// </summary>
        [SerializeField]
        MagicStats[] magicList;

        /// <summary>
        /// Retrieves stored data on a particular piece of magic.
        /// </summary>
        /// <param name="magicID">The ID of the magic that is needed.</param>
        /// <returns>The magic data.</returns>
        public MagicStats GetMagicInfomation(int magicID)
        {
            return magicList[magicID];
        }
    }
}
