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
    [SerializeField] DialogueData _dialogueData;

    [SerializeField] UIComponent _dialoguePanel;
    [SerializeField] UIComponent _dialogueText;
    [SerializeField] UIComponent _dialogueButton;

    [SerializeField] TypewriteController _typewriteController;

    protected override void Initialized()
    {
        SetDialoguePanel();
        SetDialogueText();
        SetDialogueButton();
    }

    void SetDialoguePanel()
    {

        UIComponent.UIComponentInitData _dialoguePanelInitData =
        new(
            typeof(Image), _dialogueData,
            EvaluateDialoguePanel,
            ComponentUtility.DrawWithStatusOnly,
            this
        );

        InitComponent(_dialoguePanel, _dialoguePanelInitData);
    }

    ComponentStatus EvaluateDialoguePanel(IViewData data, ComponentStatus status)
    {
        if (data == null)
            throw new System.ArgumentNullException($"{this.gameObject.name} ViewBase.InitComponent: component is null");

        if (data is DialogueData dialogueData)
        {
            if (dialogueData.IsDialogueEnd)
                return ComponentStatus.Disable;
            else
                return ComponentStatus.Enable;
        }
        else
            throw new System.ArgumentException($"{this.gameObject.name} ViewBase.InitComponent: data is not DialogueData");
    }

    void SetDialogueText()
    {
        UIComponent.UIComponentInitData _dialogueTextInitData = new(
            typeof(TextMeshProUGUI), _dialogueData,
            ComponentUtility.EvaluatorImmutable,
            DrawDialogueText,
            this
        );

        InitComponent(_dialogueText, _dialogueTextInitData);
    }

    UIBehaviour DrawDialogueText(UIBehaviour body, IViewData data, ComponentStatus status)
    {
        if (!(body as TextMeshProUGUI).text.Contains(((DialogueData)data).CurrentDialogueText))
            (body as TextMeshProUGUI).text = ((DialogueData)data).CurrentDialogueText;
        return body;
    }

    void SetDialogueButton()
    {
        UIComponent.UIComponentInitData _dialogueButtonInitData = new(
            typeof(Image), _dialogueData,
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
            _dialogueData.Next();
            if (!_dialogueData.IsDialogueEnd)
                _typewriteController.StartWriter();
        }
    }
}
