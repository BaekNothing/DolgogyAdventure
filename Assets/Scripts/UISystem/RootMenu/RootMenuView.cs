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
    protected override void Initialized()
    {
        Utility.Logger.Log($"{this.gameObject.name} RootMenuView.Initialized");
    }
}
