using UnityEngine;

public class GunController : MonoBehaviour
{
    protected Inventory m_ConnectedInventory;
    [SerializeField]
    protected Gun m_EquippedGun;
    protected Magazine<Ammo> m_Magazine;
    protected float m_Cooldown;

    [SerializeField]
    private LightDrop _MuzzleFlash;
    [SerializeField]
    private Transform MuzzleTransform;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AudioSource source;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.m_ConnectedInventory = this.GetComponentInChildren<Inventory>();
        this.m_Cooldown = this.m_EquippedGun.FireRate;
    }

    protected virtual void Shoot()
    {
        if (m_EquippedGun == null || m_Magazine == null) return;
        if (m_Magazine.TryReduce(1))
        {
            SpawnProjectiles();
            source.PlayOneShot(m_EquippedGun.ShotClip);
            m_Cooldown = m_EquippedGun.FireRate;
        }
    }

    protected virtual bool Reload()
    {
        if (m_Magazine == null || m_ConnectedInventory.TryAddItems(m_Magazine))
        {
            animator.SetTrigger("Reload");
            source.PlayOneShot(m_EquippedGun.ReloadClip);
            ItemDatabase.instance.TryGetStackSize(m_EquippedGun.compatibleAmmo.id, out int amount);
            int stacksize = amount;
            m_ConnectedInventory.TryRemoveItems(m_EquippedGun.compatibleAmmo.id, ref amount);
            int remaining = stacksize - amount;
            m_Magazine = new Magazine<Ammo>(m_EquippedGun.compatibleAmmo, remaining, stacksize);
            return true;
        }
        return false;
    }

    protected virtual void SpawnProjectiles()
    {
        LightDrop bulletPrefab = m_Magazine.getValue().BulletPrefab;
        float projectileSpeed = m_EquippedGun.MuzzleVelocity;
        for (int i = 0; i < m_EquippedGun.ProjectileAmount; i++)
        {
            LightDrop bullet = Instantiate(bulletPrefab, MuzzleTransform.position, MuzzleTransform.rotation);
            float width = Random.Range(-1f, 1f) * m_EquippedGun.SpreadAngle;
            bullet.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
            bullet.GetComponentInChildren<Bullet>().damage = m_Magazine.getValue().damage;
        }
    }
    public virtual void EquipGun(Gun gun)
    {
        int amount = 1;
        if (m_ConnectedInventory.TryRemoveItems(gun.id, ref amount))
        {
            m_ConnectedInventory.TryAddItems(m_EquippedGun.id, 1);
            m_EquippedGun = gun;
            Reload();
        }
    }
}
