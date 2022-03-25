using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    /// <summary>
    /// The TurnOrderTrack script stores data on the turn order.
    /// </summary>
    public class TurnOrderTrack : MonoBehaviour
    {
        [SerializeField]
        Image[] trackImages;

        [SerializeField]
        Sprite emptySprite;

        /// <summary>
        /// Updates the track with new sprites, showing the new turn order.
        /// </summary>
        /// <param name="sprites">The array of sprites to be used.</param>
        public void UpdateTrack(ArrayList sprites)
        {
            int trackCounter = 0;
            foreach (Sprite sprite in sprites)
            {
                trackImages[trackCounter].sprite = sprite;
                trackCounter++;
                if(trackCounter >= trackImages.Length) { return; }
            }
            //Any leftoverspaces in the track are left empty
            for (int i = trackCounter; i < trackImages.Length; i++)
            {
                trackImages[i].sprite = emptySprite;
            }
        }
    }
}
