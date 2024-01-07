using System;
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
