using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UISystem
{
    public class DialogueView : ViewBase
    {
        [SerializeField] UIComponent _dialogueText;
        [SerializeField] UIComponent _dialogueButton;
        [SerializeField] DialogueData _viewData;
        [SerializeField] TypewriteController _typewriteController;

        protected override void Initialized()
        {
            SetDialogueText();
            SetDialogueButton();
        }

        void SetDialogueText()
        {
            UIComponent.UIComponentInitData _dialogueTextInitData = new(
                typeof(TextMeshProUGUI), _viewData,
                ComponentUtility.EvaluatorImmutable,
                DrawDialogueText
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
                typeof(Image), _viewData,
                ComponentUtility.EvaluatorImmutable,
                ComponentUtility.DrawImmutable
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
                _viewData.SetIndex(Random.Range(0, _viewData.DialogueLength));
                _typewriteController.StartWriter();
            }
        }
    }
}
