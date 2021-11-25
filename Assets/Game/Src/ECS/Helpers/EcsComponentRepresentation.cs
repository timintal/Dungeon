using Leopotam.EcsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Src.ECS.Helpers
{
    public abstract class EcsComponentRepresentation : MonoBehaviour
    {
        public abstract void AddToEntity(int entity, EcsWorld world);
    }
    
    public abstract class EcsComponentRepresentation<T> : EcsComponentRepresentation where T: struct
    {
        [SerializeField] protected T _component;

        public T Component
        {
            get => _component;
            set => _component = value;
        }

#if UNITY_EDITOR && ENABLE_ECS_DEBUG
        [SerializeField] [DisableIf(nameof(_setCurrentValues))] [ShowIf("@UnityEngine.Application.isPlaying")]
        private bool _syncCurrentValues;
        [SerializeField] [DisableIf(nameof(_syncCurrentValues))][ShowIf("@UnityEngine.Application.isPlaying")]
        private bool _setCurrentValues;
        private EcsPackedEntityWithWorld _packedEntity;
#endif

        public override void AddToEntity(int entity, EcsWorld world)
        {
            var pool = world.GetPool<T>();
            if (pool.Has(entity))
            {
                pool.Del(entity);
            }

            pool.Add(entity) = _component;
            
#if UNITY_EDITOR && ENABLE_ECS_DEBUG
            _packedEntity = world.PackEntityWithWorld(entity);
#endif
        }

#if UNITY_EDITOR && ENABLE_ECS_DEBUG
        
        private void Update()
        {
            if (_syncCurrentValues)
            {
                UpdateComponentValues();
            }

            if (_setCurrentValues)
            {
                TryUpdateEntityValues();
            }
        }
        
        void TryUpdateEntityValues()
        {
            if (_packedEntity.Unpack(out EcsWorld world, out int entity))
            {
                EcsPool<T> pool = world.GetPool<T>();

                ref T comp = ref pool.Get(entity);
                comp = _component;
            }
            else
            {
                _setCurrentValues = false;
                Debug.LogError("Entity is no longer valid!");
            }
        }

        void UpdateComponentValues()
        {
            if (_packedEntity.Unpack(out EcsWorld world, out int entity))
            {
                EcsPool<T> pool = world.GetPool<T>();

                T comp = pool.Get(entity);
                _component = comp;
            }
            else
            {
                _syncCurrentValues = false;
                Debug.LogError("Entity is no longer valid!");
            }
        }
#endif

    }
}

