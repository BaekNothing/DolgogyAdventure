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
