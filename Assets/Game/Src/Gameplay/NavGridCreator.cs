using Astar;
using Game.Src.ECS.Components.Pathfinding;
using Game.Src.ECS.Components.Pathfinding.UnityRepresentation;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using VContainer;

public class NavGridCreator : MonoBehaviour
{
    [SerializeField] private GridComponentRepresentation _grid;
    
    #if UNITY_EDITOR
    [SerializeField] private GridNodeVisualizeComponentRepresentation _nodeDebugVisual;
    #endif

    private EcsSystem _ecsSystem;
    
    [Inject][UsedImplicitly]
    public void Construct(EcsSystem ecs)
    {
        _ecsSystem = ecs;
    }

    public void Start()
    {
        CreateEntities();
    }
    
    public void CreateEntities()
    {
        EcsWorld world = _ecsSystem.GetWorld(string.Empty);

        if (world != null)
        {
            var gridEntity = world.NewEntity();

            SetupGridComponent(world);

            _grid.AddToEntity(gridEntity, world);
            _nodeDebugVisual.AddToEntity(gridEntity, world);
        }
        
    }

    private void SetupGridComponent(EcsWorld world)
    {
        GridComponent gridComponent = _grid.Component;

        gridComponent.Grid =
            new NativeHashMap<int2, GridNode>(gridComponent.XMax * gridComponent.YMax, Allocator.Persistent);
        
        gridComponent.XMax = Mathf.RoundToInt(gridComponent.GridSize.x / gridComponent.NodeSize);
        gridComponent.YMax = Mathf.RoundToInt(gridComponent.GridSize.z / gridComponent.NodeSize);
        gridComponent.GridStartPoint =
            transform.position - new Vector3(gridComponent.GridSize.x / 2, 0, gridComponent.GridSize.z / 2);

        var gridNodeComponentsPool = world.GetPool<GridNodeComponent>();
        for (int i = 0; i < gridComponent.XMax; i++)
        {
            for (int j = 0; j < gridComponent.YMax; j++)
            {
                var currIndex = new int2(i, j);
                gridComponent.Grid.Add(currIndex, new GridNode{GridIndex = currIndex});

                CreateGridNodeEntity(world, currIndex, gridNodeComponentsPool, gridComponent);
            }
        }

        _grid.Component = gridComponent;
    }

    private void CreateGridNodeEntity(EcsWorld world, int2 currIndex, EcsPool<GridNodeComponent> gridNodeComponentsPool, GridComponent grid)
    {
        var nodeEntity = world.NewEntity();

        GridNodeComponent gridNodeComponent = new GridNodeComponent();
        gridNodeComponent.Index = currIndex;
        gridNodeComponent.NodeSize = grid.NodeSize;
        gridNodeComponent.WorldPosition = GetNodeWorldPos(currIndex, grid);
        gridNodeComponent.WorldMatrix = 
            Matrix4x4.Translate(gridNodeComponent.WorldPosition) *
            Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0)) * Matrix4x4.Scale(Vector3.one * grid.NodeSize * 0.9f);


        if (gridNodeComponentsPool.Has(nodeEntity))
        {
            gridNodeComponentsPool.Del(nodeEntity);
        }

        gridNodeComponentsPool.Add(nodeEntity) = gridNodeComponent;
    }
    
    Vector3 GetNodeWorldPos(int2 index, GridComponent grid)
    {
        return grid.GridStartPoint + new Vector3((index.x + 0.5f) * grid.NodeSize, 0, (index.y + 0.5f) * grid.NodeSize);
    }

    private void OnDisable()
    {
        _grid.Component.Grid.Dispose();
    }
}