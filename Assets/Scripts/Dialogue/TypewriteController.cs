using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;

[Serializable]
public class TypewriteController
{
    [SerializeField] private TypewriterByCharacter Typewriter;
    [SerializeField] private TextAnimator_TMP TextAnimator;

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
