using System;
using System.Linq;

namespace AffenECS
{
    public static class EcsTypes
    {
        public static readonly Type[] ComponentTypes; 
        public static readonly Type[] SystemTypes; 
        
        static EcsTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(x => x.GetTypes());

            ComponentTypes = types.Where(IsComponent).ToArray();
            SystemTypes = types.Where(IsSystem).ToArray();
        }

        public static bool IsSystem(this Type type) => typeof(EcsSystem).IsAssignableFrom(type) && type != typeof(EcsSystem);
        public static bool IsComponent(this Type type) => typeof(EcsComponent).IsAssignableFrom(type) && type != typeof(EcsComponent);
    }
}