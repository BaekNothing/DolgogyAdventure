using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dolgoji.UIComponent
{
    public class UIBuilder
    {
        static UIBlueprintOptions _options;
        static UIBlueprintOptions Options
        {
            get
            {
                _options ??= Resources.Load<UIBlueprintOptions>("UIBlueprintOptions");
                return _options;
            }
        }


        static void SetBasicTransform(RectTransform target, Vector2 size, Transform parent)
        {
            target.SetParent(parent);
            target.localPosition = Vector3.zero;
            target.pivot = new Vector2(0.5f, 0.5f);
            target.anchorMin = new Vector2(0.5f, 0.5f);
            target.anchorMax = new Vector2(0.5f, 0.5f);
            target.sizeDelta = size;
            target.localScale = Vector2.one;
        }

        public static GameObject Build(string path, Transform parent)
        {
            UIBluetprint blueprint = Resources.Load<UIBluetprint>(path);
            if (blueprint == null)
            {
                Debug.LogError("UIBlueprint is not found in Resources");
                return null;
            }

            GameObject uiObject = new(blueprint.Name);
            uiObject.transform.SetParent(parent);
            RectTransform uiRect = uiObject.AddComponent<RectTransform>();
            SetBasicTransform(uiRect, blueprint.ScrollInfo.Size, parent);
            uiRect.localPosition = blueprint.ScrollInfo.Position;

            ScrollRect scroll = MakeScrollRect(uiObject, blueprint.ScrollInfo);
            SetComponents(scroll, blueprint.Components);

            return uiObject;
        }

        static ScrollRect MakeScrollRect(GameObject target, UIBluetprint.scrollInfo scrollInfo)
        {
            target.AddComponent<Image>().sprite = Options.BgImage?.sprite;
            target.GetComponent<Image>().color = Options.backgroundColor;

            ScrollRect scroll = target.AddComponent<ScrollRect>();
            scroll.horizontal = scrollInfo.direction == UIBluetprint.scrollInfo.Direction.Horizontal;
            scroll.vertical = scrollInfo.direction == UIBluetprint.scrollInfo.Direction.Vertical;

            scroll.viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            SetBasicTransform(scroll.viewport, scrollInfo.Size - scrollInfo.Padding * 2, target.transform);

            GameObject content = new("Content");
            scroll.content = content.AddComponent<RectTransform>();
            SetBasicTransform(scroll.content, scrollInfo.Size - scrollInfo.Padding * 2, scroll.viewport.transform);

            content.AddComponent<ContentSizeFitter>();
            HorizontalOrVerticalLayoutGroup LayoutGroup =
            scroll.horizontal
                ? content.AddComponent<HorizontalLayoutGroup>()
                : content.AddComponent<VerticalLayoutGroup>();

            LayoutGroup.spacing = Options.spacing;
            LayoutGroup.padding = Options.pedding;
            LayoutGroup.childControlHeight = false;
            LayoutGroup.childControlWidth = false;
            LayoutGroup.childForceExpandWidth = false;
            LayoutGroup.childForceExpandHeight = false;

            LayoutGroup.childAlignment =
            scroll.horizontal
                ? TextAnchor.MiddleCenter
                : TextAnchor.UpperCenter;

            return scroll;
        }

        static void SetComponents(ScrollRect scroll, UIBluetprint.componentInfo[] components)
        {
            foreach (UIBluetprint.componentInfo componentInfo in components)
            {
                GameObject component = MakeComponent(scroll.content.gameObject, componentInfo);
                component.transform.SetParent(scroll.content.transform);
            }
        }

        static GameObject MakeComponent(GameObject target, UIBluetprint.componentInfo componentInfo)
        {
            GameObject component = new(componentInfo.Name);
            RectTransform componentRect = component.AddComponent<RectTransform>();
            SetBasicTransform(componentRect, new Vector2(100, 100), target.transform);

            switch (componentInfo.Type)
            {
                case UIBluetprint.componentInfo.ComponentType.Text:
                    Text text = component.AddComponent<Text>();
                    text.font = Options.font;
                    text.color = Options.textColor;
                    text.text = componentInfo.Path;
                    break;
                case UIBluetprint.componentInfo.ComponentType.Image:
                    Image image = component.AddComponent<Image>();
                    image.sprite = Resources.Load<Sprite>(componentInfo.Path);
                    break;
            }

            return component;
        }
    }
}
