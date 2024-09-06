using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunReloadAnimationRelayFunction : MonoBehaviour
{
    private PlayerGunController gunController;

    private void Start()
    {
        gunController = GetComponentInParent<PlayerGunController>();
    }

    public void UpdateUI()
    {
       gunController.UpdateUI();
    }
}
