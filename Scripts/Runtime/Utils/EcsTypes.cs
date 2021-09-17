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

            ComponentTypes = types.Where(x => typeof(EcsComponent).IsAssignableFrom(x)).ToArray();
            SystemTypes = types.Where(x => typeof(EcsSystem).IsAssignableFrom(x)).ToArray();
        }
    }
}