using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dolgoji.UIComponent
{
    public class UIRoot : MonoBehaviour
    {
        public Canvas uiCanvas;
        public Camera uiCamera;

        public void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        public void Start()
        {
            UIBuilder.Build("UI/Blueprints/Test", uiCanvas.transform);
        }
    }
}
