using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    /// <summary>
    /// The Target Marker is used to allow the player to select which character they want to use an action on.
    /// </summary>
    public class TargetMarker : MonoBehaviour
    {
        [SerializeField]
        PlayerBattleMenu playerMenu;

        [SerializeField]
        Text targetName;

        TargetData[] targets;
        int characterToTarget = 0;
        float distanceAboveTarget = 1.5f;
        bool inputPressed = false;
        bool targetSelected = false;
        bool cancelled = false;

        /// <summary>
        /// Update checks for any input from the player.
        /// </summary>
        void Update()
        {
            UpdateSelection();
            ConfirmTarget();
            Cancel();
        }

        /// <summary>
        /// Sets the position of the target marker to be above the new target.
        /// </summary>
        private void SetTarget()
        {
            if (targets[characterToTarget] == null)
            {
                FindNewTarget();
            }
            Vector3 targetMarkerPosition = targets[characterToTarget].GetLocation();
            targetMarkerPosition.y += distanceAboveTarget;
            transform.position = targetMarkerPosition;
            targetName.text = targets[characterToTarget].GetTargetName();
        }

        /// <summary>
        /// Find a new target for the target marker to point to.
        /// </summary>
        /// <remarks>This is used in case a target can not be found.</remarks>
        private void FindNewTarget()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    characterToTarget = i;
                }
            }
        }

        /// <summary>
        /// If left or right is pressed, move the target marker to the next target in line.
        /// </summary>
        private void UpdateSelection()
        {
            if (!inputPressed)
            {
                if (Input.GetAxis("Horizontal") < 0)
                {
                    inputPressed = true;
                    do
                    {
                        characterToTarget--;
                        if (characterToTarget < 0)
                        {
                            characterToTarget = targets.Length - 1;
                        }
                    } while (targets[characterToTarget] == null);

                }
                if (Input.GetAxis("Horizontal") > 0)
                {
                    inputPressed = true;
                    do
                    {
                        characterToTarget++;
                        if (characterToTarget > targets.Length - 1)
                        {
                            characterToTarget = 0;
                        }
                    } while (targets[characterToTarget] == null);
                }
                SetTarget();
            }
            if (Input.GetAxis("Horizontal") == 0)
            {
                inputPressed = false;
            }
        }

        /// <summary>
        /// Confirm that a target has been selected, so that the UI Manager can perform the appropriate action.
        /// </summary>
        private void ConfirmTarget()
        {
            if (Input.GetButtonDown("Submit"))
            {
                targetSelected = true;
                
            }
        }

        /// <summary>
        /// Confirm that the cancellation button has been pressed, so that the UI Manager can return to the battle menu.
        /// </summary>
        private void Cancel()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                cancelled = true;
            }
        }

        /// <summary>
        /// Updates the list of targets that can be selected.
        /// </summary>
        /// <param name="newTargets">Data on the new targets.</param>
        public void SetTargets(TargetData[] newTargets)
        {
            //Updates the avalaible targets
            targets = newTargets;
            characterToTarget = 0;
            SetTarget();
        }

        /// <summary>
        /// Retrieves data on the chosen target.
        /// </summary>
        /// <returns>The chosen target's data.</returns>
        public TargetData GetChosenTarget()
        {
            return targets[characterToTarget];
        }

        /// <summary>
        /// Check if a target has been selected by the user.
        /// </summary>
        /// <returns>True if a target has been selected, false if a target has not been selected.</returns>
        public bool IsTargetSelected()
        {
            return targetSelected;
        }

        /// <summary>
        /// Checks if the user has chosen to cancel selecting a target.
        /// </summary>
        /// <returns>True if the cancel button had been pressed, false if it had not.</returns>
        public bool HasCancelled()
        {
            return cancelled;
        }

        /// <summary>
        /// Confirms target selection and allows for another target to be selected in the future.
        /// </summary>
        public void TargetConfirmed()
        {
            targetSelected = false;
        }

        /// <summary>
        /// Confirms cancellation and allows the user to cancel out of selecting a target in the future.
        /// </summary>
        public void CancelConfirmed()
        {
            cancelled = false;
        }

    }

    /// <summary>
    /// The TargetData class is used to store infomation on the avaliable targets.
    /// </summary>
    public class TargetData
    {
        protected Vector3 targetLocation;
        protected int targetID;
        protected string targetName;
        protected bool isPlayer;

        /// <summary>
        /// The constructor stores character values.
        /// </summary>
        /// <param name="iTargetLocation">The location of the character on the screen.</param>
        /// <param name="iTargetID">The character's ID.</param>
        /// <param name="iTargetName">The character's name.</param>
        /// <param name="iIsPlayer">True if the character is a player, false if it's an enemy.</param>
        public TargetData(Vector3 iTargetLocation, int iTargetID, string iTargetName, bool iIsPlayer)
        {
            targetLocation = iTargetLocation;
            targetID = iTargetID;
            targetName = iTargetName;
            isPlayer = iIsPlayer;
        }

        /// <summary>
        /// Retrieves the location of the target.
        /// </summary>
        /// <returns>The target's location.</returns>
        public Vector3 GetLocation()
        {
            return targetLocation;
        }

        /// <summary>
        /// Retrieves the ID of the target.
        /// </summary>
        /// <returns>The target's ID.</returns>
        public int GetTargetID()
        {
            return targetID;
        }

        /// <summary>
        /// Retrieves the name of the target.
        /// </summary>
        /// <returns>The target's name.</returns>
        public string GetTargetName()
        {
            return targetName;
        }

        /// <summary>
        /// Checks if the target is a player.
        /// </summary>
        /// <returns>True if the target is a player, and false if the target is an enemy.</returns>
        public bool IsTargetPlayer()
        {
            return isPlayer;
        }
    }
}
