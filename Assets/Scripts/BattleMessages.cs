using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    /// <summary>
    /// The BattleMessages script informs the player of recent actions, such as how much damage was dealt to an enemy.
    /// </summary>
    public class BattleMessages : MonoBehaviour
    {
        /// <summary>
        /// Contains the newest message.
        /// </summary>
        Text battleText;

        /// <summary>
        /// When a new message is displayed, the previous message is stored here.
        /// </summary>
        [SerializeField]
        Text previousBattleText;

        /// <summary>
        /// On startup, all text is cleared.
        /// </summary>
        void Start()
        {
            battleText = GetComponent<Text>();
            battleText.text = "";

            previousBattleText.text = "";
        }

        /// <summary>
        /// Updates the display with the new message, and then moves the old message to the second text box.
        /// This is so that, in the event that multiple messages are displayed in succession, the player can see both messages.
        /// </summary>
        /// <param name="message">The message that is to be displayed.</param>
        public void UpdateMessage(string message)
        {
            previousBattleText.text = battleText.text;
            battleText.text = message;
        }
    }
}
