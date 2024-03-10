using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ObjectSystem;
using UIObject;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class SampleDialogueObject : MonoBehaviour, IObject, IClickAble
{
    [SerializeField] ObjectState State = ObjectState.Enable;
    [SerializeField] string[] DialogueList;
    [SerializeField] Collider2D _collider;
    Collider2D Collider => _collider ??= GetComponent<Collider2D>();

    public void Initialized()
    {

    }

    public void Draw()
    {

    }

    public ObjectState Evaluator()
    {
        return State;
    }

    public void OnMouseDown()
    {
        Debug.Log("click_Down");
        if (DialogueList.Length == 0) return;
        CoreSystem.SystemRoot.Data.GetData<DialogueViewData>().SetDialogueTexts(DialogueList);
    }
}
