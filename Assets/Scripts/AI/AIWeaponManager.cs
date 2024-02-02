using System;
using System.Collections.Generic;
using Pulsar.Debug;
using Pulsar.Utils;
using UnityEngine;

public class AIWeaponManager : MonoBehaviour
{
    // [SerializeField] private Transform _spawnLocation;
    // [SerializeField] private int _startingIndex;
    // [SerializeField] private List<WeaponBase> _startingWeapons; // List of weapon prefabs to start with
    // private List<WeaponBase> _weaponInventory = new List<WeaponBase>(); // List of currently spawned weapons
    //
    // protected Pawn _pawnAI;
    // protected WeaponBase _activeWeapon;
    //
    // protected virtual void Awake()
    // {
    //     _pawnAI = GetComponent<Pawn>();
    //     if (DebugUtils.CheckForNull(_pawnAI, $"AI_WeaponManager: Failed to find Pawn in {gameObject} object")) return;
    // }
    //
    // protected void Start()
    // {
    //     // Instantiate and add the starting weapons
    //     foreach (WeaponBase weapon in _startingWeapons)
    //     {
    //         AddWeapon(weapon);
    //     }
    //
    //     EquipWeapon(_startingIndex);
    // }
    //
    // public void AddWeapon(WeaponBase weaponPrefab)
    // {
    //     if (weaponPrefab == null)
    //     {
    //         Debug.LogError("AI_WeaponManager: Attempted to add a null weapon.");
    //         return;
    //     }
    //
    //     // Instantiate and add the new weapon to the inventory
    //     WeaponBase newWeapon = Instantiate(weaponPrefab, _spawnLocation);
    //     Utils.SetLayerRecursively(newWeapon.gameObject, LayerMask.NameToLayer("TP_Weapon"));
    //     InitializeWeapon(newWeapon);
    //
    //     _weaponInventory.Add(newWeapon);
    // }
    //
    // private void InitializeWeapon(WeaponBase weapon)
    // {
    //     weapon.Pawn = _pawnAI;
    //     Transform weaponTransform = weapon.transform;
    //     weaponTransform.localPosition = weapon.WeaponOffset;
    //     weaponTransform.localRotation = Quaternion.identity;
    //     weaponTransform.localScale = Vector3.one;
    // }
    //
    // public void RemoveWeapon(WeaponBase weapon)
    // {
    //     if (weapon == null)
    //     {
    //         Debug.LogError("AI_WeaponManager: Attempted to remove a null weapon.");
    //         return;
    //     }
    //
    //     if (HasWeapon(weapon, out int index))
    //     {
    //         if (_activeWeapon == weapon)
    //         {
    //             _activeWeapon.gameObject.SetActive(false);
    //             _activeWeapon = null;
    //         }
    //
    //         Destroy(_weaponInventory[index].gameObject); // Destroy the spawned weapon
    //         _weaponInventory.RemoveAt(index);
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"AI_WeaponManager: The weapon to be removed is not in the inventory.");
    //     }
    // }
    //
    // private void EquipWeapon(int index)
    // {
    //     if (index < 0 || index >= _weaponInventory.Count) return;
    //
    //     if (_activeWeapon != null)
    //     {
    //         _activeWeapon.gameObject.SetActive(false); // Disable the currently active weapon
    //     }
    //
    //     _activeWeapon = _weaponInventory[index];
    //     _activeWeapon.gameObject.SetActive(true); // Enable the new active weapon
    //     InitializeWeapon(_activeWeapon);
    // }
    //
    // public void Fire()
    // {
    //     if (_activeWeapon != null)
    //     {
    //         // Call the active weapon's firing method
    //         _activeWeapon.StartFire();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("AIWeaponManager: No active weapon to fire.");
    //     }
    // }
    //
    // private bool HasWeapon(WeaponBase weapon, out int index)
    // {
    //     index = _weaponInventory.IndexOf(weapon);
    //     return index != -1;
    // }
}