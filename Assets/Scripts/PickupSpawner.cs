using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField]
    private int maxPickups;
    [SerializeField]
    private PickupSpawnChance[] items;
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
        //Find sum of priorities
        float total = 0;
        foreach (PickupSpawnChance itemSpawnChance in items)
        {
            total += itemSpawnChance.Priority;
        }
        System.Array.Sort(items, (a, b) => a.Priority - b.Priority);
        float previousSpawnChance = 0f;
        foreach (PickupSpawnChance item in items)
        {
            float spawnChance = previousSpawnChance + (item.Priority / total);
            item.SpawnChance = spawnChance;
            previousSpawnChance = spawnChance;
        }
        InvokeRepeating(nameof(spawnPickups), 5, 5);
    }

    private void spawnPickups()
    {
        Vector2 point = LightManager.instance.FindValidSpawnPosition();
        if (index > maxPickups)
            index = 0;
        //Recycle old pickup
        if (pickUps[index] != null) Destroy(pickUps[index].gameObject);
        float chance = Random.Range(0f, 1f);
        Pickup drawnPickup = System.Array.Find(items, item => chance <= item.SpawnChance).item;
        pickUps[index] = Instantiate(drawnPickup, point,
                                          Quaternion.identity,
                                          transform);
        index++;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, center.pointLightOuterRadius);
        Gizmos.DrawWireSphere(Vector3.zero, spawnRadius);
    }
    [System.Serializable]
    private class PickupSpawnChance
    {
        public Pickup item;
        public int Priority;
        [HideInInspector]
        public float SpawnChance;
    }
}
