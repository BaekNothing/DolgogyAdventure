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
    class UIBlueprint : ScriptableObject
    {
        public string Name = "";
        public UIBlueprintGroups[] ComponentGroups;
    }
}
