using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

public class PoissonDiscSampling : MonoBehaviour
{
    public static IEnumerator GetPositionsCoroutine(PoissonGrid pGrid, LayerMask OverlapCollider, int numSamplesBeforeRejection = 30, int stepTime = 10)
    {
        Collider2D collider = pGrid.collider;
        float radius = pGrid.radius;
        float cellSize = radius / Mathf.Sqrt(2);
        pGrid.cellSize = cellSize;
        Bounds bounds = collider.bounds;
        Vector3 size = bounds.size;
        int?[,] grid = new int?[Mathf.CeilToInt(size.x / cellSize), Mathf.CeilToInt(size.y / cellSize)];
        pGrid.grid = grid;
        List<Vector2> points = new List<Vector2>(); //Where the points will actually be;
        pGrid.points = points;
        List<Collider2D> colliders = CastColliders(collider, OverlapCollider);
        List<Vector2> spawnPoints = CalculateStartingSpawnPoints(collider, colliders);

        var watch = new Stopwatch();
        watch.Start();
        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                Vector2 dir = Random.insideUnitCircle;
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);
                if (ValidatePosition(candidate, collider, cellSize, radius, points, grid, colliders))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    Vector2Int gridPos = GetIndex(candidate, bounds.min, cellSize);
                    grid[gridPos.x, gridPos.y] = points.Count - 1;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
            if (watch.ElapsedMilliseconds > stepTime)
            {
                yield return null;
                watch.Reset();
            }
        }
        watch.Stop();
    }
    private static List<Vector2> CalculateStartingSpawnPoints(Collider2D collider, List<Collider2D> others)
    {
        List<Vector2> spawnPoints = new List<Vector2>(); //Where we'll try to spawn points around;
        if (others.Count > 0 || !collider.OverlapPoint(collider.bounds.center))
        {
            List<Vector3> verticies = new List<Vector3>();
            collider.CreateMesh(true, true).GetVertices(verticies);

            spawnPoints.AddRange(verticies.ConvertAll<Vector2>(vertex3 => new Vector2(vertex3.x, vertex3.y)));
        }
        spawnPoints.Add(collider.bounds.center);
        return spawnPoints;
    }

    public static List<Collider2D> CastColliders(Collider2D collider, LayerMask OverlapCollider)
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(OverlapCollider);
        contactFilter.useTriggers = true;
        Physics2D.OverlapCollider(collider, contactFilter, colliders);
        return colliders;
    }

    private static Vector2Int GetIndex(Vector2 position, Vector2 center, float cellSize)
    {
        Vector2 adjusted = position - center;
        return new Vector2Int(Mathf.FloorToInt(adjusted.x / cellSize), Mathf.FloorToInt(adjusted.y / cellSize));
    }
    public static bool ValidatePosition(Vector2 candidate, Collider2D collider, float cellSize, float radius, List<Vector2> points, int?[,] grid, List<Collider2D> otherColliders)
    {
        if (!collider.OverlapPoint(candidate))
            return false;
        foreach (Collider2D other in otherColliders)
        {
            if (other.OverlapPoint(candidate))
                return false;
        }
        Bounds bounds = collider.bounds;
        //Get cell indicies
        Vector2Int indexes = GetIndex(candidate, bounds.min, cellSize);
        int cellX = indexes.x;
        int cellY = indexes.y;
        if (grid[cellX, cellY] != null) return false;

        //Characterize our search area
        int searchAreaSide = (int)(2f * radius) + 3;
        int searchAddition = (searchAreaSide - 1) / 2;

        // Get starting and ending positions for where we will search with additional range to compensate for our radius
        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

        for (int x = searchStartX; x < searchEndX; x++)
        {
            for(int y = searchStartY; y < searchEndY; y++)
            {
                //if (x == cellX && y == cellY) continue;
                int? pointIndex = grid[x, y];
                if (pointIndex != null)
                {
                    Vector2 point = points[(int)pointIndex];
                    float sqrDst = (candidate - point).sqrMagnitude;
                    if (sqrDst < radius * radius)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
public class PoissonGrid
{
    public Collider2D collider;
    public float cellSize;
    public float radius;
    public List<Vector2> points;
    public int?[,] grid;
}
