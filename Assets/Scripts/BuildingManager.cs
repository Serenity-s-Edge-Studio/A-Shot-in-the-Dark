using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    public List<Building> activeBuildings;

    private void Awake()
    {
        if (instance) Destroy(instance);
        instance = this;
    }
}
