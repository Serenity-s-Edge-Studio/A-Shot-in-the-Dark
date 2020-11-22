using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private Type type;
    [SerializeField]
    private int max;
    [SerializeField]
    private int clipSize;
    [SerializeField]
    private int fireRate;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent<Player>(out Player player))
        {
            player.equipWeapon(type, Random.Range(clipSize, max), clipSize, fireRate);
            Destroy(gameObject);
        }
    }
    public enum Type
    {
        Pistol,
        Shotgun, 
        GlowstickLauncher,
        Machinegun,
        Flamethrower
    }
}
