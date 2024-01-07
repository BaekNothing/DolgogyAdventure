using UnityEngine;

namespace CoreSystem
{
    public interface IDataContainor
    {
        DialogueData DialogueData { get; }
    }

    [CreateAssetMenu(fileName = "DataContainor", menuName = "ScriptableObjects/DataContainor", order = 1)]
    public class DataContainor : ScriptableObject, IDataContainor
    {
        public static bool IsInitialized { get; private set; } = false;

        [SerializeField] DialogueData _dialogueData;
        public DialogueData DialogueData { get => _dialogueData; }

        public void Initialized()
        {
            DialogueData.Initialized();
            IsInitialized = true;
        }
    }
}
