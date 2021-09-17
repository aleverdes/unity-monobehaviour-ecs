using System;
using System.Linq;
using UnityEngine;

namespace AffenECS
{
    public class EcsStartupSystems : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            var ecsSystemTypes = EcsTypes.SystemTypes.Where(TypeCondition);

            foreach (Type ecsSystemType in ecsSystemTypes)
            {
                gameObject.AddComponent(ecsSystemType);
            }
        }

        protected virtual bool TypeCondition(Type ecsSystemType)
        {
            return true;
        }
    }
}