using System.Collections.Generic;
using Game.Src.ECS.Components.Attacking;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Src.ECS.Systems.Visual
{
    public class ProjectileDrawSystem : IEcsRunSystem, IEcsInitSystem
    {
        [EcsFilter(typeof(ProjectileComponent))]
        private EcsFilter _projectilesFilter;

        [EcsPool] private EcsPool<ProjectileComponent> _projectilesPool;
        private int _projectilesLayer;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _projectilesFilter)
            {
                var projectile = _projectilesPool.Get(entity);

                Graphics.DrawMesh(projectile.Mesh, projectile.TransformationMatrix, projectile.Material, _projectilesLayer);
            }
        }


        public void Init(EcsSystems systems)
        {
            _projectilesLayer = LayerMask.NameToLayer("Default");
        }
    }
}