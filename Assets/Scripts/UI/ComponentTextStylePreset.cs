using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dolgoji.UIComponent
{
    [CreateAssetMenu(fileName = "ComponentTextStylePreset", menuName = "UI/ComponentTextStylePreset")]
    class ComponentTextStylePreset : ScriptableObject
    {
        public Font Font;
        public int FontSize;
        public Color FontColor;
        public TextAnchor Alignment;
        public Direction ComponentDirection = Direction.Vertical;
    }
}
