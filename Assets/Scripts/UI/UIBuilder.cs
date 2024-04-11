using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dolgoji.UIComponent
{
    public class UIBuilder
    {

        static void SetBasicTransform(RectTransform target, UIBlueprintComponent component, Transform parent)
        {
            target.SetParent(parent);
            target.localPosition = component.Position;
            target.sizeDelta = component.Size;

            switch (component.RectAnchorType)
            {
                case AnchorType.LeftTop:
                    target.anchorMin = new Vector2(0, 1);
                    target.anchorMax = new Vector2(0, 1);
                    break;
                case AnchorType.RightTop:
                    target.anchorMin = new Vector2(1, 1);
                    target.anchorMax = new Vector2(1, 1);
                    break;
                case AnchorType.LeftBottom:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(0, 0);
                    break;
                case AnchorType.RightBottom:
                    target.anchorMin = new Vector2(1, 0);
                    target.anchorMax = new Vector2(1, 0);
                    break;
                case AnchorType.Center:
                    target.anchorMin = new Vector2(0.5f, 0.5f);
                    target.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
            }

        }

        public static GameObject Build(string path, Transform parent)
        {
            UIBlueprint blueprint = Resources.Load<UIBlueprint>(path);
            if (blueprint == null)
            {
                Debug.LogError("UIBlueprint is not found in Resources");
                return null;
            }

            GameObject uiObject = new(blueprint.name);
            CreateComponent(blueprint.ComponentGroups, uiObject);

            return uiObject;
        }

        static void CreateComponent(UIBlueprintGroups[] componentGroups, GameObject parent)
        {
            foreach (UIBlueprintGroups group in componentGroups)
            {
                GameObject groupObject = new(group.Name);
                groupObject.transform.SetParent(parent.transform);

                foreach (UIBlueprintComponent component in group.Components)
                {
                    GameObject componentObject = new(component.Name);
                    componentObject.transform.SetParent(groupObject.transform);

                    switch (component.Type)
                    {
                        case ComponentType.Preset:
                            break;
                        case ComponentType.Text:
                            CreateTextComponent(component, componentObject);
                            break;
                        case ComponentType.Image:
                            CreateImageComponent(component, componentObject);
                            break;
                    }
                }
            }
        }

        static void CreateTextComponent(UIBlueprintComponent component, GameObject parent)
        {
            Text text = parent.AddComponent<Text>();
            text.text = component.Source;
            text.font = component.TextStylePreset.Font;
            text.fontSize = component.TextStylePreset.FontSize;
            text.color = component.TextStylePreset.FontColor;
            text.alignment = component.TextStylePreset.Alignment;
            text.resizeTextForBestFit = true;
            text.resizeTextMaxSize = component.TextStylePreset.FontSize;
            text.resizeTextMinSize = 4;

            text.raycastTarget = component.IsRaycastTarget;

            RectTransform rectTransform = parent.GetComponent<RectTransform>();
            SetBasicTransform(rectTransform, component, parent.transform);
            rectTransform.anchoredPosition = component.Position;
        }

        static void CreateImageComponent(UIBlueprintComponent component, GameObject parent)
        {
            Image image = parent.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>(component.Source);
            image.raycastTarget = component.IsRaycastTarget;

            RectTransform rectTransform = parent.GetComponent<RectTransform>();
            SetBasicTransform(rectTransform, component, parent.transform);
            rectTransform.anchoredPosition = component.Position;
        }
    }
}
