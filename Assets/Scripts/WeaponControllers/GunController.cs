using UnityEngine;

public class GunController : MonoBehaviour
{
    protected Inventory m_ConnectedInventory;
    [SerializeField]
    protected Gun m_EquippedGun;
    [SerializeField]
    protected Ammo _EquippedAmmo;
    protected int _BulletsInMagazine;
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
        if (m_EquippedGun == null || _EquippedAmmo == null) return;
        if (_BulletsInMagazine > 0)
        {
            _BulletsInMagazine--;
            SpawnProjectiles();
            source.PlayOneShot(m_EquippedGun.ShotClip);
            m_Cooldown = m_EquippedGun.FireRate;
        }
    }

    protected virtual bool Reload()
    {
        if (m_ConnectedInventory.TryAddItems(_EquippedAmmo, _BulletsInMagazine))
        {
            animator.SetTrigger("Reload");
            source.PlayOneShot(m_EquippedGun.ReloadClip);
            //Get magazine capacity.
            ItemDatabase.instance.TryGetStackSize(m_EquippedGun.compatibleAmmo.id, out int magSize);
            //Set ammo type.
            _EquippedAmmo = m_EquippedGun.compatibleAmmo;
            //Check avaliable ammo
            m_ConnectedInventory.CanRetrieveItems(_EquippedAmmo, in magSize, out int missingAmmo);
            _BulletsInMagazine = magSize - missingAmmo;
            //Remove bullets from inventory
            m_ConnectedInventory.TryRetriveItems(_EquippedAmmo, _BulletsInMagazine);
            return true;
        }
        return false;
    }

    protected virtual void SpawnProjectiles()
    {
        LightDrop bulletPrefab = _EquippedAmmo.BulletPrefab;
        float projectileSpeed = m_EquippedGun.MuzzleVelocity;
        for (int i = 0; i < m_EquippedGun.ProjectileAmount; i++)
        {
            LightDrop bullet = Instantiate(bulletPrefab, MuzzleTransform.position, MuzzleTransform.rotation);
            float width = Random.Range(-1f, 1f) * m_EquippedGun.SpreadAngle;
            bullet.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
            bullet.GetComponentInChildren<Bullet>().damage = _EquippedAmmo.damage;
        }
    }
    public virtual void EquipGun(Gun gun)
    {
        if (m_ConnectedInventory.TryRetriveItems(gun, 1))
        {
            m_ConnectedInventory.TryAddItems(m_EquippedGun, 1);
            m_EquippedGun = gun;
            Reload();
        }
    }
}
