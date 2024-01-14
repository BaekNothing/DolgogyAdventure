using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataObject;

[CreateAssetMenu(fileName = "RootMenuData", menuName = "ScriptableObjects/RootMenuData", order = 1)]
public class RootMenuData : AData
{
    public override void Initialized()
    {
        Utility.Logger.Log($"RootMenuData.Initialized");
    }
}
