using System;
using UnityEngine;

public class WeaponPickup : PickupBase
{
    [SerializeField] private WeaponBase _weaponPrefab;

    private HUD _hud;
    
    private void Awake()
    {
        _hud = HUD.Instance;
    }

    protected override void OnPickup(Pawn pawn)
    {
        WeaponManager weaponManager = pawn.GetComponent<WeaponManager>();
        if (weaponManager == null) return;

        if (weaponManager.HasWeapon(_weaponPrefab.Name))
        {
            weaponManager.AmmoManager.AddAmmo(_weaponPrefab.AmmoType, _weaponPrefab.ClipSize);
        }
        else
        {
            weaponManager.AddWeapon(_weaponPrefab);
        }
    }
}