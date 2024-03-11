### ./Scripts\SystemRoot.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

#pragma warning disable IDE0044 // ignore warning about private readonly field

namespace CoreSystem
{
    public class SystemRoot : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static IDataContainor Data { get => _dataContainor; }
        public static IUIContainor UI { get => _uiContainor; }
        public static IInputContainor Input { get => _inputContainor; }

        static DataContainor _dataContainor;
        static UIContainor _uiContainor;
        static InputContainor _inputContainor;

        public static bool IsInitialized { get; private set; } = false;
        static GameObject Instance = null;

#if UNITY_EDITOR
        public ScriptableObject[] Containors;
#endif

        void Start()
        {
            if (!IsInitialized)
            {
                DontDestroyOnLoad(gameObject);
                InitAllContainor();
                Instance = gameObject;
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        void Update()
        {
            if (!IsInitialized) return;
            if (Instance != gameObject) return;

            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (UnityEngine.Input.GetKeyDown(keyCode))
                    _inputContainor?.Invoke(keyCode, IInputContainor.InputType.DownOnce);
                if (UnityEngine.Input.GetKey(keyCode))
                    _inputContainor?.Invoke(keyCode, IInputContainor.InputType.Hold);
            }
        }

        void InitAllContainor()
        {
            Utility.Logger.Log($"CoreSystem.LoadAllContainor Start");

            LoadAllContainor();

            _dataContainor.Initialized();
            _uiContainor.Initialized();
            _inputContainor.Initialized();

            IsInitialized = true;

        }

        void LoadAllContainor()
        {
            var containors = Resources.LoadAll<ScriptableObject>("CoreSystem") ??
                throw new System.NullReferenceException($"CoreSystem.LoadAllContainor: containors is null");

            foreach (var containor in containors)
            {
                if (containor is DataContainor dataContainor)
                    _dataContainor = dataContainor;
                else if (containor is UIContainor uIContainor)
                    _uiContainor = uIContainor;
                else if (containor is InputContainor inputContainor)
                    _inputContainor = inputContainor;
                else
                    continue;
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            Containors = new ScriptableObject[]
            {
                _dataContainor,
                _uiContainor
            };
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}

```
### ./Scripts\CoreSystem\DataContainor.cs
```csharp
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataObject;

namespace CoreSystem
{
    public interface IDataContainor
    {
        T GetData<T>() where T : AData;
    }

    [CreateAssetMenu(fileName = "DataContainor", menuName = "ScriptableObjects/DataContainor", order = 1)]
    public class DataContainor : ScriptableObject, IDataContainor
    {
        readonly Dictionary<string, AData> _dataDict = new();
#if UNITY_EDITOR
        [SerializeField] List<AData> _dataInspectorShower = new();
#endif

        const string _uiPath = "UI";

        public void Initialized()
        {
            LoadAllData();
        }

        public void LoadAllData()
        {
            var DirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/{_uiPath}") ??
                throw new NullReferenceException($"UIContainor.Initialized: {_uiPath} not found");

            _dataDict.Clear();
#if UNITY_EDITOR
            _dataInspectorShower.Clear();
#endif
            var dirPaths = DirInfo.GetDirectories().Select(dir => dir.Name).ToArray();
            foreach (var dirPath in dirPaths)
            {
                var dataObjects = LoadAllDataObjects(dirPath);
                foreach (var dataObject in dataObjects)
                {
                    var objName = dataObject.name;
                    if (_dataDict.ContainsKey(objName))
                    {
                        Debug.LogError($"UIContainor.Initialized: UI {objName} already exist");
                        continue;
                    }
                    _dataDict.Add(objName, dataObject);
#if UNITY_EDITOR
                    _dataInspectorShower.Add(dataObject);
#endif
                }
            }
        }

        AData[] LoadAllDataObjects(string dirPath)
        {
            var data = Resources.LoadAll<AData>($"{_uiPath}/{dirPath}");
            return data;
        }

        public T GetData<T>() where T : AData
        {
            return this.Get<T>();
        }

        T Get<T>() where T : AData
        {
            var type = typeof(T);
            if (_dataDict.ContainsKey(type.Name))
            {
                return _dataDict[type.Name] as T;
            }
            else
            {
                Debug.LogError($"DataContainor.Get: {type.Name} not found");
                return null;
            }
        }
    }
}

```
### ./Scripts\CoreSystem\InputContainor.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

namespace CoreSystem
{
    [Serializable]
    struct InputActionData
    {
        public KeyCode KeyCode;
        public List<Action> actions;

        public InputActionData(KeyCode keyCode)
        {
            this.KeyCode = keyCode;
            this.actions = new List<Action>();
        }

        public readonly void SetAction(Action action)
        {
            if (!actions.Contains(action))
                actions.Add(action);
        }

        public readonly void Invoke()
        {
            actions.ForEach((action) => action?.Invoke());
        }
    }

    public interface IInputContainor
    {
        public enum InputType
        {
            Hold,
            DownOnce
        }

        public void SetAction(InputType tytpe, KeyCode key, Action action);
    }

    [CreateAssetMenu(fileName = "InputContainor", menuName = "ScriptableObjects/InputContainor", order = 1)]
    public class InputContainor : ScriptableObject, IInputContainor
    {

        readonly Dictionary<IInputContainor.InputType, Dictionary<KeyCode, InputActionData>> _actionDatas
        = new()
        {
            {IInputContainor.InputType.Hold, new Dictionary<KeyCode, InputActionData>()},
            {IInputContainor.InputType.DownOnce, new Dictionary<KeyCode, InputActionData>()},
        };

        public void Initialized()
        {
            SetAction(IInputContainor.InputType.DownOnce, KeyCode.BackQuote, () =>
            {

                if (QuantumConsole.Instance.IsActive)
                    QuantumConsole.Instance.Deactivate();
                else
                    QuantumConsole.Instance.Activate();
            });
        }

        public void Invoke(KeyCode key, IInputContainor.InputType type)
        {
            if (_actionDatas[type].ContainsKey(key))
                _actionDatas[type][key].Invoke();
        }

        public void SetAction(IInputContainor.InputType type, KeyCode key, Action action)
        {
            if (!_actionDatas[type].ContainsKey(key))
            {
                var actionData = new InputActionData(key);
                _actionDatas[type].Add(key, actionData);
            }

            _actionDatas[type][key].SetAction(action);
        }
    }
}

```
### ./Scripts\CoreSystem\UIContainor.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIObject;

namespace CoreSystem
{
    public interface IUIContainor : IUIViewStackContainor
    {

    }

    [CreateAssetMenu(fileName = "UIContainor", menuName = "ScriptableObjects/UIContainor", order = 1)]
    public class UIContainor : ScriptableObject, IUIContainor
    {
        [SerializeField] UIViewStackContainor _uiViewStackContainor = new();

        public void Initialized()
        {
            _uiViewStackContainor.Initialized();
            Focus<RootMenuView>();
        }

        public void Focus<T>() where T : ViewBase
        {
            _uiViewStackContainor.Focus<T>();
        }

        public void Pop<T>() where T : ViewBase
        {
            _uiViewStackContainor.Pop<T>();
        }

        public ViewBase Pick()
        {
            return _uiViewStackContainor.Pick();
        }
    }
}

```
### ./Scripts\CoreSystem\UIPrefabContainor.cs
```csharp
Ôªøusing System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CoreSystem
{
    public interface IUIPrefabContainor
    {
        GameObject GetUIPrefab(string name);
    }

    [Serializable]
    public class UIPrefabContainor : IUIPrefabContainor
    {
        Dictionary<string, GameObject> _uiPrefabs = new();
        const string _uiPath = "UI";

        public void Initialized()
        {
            var DirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/{_uiPath}") ??
                throw new NullReferenceException($"UIContainor.Initialized: {_uiPath} not found");

            _uiPrefabs.Clear();
            var dirPaths = DirInfo.GetDirectories().Select(dir => dir.Name).ToArray();
            foreach (var dirPath in dirPaths)
            {
                var uiPrefabs = LoadAllUIPrefabs(dirPath);
                foreach (var uiPrefab in uiPrefabs)
                {
                    var uiName = uiPrefab.name;
                    if (_uiPrefabs.ContainsKey(uiName))
                    {
                        Debug.LogError($"UIContainor.Initialized: UI {uiName} already exist");
                        continue;
                    }
                    _uiPrefabs.Add(uiName, uiPrefab);
                }
            }
        }

        GameObject[] LoadAllUIPrefabs(string dirPath)
        {
            var uiPrefabs = Resources.LoadAll<GameObject>($"{_uiPath}/{dirPath}");
            uiPrefabs = Array.FindAll(uiPrefabs, uiPrefab => uiPrefab.GetComponent<UIObject.ViewBase>() != null);
            return uiPrefabs;
        }

        public GameObject GetUIPrefab(string name)
        {
            if (!_uiPrefabs.ContainsKey(name))
            {
                Debug.LogError($"UIContainor.GetUIPrefab: UI {name} not found");
                return null;
            }

            return _uiPrefabs[name];
        }
    }
}

```
### ./Scripts\CoreSystem\UIViewStackContainor.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UIObject;

namespace CoreSystem
{
    public interface IUIViewStackContainor
    {
        public void Focus<T>() where T : ViewBase;
        public void Pop<T>() where T : ViewBase;
        public ViewBase Pick();
    }

    [Serializable]
    public class UIViewStackContainor : IUIViewStackContainor
    {
        readonly UIPrefabContainor _uiContainor = new();
        const string _uiCanvasName = "UICanvas";

        public List<ViewBase> _viewStack = new();
        static readonly Dictionary<Type, ViewBase> _cashedView = new();

        public string FocusViewName;

        public void Initialized()
        {
            _viewStack.Clear();
            _cashedView.Clear();
            _uiContainor.Initialized();
        }

        public void Focus<T>() where T : ViewBase
        {
            Utility.Logger.Log($"UIViewStackContainor.Focus: {typeof(T).Name}");
            var view = GetView<T>();
            if (view != null)
            {
                while (PickView() != view)
                {
                    PopView();
                }

                view.OnResume();
                view.SetTop();
                view.OnEnter();
            }
            else
            {
                view = LoadView<T>();
                if (view == null)
                {
                    Debug.LogError($"View {typeof(T).Name} not found");
                    return;
                }
                PushView(view);
            }

            FocusViewName = view.GetType().Name;
        }

        public void Pop<T>() where T : ViewBase
        {
            Utility.Logger.Log($"UIViewStackContainor.Pop: {typeof(T).Name}");
            var topView = PickView();
            if (topView != null && topView.GetType() == typeof(T))
            {
                PopView();
            }
            else
            {
                Debug.LogError($"View {typeof(T).Name} not found");
            }
        }

        public ViewBase Pick()
        {
            return PickView();
        }

        T GetView<T>() where T : ViewBase
        {
            if (_viewStack.Count > 0)
            {
                var view = _viewStack.Find(v => v.GetType() == typeof(T));
                if (view != null)
                {
                    return view as T;
                }
            }
            return null;
        }

        T LoadView<T>() where T : ViewBase
        {
            if (_cashedView.ContainsKey(typeof(T)))
            {
                return _cashedView[typeof(T)] as T;
            }
            else
            {
                var prefab = _uiContainor.GetUIPrefab(typeof(T).Name) ??
                throw new System.NullReferenceException($"View {typeof(T).Name} not found");

                var targetCanvas = GetUICanvas() ??
                    throw new System.NullReferenceException($"Canvas not found");

                var view = UnityEngine.Object.Instantiate(prefab, targetCanvas.transform).GetComponent<T>() ??
                    throw new System.NullReferenceException($"View {typeof(T).Name} not found");

                view.Initialized();
                _cashedView.Add(typeof(T), view);
                return view;
            }
        }

        public GameObject GetUICanvas()
        {
            var canvas = GameObject.FindWithTag(_uiCanvasName) ??
                throw new NullReferenceException($"UIContainor.GetUICanvas: canvas {_uiCanvasName} not found");

            return canvas;
        }


        void PushView(ViewBase view)
        {
            if (_viewStack.Contains(view))
            {
                Debug.LogError("View already in stack");
                return;
            }

            if (_viewStack.Count > 0)
            {
                _viewStack[^1].OnPause();
            }

            _viewStack.Add(view);
            view.OnResume();
            view.SetTop();
            view.OnEnter();
        }

        void PopView()
        {
            if (_viewStack.Count == 0)
            {
                Debug.LogError("View stack is empty");
                return;
            }

            var view = _viewStack[^1];
            view.OnPause();
            view.OnExit();
            _viewStack.RemoveAt(_viewStack.Count - 1);

            if (_viewStack.Count > 0)
            {
                _viewStack[^1].OnResume();
            }
        }

        ViewBase PickView()
        {
            if (_viewStack.Count > 0)
            {
                return _viewStack[^1];
            }
            return null;
        }
    }
}

```
### ./Scripts\DataSystem\AData.cs
```csharp
Ôªøusing System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DataObject
{
    public interface IData
    {
        public void SetValue<T>(T value, string targetName);
    }

    [Serializable]
    public abstract class AData : ScriptableObject, IData, UIObject.IViewData
    {
        public abstract void Initialized();

        public ComponentAction RefreshActions { get; } = new();
        public void AddRefreshAction<T>(Action action) where T : class
        {
            if (action == null)
                throw new ArgumentNullException($"ViewBase.InitComponent: component is null");

            RefreshActions.SetAction<T>(action);
        }

        public void RemoveRefreshAction<T>(Action action) where T : class
        {
            if (action == null)
                throw new ArgumentNullException($"ViewBase.InitComponent: component is null");

            RefreshActions.RemoveAction<T>(action);
        }

        public void SetValue<T>(T value, string targetName)
        {
            var propertyInfo = GetPropertyInfo(targetName);
            var fieldInfo = GetFieldInfo(targetName);

            if (propertyInfo != null)
                SetAsProperty(value, propertyInfo);
            else if (fieldInfo != null)
                SetAsField(value, fieldInfo);
            else
                throw new ArgumentException($"ViewData.SetValue: {targetName} is not found");

            Refresh();
        }

        PropertyInfo GetPropertyInfo(string propertyName)
        {
            return GetType()?.GetProperty(propertyName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        FieldInfo GetFieldInfo(string fieldName)
        {
            return GetType()?.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        void SetAsProperty<T>(T value, PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetValue(this) is not T targetProperty)
                throw new ArgumentException($"ViewData.SetValue: {propertyInfo.Name} is not found");

            if (EqualityComparer<T>.Default.Equals(targetProperty, value))
                return;

            propertyInfo.SetValue(this, value);
        }

        void SetAsField<T>(T value, FieldInfo fieldInfo)
        {
            if (fieldInfo.GetValue(this) is not T targetField)
                throw new ArgumentException($"ViewData.SetValue: {fieldInfo.Name} is not found");

            if (EqualityComparer<T>.Default.Equals(targetField, value))
                return;

            fieldInfo.SetValue(this, value);
        }

        void Refresh()
        {
            if (!this)
                return;

            if (RefreshActions == null)
                throw new Exception($"{this.name} ViewBase.Refresh: page is not initialized");

            RefreshActions.Invoke();
        }
    }
}

```
### ./Scripts\DetailedImplementation\Scene\SampleScene\SampleDialogueObject.cs
```csharp
Ôªøusing System;
using UnityEngine;
using ObjectSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class SampleDialogueObject : ASceneObjectBase
{
    [SerializeField] string[] DialogueList;
    ClickableComponent _clickableComponent;

    public override void Initialize()
    {
        _clickableComponent = gameObject.AddComponent<ClickableComponent>();
        _clickableComponent.SetAction(ClickAction);
    }

    public void ClickAction()
    {
        Debug.Log("click_Down");
        if (DialogueList.Length == 0) return;
        CoreSystem.SystemRoot.Data.GetData<DialogueViewData>().SetDialogueTexts(DialogueList);
    }
}

```
### ./Scripts\DetailedImplementation\UI\Dialogue\DialogueView.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UIObject;

#pragma warning disable IDE0044 // ignore warning about private readonly field

public class DialogueView : ViewBase
{
    DialogueViewData DialogueViewData => CoreSystem.SystemRoot.Data.GetData<DialogueViewData>();

    [SerializeField] UIComponent _dialogueText;
    [SerializeField] UIComponent _dialogueButton;

    [SerializeField] TypewriteController _typewriteController;

    public override void Initialized()
    {
        SetDialogueText();
        SetDialogueButton();
    }

    void SetDialogueText()
    {
        UIComponent.UIComponentInitData _dialogueTextInitData = new(
            typeof(TextMeshProUGUI), DialogueViewData,
            ComponentUtility.EvaluatorImmutable,
            DrawDialogueText,
            this
        );

        InitComponent(_dialogueText, _dialogueTextInitData);
    }

    UIBehaviour DrawDialogueText(UIBehaviour body, IViewData data, ComponentStatus status)
    {
        ((TextMeshProUGUI)body).text = ((DialogueViewData)data).CurrentDialogueText;
        return body;
    }

    void SetDialogueButton()
    {
        UIComponent.UIComponentInitData _dialogueButtonInitData = new(
            typeof(Image), DialogueViewData,
            ComponentUtility.EvaluatorImmutable,
            ComponentUtility.DrawImmutable,
            this
        );

        InitComponent(_dialogueButton, _dialogueButtonInitData);

        _dialogueButton.SetAction<DialogueView>(DialogueAction);
    }

    void DialogueAction()
    {
        if (_typewriteController.IsTyping)
        {
            _typewriteController.SkipWriter();
        }
        else
        {
            DialogueViewData.Next();
            if (!DialogueViewData.IsDialogueEnd)
                _typewriteController.StartWriter();
        }
    }
}

```
### ./Scripts\DetailedImplementation\UI\Dialogue\DialogueViewData.cs
```csharp
using UnityEngine;
using DataObject;

[CreateAssetMenu(fileName = "DialogueViewData", menuName = "ScriptableObjects/DialogueViewData", order = 1)]
public class DialogueViewData : AData
{
    [SerializeField] int _dialogueIndex = 0;
    public int DialogueIndex { get => _dialogueIndex; private set => _dialogueIndex = value; }

    public string CurrentDialogueText => DialogueIndex < DialogueTexts.Length ? DialogueTexts[DialogueIndex] : "";
    public int DialogueLength => DialogueTexts.Length;
    public bool IsDialogueEnd => DialogueIndex >= DialogueLength;

    [SerializeField] string[] _dialogueTexts = new string[] { "" };
    public string[] DialogueTexts { get => _dialogueTexts; private set => _dialogueTexts = value; }

    public override void Initialized()
    {

    }

    public void Next()
    {
        SetIndex(DialogueIndex + 1);
        if (IsDialogueEnd)
            CoreSystem.SystemRoot.UI.Pop<DialogueView>();
    }

    public void Previous()
    {
        int index = DialogueIndex - 1;
        if (index < 0)
            index = 0;
        SetIndex(index);
    }

    void SetIndex(int index)
    {
        SetValue(index, nameof(DialogueIndex));
    }

    public void SetDialogueTexts(string[] texts)
    {
        CoreSystem.SystemRoot.UI.Focus<DialogueView>();
        SetValue(texts, nameof(DialogueTexts));
        SetIndex(0);
    }
}

```
### ./Scripts\DetailedImplementation\UI\RootMenu\RootMenuView.cs
```csharp
Ôªøusing System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UIObject;

#pragma warning disable IDE0044 // ignore warning about private readonly field

public class RootMenuView : ViewBase
{
    RootMenuViewData RootMenuData => CoreSystem.SystemRoot.Data.GetData<RootMenuViewData>();

    [SerializeField] UIComponent _debugButton;

    public override void Initialized()
    {
        SetDebugButton();
        Utility.Logger.Log($"{this.gameObject.name} RootMenuView.Initialized");
    }

    void SetDebugButton()
    {
        UIComponent.UIComponentInitData _debugButtonInitData =
        new(
            typeof(Button), RootMenuData,
            ComponentUtility.EvaluatorImmutable,
            ComponentUtility.DrawWithStatusOnly,
            this
        );

        InitComponent(_debugButton, _debugButtonInitData);
        _debugButton.SetAction<RootMenuView>(_debugButtonAction);
    }

    void _debugButtonAction()
    {
        Utility.Logger.Log
            ($"{this.gameObject.name} RootMenuView.SetDebugButton: debug button is clicked",
            Utility.Logger.Importance.Warning);

        CoreSystem.SystemRoot.Data.GetData<DialogueViewData>().SetDialogueTexts(
            new string[] { $"Debug Button is Clicked {System.DateTime.Now}" }
        );
    }

}

```
### ./Scripts\DetailedImplementation\UI\RootMenu\RootMenuViewData.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataObject;

[CreateAssetMenu(fileName = "RootMenuViewData", menuName = "ScriptableObjects/RootMenuViewData", order = 1)]
public class RootMenuViewData : AData
{
    public override void Initialized()
    {
        Utility.Logger.Log($"RootMenuViewData.Initialized");
    }
}

```
### ./Scripts\ObjectSystem\ASceneObjectBase.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ObjectSystem
{
    public abstract class ASceneObjectBase : MonoBehaviour
    {
        public abstract void Initialize();
    }
}

```
### ./Scripts\ObjectSystem\ClickableComponent.cs
```csharp
Ôªøusing System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ObjectSystem
{
    public class ClickableComponent : MonoBehaviour
    {
        Action _action;

        public void SetAction(Action action)
        {
            _action = action;
        }

        public void OnMouseDown()
        {
            _action?.Invoke();
        }
    }
}

```
### ./Scripts\SceneSystem\SceneData.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ObjectSystem;
using Cysharp.Threading.Tasks;

namespace SceneSystem
{
    public class SceneData : MonoBehaviour
    {
        [SerializeField] List<ASceneObjectBase> _sceneObjects;

        void Start()
        {
            Initialize();
        }

        async void Initialize()
        {
            await UniTask.WaitUntil(() => CoreSystem.SystemRoot.IsInitialized);
            _sceneObjects = Utility.ChildFinder.GetAll<ASceneObjectBase>(transform);
            _sceneObjects.ForEach((sceneObject) => sceneObject.Initialize());
        }


    }
}

```
### ./Scripts\UISystem\IViewData.cs
```csharp
Ôªøusing System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UIObject
{
    public interface IViewData
    {
        public ComponentAction RefreshActions { get; }
        public void AddRefreshAction<T>(Action action) where T : class;
        public void RemoveRefreshAction<T>(Action action) where T : class;
    }
}

```
### ./Scripts\UISystem\TypewriteController.cs
```csharp
Ôªøusing System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;

namespace UIObject
{
    [Serializable]
    public class TypewriteController
    {
        [SerializeField] private TypewriterByCharacter Typewriter;
        public bool IsTyping => Typewriter.isShowingText;

        public void StartWriter()
        {
            Typewriter.StartShowingText();
        }

        public void SkipWriter()
        {
            Typewriter.SkipTypewriter();
        }
    }
}

```
### ./Scripts\UISystem\UIComponent.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UIObject
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIBehaviour))]
    public class UIComponent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        public struct UIComponentInitData
        {
            public Type type;
            public IViewData data;
            public Func<IViewData, ComponentStatus, ComponentStatus> evaluator;
            public Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> draw;
            public ViewBase parent;

            public UIComponentInitData(Type type, IViewData data, Func<IViewData, ComponentStatus, ComponentStatus> evaluator, Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> draw, ViewBase parent)
            {
                if (type != typeof(TextMeshProUGUI) && type != typeof(Text) &&
                    type != typeof(Image) && type != typeof(Button))
                    throw new ArgumentException($"UIComponent.Init: type is not Text, Image or Button");

                if (data == null || evaluator == null || draw == null)
                    throw new ArgumentNullException($"UIComponent.Init: argument is null");

                this.type = type;
                this.data = data;
                this.evaluator = evaluator;
                this.draw = draw;
                this.parent = parent;
            }
        }


        public ComponentStatus Status = ComponentStatus.Enable;
        [SerializeField] UIBehaviour ComponentBody;
        [SerializeField] ComponentAction Action = new();

        IViewData _viewData;
        ViewBase _parent = null;

        Func<IViewData, ComponentStatus, ComponentStatus> Evaluator { get; set; } = (data, status) =>
            throw new NotImplementedException("Evaluator is not implemented");
        Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> Draw { get; set; } = (body, data, status) =>
            throw new NotImplementedException($"Draw is not implemented");

        public void Refresh()
        {
            if (!this) return;

            if (ComponentBody == null || _viewData == null || Evaluator == null || Draw == null)
                throw new NullReferenceException($"{this.gameObject.name} UIComponent.Refresh: component is not initialized");

            Status = Evaluator(_viewData, Status);
            Draw(ComponentBody, _viewData, Status);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_parent == null ||
                !_parent.IsTop() ||
                _parent.IsPaused)
                return;

            Utility.Logger.Log($"{_parent?.IsTop()} {gameObject.name} UIComponent.OnPointerClick: {eventData.button} button is clicked");

            Action?.Invoke();
        }

        public void Initialized(UIComponentInitData initData)
        {
            var body = transform.GetComponent(initData.type) as UIBehaviour;

            if (!body)
                throw new NullReferenceException($"{this.gameObject.name} UIComponent.Init: body is null");

            SetBody(body);
            SetParent(initData.parent);
            SetData(initData.data);
            SetEvaluator(initData.evaluator);
            SetDraw(initData.draw);
        }

        public void SetBody(UIBehaviour body)
        {
            Debug.Assert(body != null);
            ComponentBody = body;
        }

        public void SetParent(ViewBase parent)
        {
            Debug.Assert(parent != null);
            _parent = parent;
        }

        public void SetData(IViewData data)
        {
            Debug.Assert(data != null);

            if (_viewData != null && _viewData != data)
                _viewData.RemoveRefreshAction<UIComponent>(Refresh);
            else if (_viewData == data)
                return;

            data.AddRefreshAction<UIComponent>(Refresh);
            _viewData = data;
        }

        public void SetEvaluator(Func<IViewData, ComponentStatus, ComponentStatus> evaluator)
        {
            Debug.Assert(evaluator != null);
            Evaluator = evaluator;
        }

        public void SetDraw(Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> draw)
        {
            Debug.Assert(draw != null);
            Draw = draw;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"> ï¥ãπ ï°ÖòùÑ †ïùòïú Å¥ûòä§ùò Ï∂úÏ≤ò </typeparam>
        /// <param name="action"></param>
        public void SetAction<T>(Action action) where T : class
        {
            Debug.Assert(action != null);
            Action.SetAction<T>(action);
        }
    }
}

```
### ./Scripts\UISystem\UIConstants.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIObject
{
    public enum ComponentStatus
    {
        Enable,
        Disable,
    }

    public static class ComponentUtility
    {
        public static ComponentStatus EvaluatorImmutable(IViewData data, ComponentStatus status) => status;
        public static UIBehaviour DrawImmutable(UIBehaviour body, IViewData data, ComponentStatus status) => body;
        public static UIBehaviour DrawWithStatusOnly(UIBehaviour body, IViewData data, ComponentStatus status)
        {
            body.gameObject.SetActive(status == ComponentStatus.Enable);
            return body;
        }
    }
}

```
### ./Scripts\UISystem\ViewBase.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIObject
{
    public abstract class ViewBase : MonoBehaviour
    {
        public const string TargetCanvasName = "UICanvas";

        public bool IsPaused { get; private set; } = false;
        public bool IsInitialized { get; private set; } = false;

        public abstract void Initialized();

        protected void InitComponent(UIComponent component, UIComponent.UIComponentInitData data)
        {
            if (component == null)
                throw new System.ArgumentNullException($"{this.gameObject.name} ViewBase.InitComponent: component is null");

            component.Initialized(data);
        }

        public bool IsTop()
        {
            return CoreSystem.SystemRoot.UI.Pick() == this;
        }

        public void SetTop()
        {
            this.transform.SetAsLastSibling();
        }

        public virtual void OnEnter()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void OnExit()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void OnPause()
        {
            IsPaused = true;
        }

        public virtual void OnResume()
        {
            IsPaused = false;
        }
    }
}

```
### ./Scripts\Utility\ChildFinder.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ChildFinder
    {
        public static List<T> GetAll<T>(Transform parent) where T : Component
        {
            List<T> result = new();

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                var component = child.GetComponent<T>();
                if (component != null)
                    result.Add(component);
                if (child.childCount > 0)
                    result.AddRange(GetAll<T>(child));
            }

            return result;
        }
    }
}

```
### ./Scripts\Utility\ComponentAction.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ComponentAction : ISerializationCallbackReceiver
{
    public SortedList<string, Action> Actions { get; private set; } = new();
    public string ActionsNames;

    public void Invoke()
    {
        Debug.Assert(Actions != null);
        foreach (var action in Actions)
            action.Value?.Invoke();
    }

    public void Invoke<T>() where T : class
    {
        Debug.Assert(Actions != null);
        Debug.Assert(Actions.ContainsKey(typeof(T).Name));
        Actions[typeof(T).Name]?.Invoke();
    }

    public void SetAction<T>(Action action) where T : class
    {
        Debug.Assert(action != null);
        Actions ??= new SortedList<string, Action>();

        string typeName = typeof(T).Name;

        if (Actions.ContainsKey(typeName))
        {
            // Prevent duplicate actions
            Actions[typeName] -= action;
            Actions[typeName] += action;
        }
        else
            Actions.Add(typeName, action);
    }

    public void RemoveAction<T>(Action action) where T : class
    {
        Debug.Assert(action != null);
        Debug.Assert(Actions != null);

        string typeName = typeof(T).Name;

        if (Actions.ContainsKey(typeName))
            Actions[typeName] -= action;
    }

    public void OnBeforeSerialize()
    {
        ActionsNames = string.Join("\n", Actions.Select(x => x.Key + " : " + x.Value.Method.Name));
    }

    public void OnAfterDeserialize()
    {

    }
}

```
### ./Scripts\Utility\Logger.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class Logger
    {
        public enum Importance
        {
            Info,
            Warning,
            Error
        }

        public static Color[] ImportanceColors = new Color[]
        {
            Color.white,
            Color.yellow,
            Color.red
        };

        public static void Log(string message, Importance importance = Importance.Info)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(ImportanceColors[(int)importance])}>{message}</color>");
#endif
        }
    }
}

```
