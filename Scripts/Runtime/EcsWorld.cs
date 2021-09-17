using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AffenECS
{
    public class EcsWorld
    {
        public static readonly HashSet<EcsEntity> Entities = new HashSet<EcsEntity>();
        public static readonly Dictionary<Type, Dictionary<EcsEntity, EcsComponent>> ComponentTypeToEntityToComponent = new Dictionary<Type, Dictionary<EcsEntity, EcsComponent>>();
        public static readonly Dictionary<Type, Dictionary<EcsComponent, EcsEntity>> ComponentTypeToComponentToEntity = new Dictionary<Type, Dictionary<EcsComponent, EcsEntity>>();
        public static readonly Dictionary<EcsEntity, Dictionary<Type, EcsComponent>> EntitiesToComponents = new Dictionary<EcsEntity, Dictionary<Type, EcsComponent>>();
        
        private static int _entityId;

        static EcsWorld()
        {
            var ecsComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(EcsComponent).IsAssignableFrom(x));

            foreach (var ecsComponentType in ecsComponentTypes)
            {
                ComponentTypeToComponentToEntity[ecsComponentType] = new Dictionary<EcsComponent, EcsEntity>();
                ComponentTypeToEntityToComponent[ecsComponentType] = new Dictionary<EcsEntity, EcsComponent>();
            }
        }
        
        public static EcsEntity CreateEntity()
        {
            var go = new GameObject($"Entity {_entityId}");
            _entityId++;
            
            var entity = go.AddComponent<EcsEntity>();
            Entities.Add(entity);
            return entity;
        }
        
        public static void DestroyEntity(EcsEntity entity)
        {
            var components = EntitiesToComponents[entity].Select(x => x.Value);
            foreach (EcsComponent component in components)
            {
                ComponentTypeToComponentToEntity[component.GetType()].Remove(component);
            }
            EntitiesToComponents.Remove(entity);
            Entities.Remove(entity);
            UnityEngine.Object.Destroy(entity.gameObject);
        }
        
        public static void AddComponent(EcsEntity entity, EcsComponent component)
        {
            Type componentType = component.GetType();

            var componentToEntities = ComponentTypeToComponentToEntity.ContainsKey(componentType)
                ? ComponentTypeToComponentToEntity[componentType]
                : ComponentTypeToComponentToEntity[componentType] = new Dictionary<EcsComponent, EcsEntity>();
            componentToEntities[component] = entity;

            var componentToEntities2 = ComponentTypeToEntityToComponent.ContainsKey(componentType)
                ? ComponentTypeToEntityToComponent[componentType]
                : ComponentTypeToEntityToComponent[componentType] = new Dictionary<EcsEntity, EcsComponent>();
            componentToEntities2[entity] = component;
            
            var entitiesToComponents = EntitiesToComponents.ContainsKey(entity) 
                ? EntitiesToComponents[entity] 
                : EntitiesToComponents[entity] = new Dictionary<Type, EcsComponent>();
            entitiesToComponents[componentType] = component;
        }
        
        public static void RemoveComponent(EcsEntity entity, EcsComponent component)
        {
            Type componentType = component.GetType();
            if (ComponentTypeToComponentToEntity.ContainsKey(componentType))
            {
                ComponentTypeToComponentToEntity[componentType].Remove(component);
            }
            if (ComponentTypeToEntityToComponent.ContainsKey(componentType))
            {
                ComponentTypeToEntityToComponent[componentType].Remove(entity);
            }
            if (EntitiesToComponents.ContainsKey(entity))
            {
                EntitiesToComponents[entity].Remove(componentType);
            }
        }
    }
}