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
