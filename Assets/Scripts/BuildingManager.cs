using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    public List<Building> activeBuildings = new List<Building>();
    public List<Campfire> SpawnPoints = new List<Campfire>();

    private void Awake()
    {
        if (instance) Destroy(instance);
        instance = this;
    }
    public void Add(Building building)
    {
        activeBuildings.Add(building);
        if (building is Campfire campfire){
            SpawnPoints.Add(campfire);
        }
    }
    public void Remove(Building building)
    {
        activeBuildings.Remove(building);
        if (building is Campfire campfire)
        {
            SpawnPoints.Remove(campfire);
        }
    }
    /// <summary>
    /// Iterates through all avaliable campfires and returns the closest one to position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector2 GetClosestRespawnPoint(Vector2 position)
    {
        PriorityQueue<Vector2, float> campfireQueue = new PriorityQueue<Vector2, float>(0);
        SpawnPoints.ForEach(campfire => campfireQueue.Insert(campfire.transform.position, Vector2.Distance(campfire.transform.position, position)));
        return campfireQueue.Count > 0 ? campfireQueue.Pop() + Vector2.up : Vector2.zero;
    }
}
