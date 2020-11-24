using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField]
    private int maxPickups;
    [SerializeField]
    private Pickup[] prefabs;
    [SerializeField]
    private float spawnRadius;
    [SerializeField]
    private Light2D center;
    private int index;
    Pickup[] pickUps;
    // Start is called before the first frame update
    void Start()
    {
        pickUps = new Pickup[maxPickups];
        InvokeRepeating("spawnPickups", 5, 5);
    }
    private void spawnPickups()
    {
        float innerRadius = center.pointLightOuterRadius;
        float ratio = innerRadius / spawnRadius;
        float radius = Mathf.Sqrt(Random.Range(ratio * ratio, 1f)) * spawnRadius;
        Vector2 point = Random.insideUnitCircle.normalized * radius;
        if (pickUps[index] != null) Destroy(pickUps[index].gameObject);
        pickUps[index] = Instantiate(prefabs[index % prefabs.Length], point,
                                          Quaternion.identity,
                                          transform);
        if (index >= maxPickups)
            index = 0;
        else
            index++;

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, center.pointLightOuterRadius);
        Gizmos.DrawWireSphere(Vector3.zero, spawnRadius);
    }
}
