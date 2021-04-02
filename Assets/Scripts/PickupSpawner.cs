using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PickupSpawner : PoissonSpawner
{
    [SerializeField]
    private int maxPickups;
    private int index;
    [SerializeField]
    private PickupSpawnChance[] items;

    private Pickup[] pickUps;
    private Queue<Vector2> spawnPositions;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
        if (GetNextPosition(out Vector2 point))
        {
            if (index == pickUps.Length)
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
