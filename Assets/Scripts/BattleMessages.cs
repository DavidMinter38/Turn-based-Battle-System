using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem.Interface
{
    public class BattleMessages : MonoBehaviour
    {
        Text battleText;

        [SerializeField]
        Text previousBattleText;

        // Start is called before the first frame update
        void Start()
        {
            battleText = GetComponent<Text>();
            battleText.text = "";

            previousBattleText.text = "";
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateMessage(string message)
        {
            previousBattleText.text = battleText.text;
            battleText.text = message;
        }
    }
}
