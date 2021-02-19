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
    protected Animator animator;
    [SerializeField]
    private AudioSource source;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.m_ConnectedInventory = this.GetComponentInChildren<Inventory>();
        if (m_EquippedGun == null || _EquippedAmmo == null)
            Debug.LogWarning($"This gun controller on {gameObject.name} is missing either it's equipped gun or ammo, these should be initially assigned in the inspector");
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
        m_Cooldown = m_EquippedGun.ReloadTime;
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
        LightDrop projectilePrefab = _EquippedAmmo.BulletPrefab;
        float projectileSpeed = m_EquippedGun.MuzzleVelocity;
        for (int i = 0; i < m_EquippedGun.ProjectileAmount; i++)
        {
            LightDrop projectile = Instantiate(projectilePrefab, MuzzleTransform.position, MuzzleTransform.rotation);
            float width = Random.Range(-1f, 1f) * m_EquippedGun.SpreadAngle;
            projectile.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
            Bullet bullet = projectile.GetComponentInChildren<Bullet>();
            if (bullet != null) bullet.damage = _EquippedAmmo.damage * m_EquippedGun.DamageScale;
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
