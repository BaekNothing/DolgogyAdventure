using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    [CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)]
    public class DialogueData : ScriptableObject, IViewData
    {
        [SerializeField] int DialogueIndex = 0;
        public string CurrentDialogueText => DialogueTexts[DialogueIndex];
        public int DialogueLength => DialogueTexts.Length;
        [SerializeField]
        string[] DialogueTexts = new string[]
        {
            "the first rule of fight club is you do not talk about fight club",
            "the second rule of fight club is you do not talk about fight club",
            "the third rule of fight club is when someone says stop or goes limp, the fight is over",
            "the fourth rule of fight club is only two guys to a fight",
            "the fifth rule of fight club is one fight at a time, fellas",
            "the sixth rule of fight club is no shirts, no shoes",
            "the seventh rule of fight club is fights will go on as long as they have to",
            "the eighth and final rule of fight club is if this is your first night at fight club, you have to fight"
        };

        public int GetIndex => DialogueIndex;

        public void SetIndex(int index)
        {
            DialogueIndex++;
            if (DialogueIndex >= DialogueLength)
                DialogueIndex = 0;
        }

        public void SetDialogueTexts(string[] texts)
        {
            DialogueTexts = texts;
            SetIndex(0);
        }
    }
}
