using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Game.Src.ECS.Helpers
{
    public class EcsConverter : MonoBehaviour
    {
        [SerializeField] private List<EcsComponentRepresentation> components;
        [SerializeField] private bool createOnStart;
        [SerializeField] private string worldName;

        private EcsSystem _ecsSystem;

        [Inject] [UsedImplicitly]
        public void Construct(EcsSystem b)
        {
            _ecsSystem = b;
        }
    
        private void Start()
        {
            if (createOnStart)
            {
                CreateEntity();
            }
        }

        public void CreateEntity()
        {
            EcsWorld world = _ecsSystem.GetWorld(worldName);

            if (world != null)
            {
                var entity = world.NewEntity();

                foreach (var component in components)
                {
                    component.AddToEntity(entity, world);
                }
            }
        }
    
        [Button]
        void CacheComponents()
        {
            components = GetComponentsInChildren<EcsComponentRepresentation>().ToList();
        }
    }
}
