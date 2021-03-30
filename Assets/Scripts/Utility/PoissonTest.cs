using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonTest : MonoBehaviour
{
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
    private List<Vector2> points;
    private void OnValidate()
    {
        //if (collider != null)
            //Compute();
    }
    public void Compute()
    {
        if (collider != null)
            points = PoissonDiscSampling.GetPositions(collider, layerMask, radius, numSamplesBeforeRejection);
    }
    private void OnDrawGizmosSelected()
    {
        if (points != null)
            foreach (Vector2 point in points)
            {
                Gizmos.DrawSphere(point, displayRadius);
            }
        if (collider != null)
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
