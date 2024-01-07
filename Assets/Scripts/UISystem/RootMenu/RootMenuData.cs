using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIObject;

[CreateAssetMenu(fileName = "RootMenuData", menuName = "ScriptableObjects/RootMenuData", order = 1)]
public class RootMenuData : ViewData
{
    public override void Initialized()
    {
        Utility.Logger.Log($"RootMenuData.Initialized");
    }
}
