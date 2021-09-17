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
            
            var ecsSystemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(EcsSystem).IsAssignableFrom(x));

            foreach (Type ecsSystemType in ecsSystemTypes)
            {
                gameObject.AddComponent(ecsSystemType);
            }
        }
    }
}