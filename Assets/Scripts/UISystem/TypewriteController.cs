using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;

namespace UIObject
{
    [Serializable]
    public class TypewriteController
    {
        [SerializeField] private TypewriterByCharacter Typewriter;
        public bool IsTyping => Typewriter.isShowingText;

        public void StartWriter()
        {
            Typewriter.StartShowingText();
        }

        public void SkipWriter()
        {
            Typewriter.SkipTypewriter();
        }
    }
}
