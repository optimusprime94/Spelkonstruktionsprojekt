﻿using System;
using System.Collections.Generic;
using Spelkonstruktionsprojekt.ZEngine.Components;
using Spelkonstruktionsprojekt.ZEngine.Managers;
using ZEngine.Components;

namespace Spelkonstruktionsprojekt.ZEngine.Helpers
{
    public class ComponentFactory
    {
        private readonly Dictionary<Type, Stack<IComponent>> _scrapyard = new Dictionary<Type, Stack<IComponent>>();

        public void Recycle<T>(IComponent component)
        {
            if (component.GetType() == typeof(T))
            {
                _scrapyard[typeof(T)].Push(component);
            }
        }

        public void Recycle(Type componentType, IComponent component)
        {
            if (component.GetType() == componentType)
            {
                _scrapyard[componentType].Push(component);
            }
        }


        public T NewComponent<T>() where T : IComponent, new()
        {
            Stack<IComponent> components;
            var status = _scrapyard.TryGetValue(typeof(T), out components);
            if (status && components.Count >= 1)
            {
                return (T) components.Pop().Reset();
            }

            var newComponent = new T();
            if (!status)
            {
                var newStack = new Stack<IComponent>();
                _scrapyard[typeof(T)] = newStack;
            }
            return newComponent;
        }
    }
}