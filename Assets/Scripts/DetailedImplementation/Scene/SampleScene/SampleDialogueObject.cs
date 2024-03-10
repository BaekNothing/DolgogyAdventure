using System;
using UnityEngine;
using ObjectSystem;
using Cysharp.Threading.Tasks;
using QFSW.QC;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class SampleDialogueObject : MonoBehaviour
{
    [SerializeField] string[] DialogueList;
    ClickableComponent _clickableComponent;

    public void Start()
    {
        //Initialize();
    }

    [Command("player_Init")]
    async void Initialize()
    {
        await UniTask.WaitUntil(() => CoreSystem.SystemRoot.IsInitialized);

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
