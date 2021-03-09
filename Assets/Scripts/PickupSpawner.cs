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
    private Pickup[] pickUps;

    // Start is called before the first frame update
    private void Start()
    {
        pickUps = new Pickup[maxPickups];
        InvokeRepeating("spawnPickups", 5, 5);
    }

    private void spawnPickups()
    {
        Vector2 point = LightManager.instance.FindValidSpawnPosition();
        if (index > maxPickups)
            index = 0;
        //Recycle old pickup
        if (pickUps[index] != null) Destroy(pickUps[index].gameObject);
        pickUps[index] = Instantiate(prefabs[index % prefabs.Length], point,
                                          Quaternion.identity,
                                          transform);
        index++;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, center.pointLightOuterRadius);
        Gizmos.DrawWireSphere(Vector3.zero, spawnRadius);
    }
}
