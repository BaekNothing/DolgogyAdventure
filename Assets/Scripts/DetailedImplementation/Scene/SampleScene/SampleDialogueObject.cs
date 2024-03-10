using System;
using UnityEngine;
using ObjectSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class SampleDialogueObject : ASceneObjectBase
{
    [SerializeField] string[] DialogueList;
    ClickableComponent _clickableComponent;

    public override void Initialize()
    {
        _clickableComponent = gameObject.AddComponent<ClickableComponent>();
        _clickableComponent.SetAction(ClickAction);
    }

    public void ClickAction()
    {
        Debug.Log("click_Down");
        if (DialogueList.Length == 0) return;
        CoreSystem.SystemRoot.Data.GetData<DialogueViewData>().SetDialogueTexts(DialogueList);
    }
}
