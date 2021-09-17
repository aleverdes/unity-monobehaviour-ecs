using UnityEngine;

namespace AffenECS
{
    [RequireComponent(typeof(EcsEntity))]
    public abstract class EcsComponent : MonoBehaviour
    {
        [HideInInspector] public EcsEntity Entity;

        protected virtual void Reset()
        {
            Entity = GetComponent<EcsEntity>();
        }

        protected virtual void Awake()
        {
            if (!Entity)
            {
                Entity = GetComponent<EcsEntity>();
            }

            if (Entity && !Entity.Has(GetType()))
            {
                Entity.Add(this);
            }
        }

        protected virtual void OnDestroy()
        {
            Entity.Remove(this);
        }

        public void Destroy()
        {
            Destroy(this);
        }
    }
}