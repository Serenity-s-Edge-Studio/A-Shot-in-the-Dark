using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGunController : GunController
{
    public Vector2 TargetPos;
    public void OnEnable()
    {
        TurretManager.instance._Turrets.Add(this);
    }
    private void OnDisable()
    {
        TurretManager.instance._Turrets.Remove(this);
    }
}
