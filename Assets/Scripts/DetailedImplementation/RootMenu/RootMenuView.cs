using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UIObject;

#pragma warning disable IDE0044 // ignore warning about private readonly field

public class RootMenuView : ViewBase
{
    [SerializeField] RootMenuData _rootMenuData;

    [SerializeField] UIComponent _debugButton;

    public override void Initialized()
    {
        SetDebugButton();
        Utility.Logger.Log($"{this.gameObject.name} RootMenuView.Initialized");
    }

    void SetDebugButton()
    {
        UIComponent.UIComponentInitData _debugButtonInitData =
        new(
            typeof(Button), _rootMenuData,
            ComponentUtility.EvaluatorImmutable,
            ComponentUtility.DrawWithStatusOnly,
            this
        );

        InitComponent(_debugButton, _debugButtonInitData);

        void _debugButtonAction()
        {
            Utility.Logger.Log
                ($"{this.gameObject.name} RootMenuView.SetDebugButton: debug button is clicked",
                Utility.Logger.Importance.Warning);

            CoreSystem.SystemRoot.Data.DialogueData.SetDialogueTexts(
                new string[] { $"Debug Button is Clicked {System.DateTime.Now}" }
            );
        }

        _debugButton.SetAction<RootMenuView>(_debugButtonAction);
    }
}
