using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

namespace Assets.Scripts.Utility {
    public class GridManager : MonoBehaviour
    {
        public static GridManager instance;

        private Dictionary<Vector2Int, List<GridObject>> grid;
        [SerializeField]
        private Vector2Int Size;
        [SerializeField]
        private float cellSize;
        [SerializeField]
        private List<GridObject> gridObjects;
        private void Awake()
        {
            if (instance) Destroy(instance);
            instance = this;
        }
        private void Start()
        {
            gridObjects = new List<GridObject>();
            grid = new Dictionary<Vector2Int, List<GridObject>>();
        }

        public void Add(GridObject gridObject)
        {
            if (!gridObjects.Contains(gridObject))
                gridObjects.Add(gridObject);
        }
        public void Remove(GridObject gridObject)
        {
            gridObjects.Remove(gridObject);
        }
        private void Update()
        {
            if (gridObjects.Count == 0)
                return;
            grid.Clear();
            Dictionary<GridObject, MapPositionJob> mapPositionJobs = new Dictionary<GridObject, MapPositionJob>(gridObjects.Count);
            NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(gridObjects.Count, Allocator.TempJob);
            for(int i = 0; i < gridObjects.Count; i++)
            {
                var mapPos = new MapPositionJob
                {
                    CellSize = cellSize,
                    Position = gridObjects[i].transform.position,
                    OffSet = transform.position,
                    Result = new NativeArray<Vector2Int>(1, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
                };
                mapPositionJobs.Add(gridObjects[i], mapPos);
                jobs[i] = mapPos.Schedule();
            }
            JobHandle.CompleteAll(jobs);
            foreach (GridObject gridObject in gridObjects)
            {
                Vector2Int gridPos = mapPositionJobs[gridObject].Result[0];
                if (grid.TryGetValue(gridPos, out List<GridObject> objects))
                    objects.Add(gridObject);
                else
                {
                    List<GridObject> objectsInCell = new List<GridObject>();
                    objectsInCell.Add(gridObject);
                    grid.Add(gridPos, objectsInCell);
                }
                mapPositionJobs[gridObject].Result.Dispose();
            }
        }
        public T[] GetGridObjectsInRadius<T>(Vector2 position, float radius) where T : GridObject
        {
            Vector2Int min = WorldPosToGridPos(new Vector2(position.x - radius, position.y - radius));
            Vector2Int max = WorldPosToGridPos(new Vector2(position.x + radius, position.y + radius));
            List<T> result = new List<T>();
            for (int x = min.x; x < max.x + 1; x++)
            {
                for (int y = min.y; y < max.y + 1; y++)
                {
                    if (grid.TryGetValue(new Vector2Int(x,y), out List<GridObject> objectsInCell))
                    {
                        foreach(GridObject gridObject in objectsInCell)
                        {
                            if (gridObject is T desiredType && desiredType.isActiveAndEnabled)
                                result.Add(desiredType);
                        }
                    }
                }
            }
            return result.ToArray();
        }
        public Vector2Int WorldPosToGridPos(Vector2 worldPos)
        {
            return WorldPosToGridPos(worldPos, transform.position, cellSize);
        }
        public static Vector2Int WorldPosToGridPos(Vector2 worldPos, Vector2 offset, float cellSize)
        {
            Vector2 adjusted = worldPos - offset;
            return new Vector2Int(Mathf.FloorToInt(adjusted.x / cellSize), Mathf.FloorToInt(adjusted.y / cellSize));
        }
        [BurstCompile]
        private struct MapPositionJob : IJob
        {
            public Vector2 Position;
            public Vector2 OffSet;
            public float CellSize;
            public NativeArray<Vector2Int> Result;
            public void Execute()
            {
                Result[0] = WorldPosToGridPos(Position, OffSet, CellSize);
            }
        }
        private void OnDrawGizmos()
        {
            Bounds bounds = new Bounds(transform.position, (Vector2)Size);
            Gizmos.DrawWireCube(transform.position, (Vector2)Size);
            Vector2 min = (Vector2)WorldPosToGridPos(bounds.min) * cellSize;
            Vector2 max = (Vector2)WorldPosToGridPos(bounds.max) * cellSize;
            for (float x = min.x; x < max.x; x += cellSize)
                Gizmos.DrawLine(new Vector2(x, min.y), new Vector2(x, max.y));
            for (float y = min.y; y < max.y; y += cellSize)
                Gizmos.DrawLine(new Vector2(min.x, y), new Vector2(max.x, y));
        }
    }
}
