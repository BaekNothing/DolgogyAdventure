using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Dolgoji.UI
{
    [CreateAssetMenu(fileName = "BasicPopupModel", menuName = "Dolgiji/UI/Models/BasicPopupModel")]
    public class BasicPopupModel : UIModel
    {
        [SerializeField] private string _title;
        public string Title { get => _title; }
        [SerializeField] private string _content;
        public string Content { get => _content; }

        public override void SetModel(List<IData> datas) => new System.NotImplementedException();
    }
}
