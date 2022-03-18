using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    //Contains data on the targets
    public class TargetData
    {
        protected Vector3 targetLocation;
        protected int targetID;
        protected string targetName;
        protected bool isPlayer;

        public TargetData(Vector3 iTargetLocation, int iTargetID, string iTargetName, bool iIsPlayer)
        {
            targetLocation = iTargetLocation;
            targetID = iTargetID;
            targetName = iTargetName;
            isPlayer = iIsPlayer;
        }

        public Vector3 GetLocation()
        {
            return targetLocation;
        }

        public int GetTargetID()
        {
            return targetID;
        }

        public string GetTargetName()
        {
            return targetName;
        }

        public bool IsTargetPlayer()
        {
            return isPlayer;
        }
    }

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

        void Start()
        {
            HideMarker();
        }

        void Update()
        {
            UpdateSelection();
            ConfirmTarget();
            Cancel();
        }

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

        private void ConfirmTarget()
        {
            if (Input.GetButtonDown("Submit"))
            {
                targetSelected = true;
                
            }
        }

        private void Cancel()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                playerMenu.DisplayBattleMenu();
                HideMarker();
            }
        }

        public void DisplayMarker()
        {
            this.gameObject.SetActive(true);
            if (targets[characterToTarget] == null)
            {
                //Find a new target
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null)
                    {
                        characterToTarget = i;
                        SetTarget();
                    }
                }
            }
        }

        public void HideMarker()
        {
            this.gameObject.SetActive(false);
        }

        public void SetTargets(TargetData[] newTargets)
        {
            //Updates the avalaible targets
            targets = newTargets;
            characterToTarget = 0;
            SetTarget();
        }

        public TargetData GetChosenTarget()
        {
            return targets[characterToTarget];
        }

        public bool IsTargetSelected()
        {
            return targetSelected;
        }

        public void TargetConfirmed()
        {
            targetSelected = false;
        }

    }
}
