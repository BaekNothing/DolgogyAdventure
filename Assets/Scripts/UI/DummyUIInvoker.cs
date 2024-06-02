using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dolgoji.UI;

namespace Dolgoji.UI
{
    public class DummyUIInvoker : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            UIUtility.Show<BasicPopup>();
        }
    }
}
