using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BattleSystem.Gameplay;
using BattleSystem.Data;

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

        Magic.MagicStats selectedMagic;
        bool isUsingMagic;

        // Start is called before the first frame update
        void Start()
        {
            targets = FindObjectOfType<GameManager>().GetEnemyData();
            if (targets == null) { Debug.LogError("No enemies are active in the scene."); }

            SetTarget();
            this.gameObject.SetActive(false);
        }

        // Update is called once per frame
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
                if (isUsingMagic)
                {
                    FindObjectOfType<GameManager>().GetCurrentTurnPlayer().StartCoroutine("LoseMagic", selectedMagic.magicCost);
                    FindObjectOfType<BattleMessages>().UpdateMessage(FindObjectOfType<GameManager>().GetCurrentTurnPlayer().GetCharacterName() + " casts " + (selectedMagic.magicName + "!"));
                    if (selectedMagic.restores)
                    {
                        if (selectedMagic.affectsDead && targets[characterToTarget].IsTargetPlayer())
                        {
                            FindObjectOfType<GameManager>().RevivePlayer(targets[characterToTarget].GetTargetID());
                        }
                        FindObjectOfType<GameManager>().Heal(targets[characterToTarget].GetTargetID(), false, selectedMagic.magicStrength);
                    }
                    else
                    {
                        FindObjectOfType<GameManager>().Attack(targets[characterToTarget].GetTargetID(), false, true, selectedMagic.magicStrength);
                    }
                }
                else
                {
                    FindObjectOfType<GameManager>().Attack(targets[characterToTarget].GetTargetID(), false, false, 0);
                }
                HideMarker();
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

        public void DisplayMarker(bool usingMagic)
        {
            this.gameObject.SetActive(true);
            isUsingMagic = usingMagic;
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

        public void SetMagicInfomation(Magic.MagicStats magic)
        {
            selectedMagic = magic;
        }
    }
}
