using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataObject;

[CreateAssetMenu(fileName = "RootMenuViewData", menuName = "ScriptableObjects/RootMenuViewData", order = 1)]
public class RootMenuViewData : AData
{
    public override void Initialized()
    {
        Utility.Logger.Log($"RootMenuViewData.Initialized");
    }
}
