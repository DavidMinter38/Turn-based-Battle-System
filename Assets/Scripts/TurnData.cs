using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem.Data
{
    /// <summary>
    /// The TurnData class is used to contain character data when setting up turns in combat.
    /// </summary>
    public class TurnData
    {
        /// <summary>
        /// The character's ID.
        /// </summary>
        int ID;
        /// <summary>
        /// The character's speed value.
        /// </summary>
        float speedValue;
        /// <summary>
        /// If the character is a player, this will be set to true.
        /// </summary>
        bool player;

        /// <summary>
        /// The constructor stores the player's values.
        /// </summary>
        /// <param name="inputID">The ID of the character.</param>
        /// <param name="inputSpeedValue">The character's speed value.</param>
        /// <param name="inputPlayer">True if the character is a player, false if it's an enemy.</param>
        public TurnData(int inputID, float inputSpeedValue, bool inputPlayer)
        {
            ID = inputID;
            speedValue = inputSpeedValue;
            player = inputPlayer;
        }

        /// <summary>
        /// Retrieves the ID to the character.
        /// </summary>
        /// <returns>The character's ID.</returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Retrieves the speed value of the character.
        /// </summary>
        /// <returns>The character's speed value.</returns>
        public float GetSpeed()
        {
            return speedValue;
        }

        /// <summary>
        /// Checks if the character stored is a player.
        /// </summary>
        /// <returns>True if the character is a player, false if it's an enemy.</returns>
        public bool IsPlayer()
        {
            return player;
        }
    }
}
