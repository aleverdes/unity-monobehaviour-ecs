using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AffenECS
{
    public class EcsWorld
    {
        public static readonly HashSet<EcsEntity> Entities = new HashSet<EcsEntity>();
        
        private static int _entityId;
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
            Entities.Remove(entity);
            UnityEngine.Object.Destroy(entity);
        }
    }
}