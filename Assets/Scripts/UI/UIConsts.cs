using UnityEngine.UI;
using UnityEngine;

namespace Dolgoji.UIComponent
{
    public static class UIConsts
    {
        public const string UI_PREFAB_PATH = "UI/Prefabs/";
        public const string UI_RootCanvas = "UIRootCanvas";
        public const string UI_RootCamera = "UIRootCamera";
        public const string UI_Layer = "UI";

        static Camera _uiRootCamera;
        public static Camera UIRootCamera
        {
            get
            {
                if (_uiRootCamera == null)
                {
                    GameObject go = GameObject.Find(UI_RootCamera);
                    if (go == null)
                    {
                        go = new GameObject(UI_RootCamera, typeof(Camera))
                        {
                            layer = LayerMask.NameToLayer(UI_Layer)
                        };
                    }
                    _uiRootCamera = go.GetComponent<Camera>();
                    _uiRootCamera.clearFlags = CameraClearFlags.Depth;
                    _uiRootCamera.cullingMask = 1 << LayerMask.NameToLayer(UI_Layer);
                    _uiRootCamera.orthographic = true;
                    _uiRootCamera.nearClipPlane = -10;
                    _uiRootCamera.farClipPlane = 10;
                    _uiRootCamera.depth = 0;
                    _uiRootCamera.allowHDR = false;
                    _uiRootCamera.allowMSAA = false;
                    _uiRootCamera.allowDynamicResolution = false;
                    _uiRootCamera.useOcclusionCulling = false;
                }

                return _uiRootCamera;
            }
        }

        static Canvas _uiRootCanvas;
        public static Canvas UIRootCanvas
        {
            get
            {
                if (_uiRootCanvas == null)
                {
                    GameObject go = new(UI_RootCanvas,
                        typeof(Canvas),
                        typeof(CanvasScaler),
                        typeof(GraphicRaycaster))
                    {
                        layer = LayerMask.NameToLayer(UI_Layer)
                    };

                    _uiRootCanvas = go.GetComponent<Canvas>();
                    _uiRootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    _uiRootCanvas.worldCamera = UIRootCamera;

                    CanvasScaler canvasScaler = go.GetComponent<CanvasScaler>();
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = new Vector2(1920, 1080);
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 0f;

                    Object.DontDestroyOnLoad(go);
                }

                return _uiRootCanvas;
            }
        }
    }
}
