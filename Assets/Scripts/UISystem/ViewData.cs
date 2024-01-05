using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UISystem
{

    public interface IViewData
    {
        public UIComponentAction RefreshActions { get; }
        public void AddRefreshAction<T>(Action action) where T : class;
        public void RemoveRefreshAction<T>(Action action) where T : class;
    }

    public abstract class ViewData : ScriptableObject, IViewData
    {
        public abstract void Initialized();

        public UIComponentAction RefreshActions { get; } = new();
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

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="targetName"></param>
        /// <exception cref="ArgumentException">  </exception>
        public void SetValue<T>(T value, string targetName) //where T : IEquatable<T>
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
