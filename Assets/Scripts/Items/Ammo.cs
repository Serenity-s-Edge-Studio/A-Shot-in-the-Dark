using UnityEngine;
[CreateAssetMenu(fileName = "Ammo", menuName = "Items/Ammo")]
public class Ammo : Item
{
    private static LightDrop BulletPlaceholder;

    public string caliber;
    public int damage;
    public LightDrop BulletPrefab;
}
