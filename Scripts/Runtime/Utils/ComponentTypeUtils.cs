using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AffenECS
{
    public static class ComponentTypeUtils
    {
        private static readonly Dictionary<Type, ushort> SerializedTypesByType = new Dictionary<Type, ushort>();
        private static readonly Dictionary<ushort, Type> SerializedTypesByIndex = new Dictionary<ushort, Type>();
        
        static ComponentTypeUtils()
        {
            var ecsComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(EcsComponent).IsAssignableFrom(x))
                .OrderBy(x => x.FullName);

            ushort index = 0;
            foreach (Type ecsComponentType in ecsComponentTypes)
            {
                SerializedTypesByType.Add(ecsComponentType, index);
                SerializedTypesByIndex.Add(index, ecsComponentType);
                index++;
            }
        }
        
        public static ushort SerializeType(this EcsComponent ecsComponent)
        {
            return SerializedTypesByType[ecsComponent.GetType()];
        }

        public static Type DeserializeType(this ushort ecsComponentSerializedTypeIndex)
        {
            return SerializedTypesByIndex[ecsComponentSerializedTypeIndex];
        }
    }
}