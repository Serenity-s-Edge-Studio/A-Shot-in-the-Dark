using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonSpawner : MonoBehaviour
{
    [Header("Poisson Settings")]
    [SerializeField]
    private new Collider2D collider;
    [SerializeField]
    private float radius;
    [SerializeField]
    private float displayRadius;
    [SerializeField]
    private int numSamplesBeforeRejection;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private Queue<Vector2> points;

    protected bool GetNextPosition(out Vector2 position)
    {
        if (points != null && points.Count > 0)
        {
            position = points.Dequeue();
            return true;
        }
        if (collider == null)
        {
            position = Vector2.zero;
            return false;
        }
        GeneratePositions();
        if (points.Count > 0)
        {
            position = points.Dequeue();
            return true;
        }
        position = Vector2.zero;
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        if (points != null)
        {
            foreach (Vector2 position in points.ToArray())
            {
                Gizmos.DrawSphere(position, displayRadius);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    public void GeneratePositions()
    {
        List<Vector2> newPositions = PoissonDiscSampling.GetPositions(collider, layerMask, radius, numSamplesBeforeRejection);
        newPositions.Shuffle();
        points = new Queue<Vector2>(newPositions);
    }
}
