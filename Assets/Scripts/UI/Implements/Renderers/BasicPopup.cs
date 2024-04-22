using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Dolgoji.UI
{
    public class BasicPopup : UIRenderer
    {
        [SerializeField] Button _closeButton;

        [SerializeField] Text _titleText;
        [SerializeField] Text _contentText;

        public override void Initialize()
        {
            var model = UIUtility.GetUIModel<BasicPopupModel>();
            model.RegisterUIRenderer(this);
            Refresh(model);
        }

        public override void Refresh(IUIModel model)
        {
            if (model is BasicPopupModel basicPopupModel)
            {
                _titleText.text = basicPopupModel.Title;
                _contentText.text = basicPopupModel.Content;
            }
        }

        void Awake()
        {
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() => Hide());
        }
    }
}
