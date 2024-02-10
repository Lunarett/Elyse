using System;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    Handgun,
    Rifle,
    Sniper,
    Launcher
}

[System.Serializable]
public struct Ammo
{
    public string name;
    public AmmoType ammoType;
    public int initialCount;
}

public class AmmoManager
{
    private readonly Dictionary<AmmoType, int> _ammoInventory = new Dictionary<AmmoType, int>();

    // Define an event that passes the ammo type and the new count
    public event Action<AmmoType, int> OnAmmoChanged;

    public AmmoManager(List<Ammo> startingAmmo = null)
    {
        if (startingAmmo is not { Count: > 0 }) return;

        foreach (var ammoInitialization in startingAmmo)
        {
            _ammoInventory[ammoInitialization.ammoType] = ammoInitialization.initialCount;
        }
    }
    
    public void AddAmmo(AmmoType ammoType, int amount)
    {
        _ammoInventory.TryAdd(ammoType, 0);
        
        _ammoInventory[ammoType] = Mathf.Clamp(_ammoInventory[ammoType] + amount, 0, int.MaxValue);
        
        // Invoke the event after adding ammo
        OnAmmoChanged?.Invoke(ammoType, _ammoInventory[ammoType]);
    }
    
    public bool UseAmmo(AmmoType ammoType, int amount)
    {
        if (!_ammoInventory.ContainsKey(ammoType) || _ammoInventory[ammoType] < amount)
        {
            // Out of Ammo
            return false;
        }

        _ammoInventory[ammoType] -= amount;
        
        // Invoke the event after using ammo
        OnAmmoChanged?.Invoke(ammoType, _ammoInventory[ammoType]);
        
        return true;
    }

    public int CheckAmmo(AmmoType ammoType)
    {
        return _ammoInventory.GetValueOrDefault(ammoType, 0);
    }
}
