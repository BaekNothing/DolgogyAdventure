using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dolgoji.UIComponent
{

    [CreateAssetMenu(fileName = "BlueprintComponent", menuName = "UI/BlueprintComponent")]
    class UIBlueprintComponent : ScriptableObject
    {
        public ComponentType Type = ComponentType.Preset;
        public ScriptableObject Data;

        public string Name = "";
        public string Source = "";
        public ComponentTextStylePreset TextStylePreset;
        public bool IsRaycastTarget = false;

        public Vector3 Position = new(0, 0, 0);
        public Vector2 Size = new(0, 0);
        public AnchorType RectAnchorType = AnchorType.LeftTop;
    }

    // UIBlueprintComponent Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(UIBlueprintComponent))]
    public class UIBlueprintComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            UIBlueprintComponent componentInfo = target as UIBlueprintComponent;

            if (componentInfo.Type == ComponentType.Preset)
            {
                componentInfo.Type = (ComponentType)EditorGUILayout.EnumPopup("Type", componentInfo.Type);
                EditorGUILayout.LabelField("Name", componentInfo.Data?.name ?? "null");
                componentInfo.Data = EditorGUILayout.ObjectField("Data", componentInfo.Data, typeof(ScriptableObject), false) as ScriptableObject;
            }
            else
            {
                componentInfo.Type = (ComponentType)EditorGUILayout.EnumPopup("Type", componentInfo.Type);
                componentInfo.Name = EditorGUILayout.TextField("Name", componentInfo.Name);
                componentInfo.Source = EditorGUILayout.TextField("Source", componentInfo.Source);
                if (componentInfo.Type == ComponentType.Text)
                {
                    componentInfo.TextStylePreset = EditorGUILayout.ObjectField("TextStyle", componentInfo.TextStylePreset, typeof(ComponentTextStylePreset), false) as ComponentTextStylePreset;
                }

                componentInfo.Position = EditorGUILayout.Vector3Field("Position", componentInfo.Position);
                componentInfo.Size = EditorGUILayout.Vector2Field("Size", componentInfo.Size);
                componentInfo.RectAnchorType = (AnchorType)EditorGUILayout.EnumPopup("AnchorType", componentInfo.RectAnchorType);

                componentInfo.IsRaycastTarget = EditorGUILayout.Toggle("IsRaycastTarget", componentInfo.IsRaycastTarget);
            }

            EditorUtility.SetDirty(target);
        }
    }

#endif
}
