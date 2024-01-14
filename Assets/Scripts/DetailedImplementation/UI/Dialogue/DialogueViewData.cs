using UnityEngine;
using DataObject;

[CreateAssetMenu(fileName = "DialogueViewData", menuName = "ScriptableObjects/DialogueViewData", order = 1)]
public class DialogueViewData : AData
{
    [SerializeField] int _dialogueIndex = 0;
    public int DialogueIndex { get => _dialogueIndex; private set => _dialogueIndex = value; }

    public string CurrentDialogueText => DialogueIndex < DialogueTexts.Length ? DialogueTexts[DialogueIndex] : "";
    public int DialogueLength => DialogueTexts.Length;
    public bool IsDialogueEnd => DialogueIndex >= DialogueLength;

    [SerializeField] string[] _dialogueTexts = new string[] { "" };
    public string[] DialogueTexts { get => _dialogueTexts; private set => _dialogueTexts = value; }

    public override void Initialized()
    {

    }

    public void Next()
    {
        SetIndex(DialogueIndex + 1);
        if (IsDialogueEnd)
            CoreSystem.SystemRoot.UI.Pop<DialogueView>();
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
        CoreSystem.SystemRoot.UI.Focus<DialogueView>();
        SetValue(texts, nameof(DialogueTexts));
        SetIndex(0);
    }
}
