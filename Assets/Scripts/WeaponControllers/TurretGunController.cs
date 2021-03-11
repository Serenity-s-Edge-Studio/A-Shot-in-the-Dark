using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGunController : GunController
{
    public Vector2 TargetPos;

    public float Cooldown { get 
        {
            return m_Cooldown;
        }
        set 
        {
            m_Cooldown = Mathf.Clamp(value, 0, m_EquippedGun.FireRate);
        } 
    }
    public void ShootTurret()
    {
        //Implement removing bullets from magazine and integrate inventory.
        _BulletsInMagazine++;
        Shoot();
    }
    public void OnEnable()
    {
        TurretManager.instance._Turrets.Add(this);
        _BulletsInMagazine = 100;
    }
    private void OnDisable()
    {
        TurretManager.instance._Turrets.Remove(this);
    }
}
