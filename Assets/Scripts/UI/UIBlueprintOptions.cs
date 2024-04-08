using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dolgoji.UIComponent
{
    [CreateAssetMenu(fileName = "UIBlueprintOptions", menuName = "UI/BlueprintOptions")]
    public class UIBlueprintOptions : ScriptableObject
    {
        [SerializeField] Image[] bgImages;
        public Image BgImage
        {
            get
            {
                if (bgImages.Length > 0)
                    return bgImages[UnityEngine.Random.Range(0, bgImages.Length)];
                else    // if there is no image, return null
                    return null;
            }
        }

        public RectOffset pedding = new RectOffset();
        public int spacing = 10;
        public int fontSize = 14;
        public Color fontColor = Color.black;
        public Color backgroundColor = Color.white;
        public Color textColor = Color.black;
        public Font font;
    }
}
