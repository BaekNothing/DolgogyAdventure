using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UIObject;

#pragma warning disable IDE0044 // ignore warning about private readonly field

public class DialogueView : ViewBase
{
    DialogueViewData DialogueViewData => CoreSystem.SystemRoot.Data.GetData<DialogueViewData>();

    [SerializeField] UIComponent _dialogueText;
    [SerializeField] UIComponent _dialogueButton;

    [SerializeField] TypewriteController _typewriteController;

    public override void Initialized()
    {
        SetDialogueText();
        SetDialogueButton();
    }

    void SetDialogueText()
    {
        UIComponent.UIComponentInitData _dialogueTextInitData = new(
            typeof(TextMeshProUGUI), DialogueViewData,
            ComponentUtility.EvaluatorImmutable,
            DrawDialogueText,
            this
        );

        InitComponent(_dialogueText, _dialogueTextInitData);
    }

    UIBehaviour DrawDialogueText(UIBehaviour body, IViewData data, ComponentStatus status)
    {
        ((TextMeshProUGUI)body).text = ((DialogueViewData)data).CurrentDialogueText;
        return body;
    }

    void SetDialogueButton()
    {
        UIComponent.UIComponentInitData _dialogueButtonInitData = new(
            typeof(Image), DialogueViewData,
            ComponentUtility.EvaluatorImmutable,
            ComponentUtility.DrawImmutable,
            this
        );

        InitComponent(_dialogueButton, _dialogueButtonInitData);

        _dialogueButton.SetAction<DialogueView>(DialogueAction);
    }

    void DialogueAction()
    {
        if (_typewriteController.IsTyping)
        {
            _typewriteController.SkipWriter();
        }
        else
        {
            DialogueViewData.Next();
            if (!DialogueViewData.IsDialogueEnd)
                _typewriteController.StartWriter();
        }
    }
}
