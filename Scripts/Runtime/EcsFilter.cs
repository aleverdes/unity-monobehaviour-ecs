using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AffenECS
{
    public sealed class EcsFilter : IEnumerable<EcsEntity>
    {
        private readonly HashSet<Type> _includeTypes = new HashSet<Type>();
        private readonly HashSet<Type> _excludeTypes = new HashSet<Type>();

        public EcsFilter Include<T>() where T : EcsComponent
        {
            _includeTypes.Add(typeof(T));
            return this;
        }

        public EcsFilter Include<T1, T2>() where T1 : EcsComponent where T2 : EcsComponent
        {
            _includeTypes.Add(typeof(T1));
            _includeTypes.Add(typeof(T2));
            return this;
        }

        public EcsFilter Include<T1, T2, T3>() where T1 : EcsComponent where T2 : EcsComponent where T3 : EcsComponent
        {
            _includeTypes.Add(typeof(T1));
            _includeTypes.Add(typeof(T2));
            _includeTypes.Add(typeof(T3));
            return this;
        }

        public EcsFilter Include<T1, T2, T3, T4>() where T1 : EcsComponent where T2 : EcsComponent where T3 : EcsComponent where T4 : EcsComponent
        {
            _includeTypes.Add(typeof(T1));
            _includeTypes.Add(typeof(T2));
            _includeTypes.Add(typeof(T3));
            _includeTypes.Add(typeof(T4));
            return this;
        }

        public EcsFilter Exclude<T>() where T : EcsComponent
        {
            _excludeTypes.Add(typeof(T));
            return this;
        }

        public EcsFilter Exclude<T1, T2>() where T1 : EcsComponent where T2 : EcsComponent
        {
            _excludeTypes.Add(typeof(T1));
            _excludeTypes.Add(typeof(T2));
            return this;
        }

        public EcsFilter Exclude<T1, T2, T3>() where T1 : EcsComponent where T2 : EcsComponent where T3 : EcsComponent
        {
            _excludeTypes.Add(typeof(T1));
            _excludeTypes.Add(typeof(T2));
            _excludeTypes.Add(typeof(T3));
            return this;
        }

        public EcsFilter Exclude<T1, T2, T3, T4>() where T1 : EcsComponent where T2 : EcsComponent where T3 : EcsComponent where T4 : EcsComponent
        {
            _excludeTypes.Add(typeof(T1));
            _excludeTypes.Add(typeof(T2));
            _excludeTypes.Add(typeof(T3));
            _excludeTypes.Add(typeof(T4));
            return this;
        }

        public IEnumerator<EcsEntity> GetEnumerator()
        {
            Type mainType = _includeTypes.First();
            int mainTypeCount = EcsWorld.ComponentTypeToComponentToEntity[mainType].Count;
            
            if (_includeTypes.Count > 1)
            {
                foreach (Type includeType in _includeTypes)
                {
                    int includeTypeCount = EcsWorld.ComponentTypeToComponentToEntity[includeType].Count;
                    if (includeTypeCount < mainTypeCount)
                    {
                        mainType = includeType;
                        mainTypeCount = includeTypeCount;
                    }
                }
            }
            
            return EcsWorld.ComponentTypeToEntityToComponent[mainType].Where(x =>
            {
                foreach (Type includeType in _includeTypes)
                {
                    if (!x.Key.Has(includeType))
                    {
                        return false;
                    }
                }

                foreach (Type excludeType in _excludeTypes)
                {
                    if (x.Key.Has(excludeType))
                    {
                        return false;
                    }
                }

                return true;
            }).Select(x => x.Key).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}