using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Building", menuName = "Building")]
public class ConstructableSO : ScriptableObject
{
    public List<ItemStack> RequiredResources;
    public Building buildingPrefab;
    public Sprite icon;
    public bool CanRotate = true;
}
