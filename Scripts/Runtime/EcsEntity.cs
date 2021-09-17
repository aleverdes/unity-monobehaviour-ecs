using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AffenECS
{
    [DisallowMultipleComponent]
    public sealed class EcsEntity : MonoBehaviour
    {
        private readonly Dictionary<Type, EcsComponent> _components = new Dictionary<Type, EcsComponent>();

        private void Awake()
        {
            EcsWorld.Entities.Add(this);
        }

        private void OnDestroy()
        {
            EcsWorld.Entities.Remove(this);
        }

        #region Add components
        
        public T Add<T>() where T : EcsComponent
        {
            return (T) Add(typeof(T));
        }

        public EcsComponent Add(Type type)
        {
            if (!typeof(EcsComponent).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Failed on add component with type {type}: type should be assignable from EcsComponent");
            }

            return _components.ContainsKey(type) ? _components[type] : Add((EcsComponent) gameObject.AddComponent(type));
        }

        public EcsComponent Add(EcsComponent component)
        {
            _components[component.GetType()] = component;
            EcsWorld.AddComponent(this, component);
            return component;
        }
        
        #endregion

        #region Remove components

        public void Remove(EcsComponent component)
        {
            _components.Remove(component.GetType());
            EcsWorld.RemoveComponent(this, component);
            Destroy(component);
        }

        public void Remove<T>() where T : Type
        {
            if (_components.TryGetValue(typeof(T), out EcsComponent component))
            {
                _components.Remove(typeof(T));
                EcsWorld.RemoveComponent(this, component);
                Destroy(component);
            }
        }
        
        #endregion

        #region Has components

        public bool Has<T>() where T : Type
        {
            return Has(typeof(T));
        }

        public bool Has(Type type)
        {
            return _components.ContainsKey(type);
        }
        
        #endregion

        #region Get components

        public T Get<T>() where T : EcsComponent
        {
            return (T) _components[typeof(T)];
        }

        public EcsComponent Get(Type componentType)
        {
            return _components[componentType];
        }
        
        #endregion
    }
}