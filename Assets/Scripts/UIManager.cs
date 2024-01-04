using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public static class UIManager
    {
        static List<ViewBase> views = new();

        public static void RegisterView(ViewBase view)
        {
            if (!views.Contains(view))
                views.Add(view);
        }

        public static void Refresh()
        {
            foreach (var view in views)
            {
                view.Refresh();
            }
        }
    }
}
