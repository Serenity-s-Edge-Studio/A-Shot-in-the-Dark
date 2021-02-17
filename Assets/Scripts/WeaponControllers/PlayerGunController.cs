using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerGunController : GunController
{
    public bool CanShoot;

    private PlayerInput.PlayerActions input;
    private bool isShooting = false;
    private bool _CanShoot => CanShoot && isShooting && !EventSystem.current.IsPointerOverGameObject();

    [SerializeField]
    private Image gunImage;
    [SerializeField]
    private TextMeshProUGUI ammoText;
    protected override void Start()
    {
        base.Start();
        input = new PlayerInput().Player;
        input.Enable();
        input.Shoot.performed += _ => isShooting = _.ReadValueAsButton();
        input.Reload.performed += _ => Reload();
        Reload();
    }
    private void Update()
    {
        m_Cooldown = Mathf.Max(0, m_Cooldown - Time.deltaTime);
        if (m_Cooldown < 0.01f && _CanShoot)
        {
            Shoot();
            UpdateUI();
            if (_BulletsInMagazine == 0)
            {
                Reload();
            }
        }
    }
    protected override bool Reload()
    {
        bool result = base.Reload();
        UpdateUI();
        return result;
    }
    public void UpdateUI()
    {
        gunImage.sprite = m_EquippedGun.icon;
        m_ConnectedInventory.TryGetItem(m_EquippedGun.compatibleAmmo, out int amountInInventory);
        ammoText.text = $"{_BulletsInMagazine}/{amountInInventory}";
        //if (currentGunType == Pickup.Type.Pistol)
        //{
        //    ammoText.text = $"{currentMagazine}/∞";
        //}
        //else
        //{
        //    ammoText.text = $"{currentMagazine}/{ammo}";
        //}
    }
}
