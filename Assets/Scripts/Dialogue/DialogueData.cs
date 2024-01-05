using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    [CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)]
    public class DialogueData : ViewData
    {
        [SerializeField] int _dialogueIndex = 0;
        public int DialogueIndex { get => _dialogueIndex; private set => _dialogueIndex = value; }

        public string CurrentDialogueText => DialogueIndex < DialogueTexts.Length ? DialogueTexts[DialogueIndex] : "";
        public int DialogueLength => DialogueTexts.Length;
        public bool IsDialogueEnd => DialogueIndex >= DialogueLength;

        [SerializeField]
        string[] _dialogueTexts = new string[]
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
        public string[] DialogueTexts { get => _dialogueTexts; private set => _dialogueTexts = value; }

        public override void Initialized()
        {
            SetIndex(0);
        }

        public void Next()
        {
            SetIndex(DialogueIndex + 1);
        }

        public void Previous()
        {
            int index = DialogueIndex - 1;
            if (index < 0)
                index = 0;
            SetIndex(index);
        }

        void SetIndex(int index)
        {
            SetValue(index, nameof(DialogueIndex));
        }

        public void SetDialogueTexts(string[] texts)
        {
            SetValue(texts, nameof(DialogueTexts));
            SetIndex(0);
        }
    }
}
