using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dolgoji.UIComponent
{
    /// <summary>
    /// This class is a blueprint for creating a new UI component.
    /// blueprint is made with ScrollView and components
    /// background is a ScrollView, components are Text, Image
    /// UIBuilder will create a ScrollView and components based on this blueprint
    /// Components are will be created as children of ScrollView ordered by the order of the array
    /// </summary>
    [CreateAssetMenu(fileName = "NewUIBlueprint", menuName = "UI/Blueprint")]
    class UIBluetprint : ScriptableObject
    {
        public string Name = "";
        public scrollInfo ScrollInfo;
        public componentInfo[] Components;

        [Serializable]
        public class scrollInfo
        {
            public enum Direction
            {
                Vertical,
                Horizontal
            }
            public Vector3 Position = new Vector3(0, 0, 0);
            public Vector2 Size = new Vector2(0, 0);
            public Vector2 Padding = new Vector2(10, 10);
            public Direction direction = Direction.Vertical;
        }

        [Serializable]
        public class componentInfo
        {
            public enum ComponentType
            {
                None,
                Text,
                Image
            }
            public string Name = "";
            public ComponentType Type = ComponentType.None;
            public string Path = "";
            public bool IsRaycastTarget = false;
        }
    }
}
