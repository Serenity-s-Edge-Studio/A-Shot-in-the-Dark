using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoissonDiscSampling : MonoBehaviour
{
    public static List<Vector2> GetPositions(Collider2D collider, Dictionary<Vector2, float> reservedSpace, float radius, int numSamplesBeforeRejection = 30)
    {
        float cellSize = radius / Mathf.Sqrt(2);
        Bounds bounds = collider.bounds;
        Vector3 extents = bounds.extents * 2;
        int[,] grid = new int[Mathf.CeilToInt(extents.x / cellSize), Mathf.CeilToInt(extents.y / cellSize)];
        List<Vector2> points = new List<Vector2>(); //Where the points will actually be;
        List<Vector2> spawnPoints = reservedSpace?.Keys.ToList() ?? new List<Vector2>(); //Where we'll try to spawn points around;
        Dictionary<Vector2, float> storedRadii = reservedSpace ?? new Dictionary<Vector2, float>(); //Where we will store the associated Radii.

        spawnPoints.Add(bounds.center);

        while(spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                Vector2 dir = Random.insideUnitCircle;
                float currentRadius = storedRadii.TryGetValue(spawnCentre, out float stored) ? stored : radius;
                Vector2 candidate = spawnCentre + dir * Random.Range(currentRadius, currentRadius + radius);
                if (IsCandidateValid(candidate, bounds, cellSize, radius, points, grid, storedRadii))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    storedRadii.Add(candidate, radius);
                    Vector2Int gridPos = GetIndex(candidate, extents / 2, bounds.center, cellSize);
                    grid[gridPos.x, gridPos.y] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        return points;
    }
    private static Vector2Int GetIndex(Vector2 position, Vector2 extents, Vector2 center, float cellSize)
    {
        Vector2 adusted = position - center + extents;
        return new Vector2Int((int)(adusted.x / cellSize), (int)(adusted.y / cellSize));
    }
    private static bool IsCandidateValid(Vector2 candidate, Bounds bounds, float cellSize, float radius, List<Vector2> points, int[,] grid, Dictionary<Vector2, float> storedRadii)
    {
        if (!bounds.Contains(candidate) || storedRadii.ContainsKey(candidate))
            return false;

        //Get cell indicies
        Vector2Int indexes = GetIndex(candidate, bounds.extents, bounds.center, cellSize);
        int cellX = indexes.x;
        int cellY = indexes.y;

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
                int pointIndex = grid[x, y] - 1;
                if (pointIndex != -1)
                {
                    Vector2 point = points[pointIndex];
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
