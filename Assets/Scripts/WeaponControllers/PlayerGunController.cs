using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerGunController : GunController
{
    private PlayerInput.PlayerActions input;
    private bool isShooting = false;

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
        if (m_Cooldown < 0.01f && isShooting && !EventSystem.current.IsPointerOverGameObject())
        {
            Shoot();
            if (m_Magazine.TotalNumberOfItems == 0)
            {
                if (!Reload())
                {
                    
                }
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
        int amountInInventory = m_ConnectedInventory.GetStack(m_EquippedGun.compatibleAmmo, out IStackable<Item> items) ? items.TotalNumberOfItems : 0;
        ammoText.text = $"{m_Magazine.size} / {amountInInventory}";
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
