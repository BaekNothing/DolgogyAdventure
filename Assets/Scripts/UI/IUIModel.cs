using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dolgoji.UI
{
    public interface IUIModel
    {
        public void SetModel(List<IData> datas);
        public void Refresh();
        public void RegisterUIRenderer(UIRenderer uiRenderer);
        public void RemoveUIRenderer(UIRenderer uiRenderer);
    }

    public abstract class UIModel : ScriptableObject, IUIModel
    {
        List<UIRenderer> _uiRendererList = new();

        public abstract void SetModel(List<IData> datas);

        public void Refresh()
        {
            foreach (var uiRenderer in _uiRendererList)
            {
                uiRenderer.Refresh(this);
            }
        }

        public void RegisterUIRenderer(UIRenderer uiRenderer)
        {
            if (uiRenderer != null && !_uiRendererList.Contains(uiRenderer))
                _uiRendererList.Add(uiRenderer);
        }

        public void RemoveUIRenderer(UIRenderer uiRenderer)
        {
            if (uiRenderer != null && _uiRendererList.Contains(uiRenderer))
                _uiRendererList.Remove(uiRenderer);
        }
    }
}
