using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private PoissonGrid pGrid;
    private bool isGenerating;
    protected virtual void Start()
    {
        StartCoroutine(GeneratePositions(1000));
    }
    protected bool GetNextPosition(out Vector2 position)
    {
        position = Vector2.zero;
        if (collider == null || isGenerating)
            return false;
        if (points != null)
        {
            List<Collider2D> colliders = PoissonDiscSampling.CastColliders(collider, layerMask);
            while (points.Count > 0)
            {
                position = points.Dequeue();
                bool inFOV = Player.instance != null ? Player.instance.IsPositionInFOV(position) : false;
                foreach (Collider2D other in colliders)
                {
                    if (!inFOV && !other.OverlapPoint(position))
                        return true;
                }
            }
        }
        StartCoroutine(GeneratePositions(10));
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
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    public IEnumerator GeneratePositions(int stepTime)
    {
        isGenerating = true;
        pGrid = new PoissonGrid
        { collider = this.collider, radius = this.radius };
        yield return PoissonDiscSampling.GetPositionsCoroutine(pGrid, layerMask, numSamplesBeforeRejection, stepTime);
        List<Vector2> newPositions = pGrid.points.ToArray().ToList();
        newPositions.Shuffle();
        points = new Queue<Vector2>(newPositions);
        isGenerating = false;
    }
}
