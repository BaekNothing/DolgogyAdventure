using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UISystem
{
    public enum ComponentStatus
    {
        Selected,
        Enable,
        Disable,
        Hide
    }

    public interface IViewData
    {
        public T GetData<T>() where T : class, IViewData;
    }
}
