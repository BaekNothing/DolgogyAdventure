using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dolgoji.UI
{
    public abstract class UIRenderer : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public abstract void Initialize();
        public abstract void Refresh(IUIModel model);
    }
}
