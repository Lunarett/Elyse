using System;
using System.Collections.Generic;
using System.IO;
using Pulsar.Debug;
using UnityEngine;
using Pulsar.Utils;
using Unity.Mathematics;

public class WeaponManager : MonoBehaviour
{
    [Header("Spawn Location References")]
    [SerializeField] private Transform _fpsSpawnLocation;
    [SerializeField] private Transform _tpsSpawnLocation;

    [Space] 
    [SerializeField] private int _startingIndex;
    [SerializeField] private List<WeaponBase> _weaponPrefabs;
    
    private PlayerInputManager _inputManager;
    private WeaponBase _activeWeapon;
    private ElyseCharacter _elyseCharacter;
    private int _currentWeaponIndex;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInputManager>();
        if (DebugUtils.CheckForNull<PlayerInputManager>(_inputManager, $"WeaponManager: PlayerInputManager not attached to {gameObject.name}!")) return;
        
        _elyseCharacter = GetComponent<ElyseCharacter>();
        if (DebugUtils.CheckForNull<ElyseCharacter>(_elyseCharacter, $"WeaponManager: ElyseCharacter not attached to {gameObject.name}!")) return;
    }

    void Start()
    {
        if (DebugUtils.CheckForNull<Transform>(_fpsSpawnLocation, "WeaponManager: FPS spawn point is missing!")) return;
        if (DebugUtils.CheckForNull<Transform>(_tpsSpawnLocation, "WeaponManager: TPS spawn point is missing!")) return;
        _currentWeaponIndex = _startingIndex;
        AddWeapon(_currentWeaponIndex);

        _elyseCharacter.OnViewChanged += UpdateWeaponOnViewChanged;
    }

    private void Update()
    {
        if (_activeWeapon == null) return;
        UpdateWeaponFire();
        UpdateWeaponSwitching();
    }

    public void SwitchWeapon(int direction)
    {
        _currentWeaponIndex = (_currentWeaponIndex + direction + _weaponPrefabs.Count) % _weaponPrefabs.Count;
        Destroy(_activeWeapon.gameObject);
        AddWeapon(_currentWeaponIndex);
    }

    public void AddWeapon(int weaponIndex)
    {
        WeaponBase weapon = Instantiate(_weaponPrefabs[weaponIndex], _fpsSpawnLocation);
        Utils.SetLayerRecursively(weapon.gameObject, _elyseCharacter.ViewMode == EViewMode.FPS ?
            LayerMask.NameToLayer("FP_Weapon") :
            LayerMask.NameToLayer("TP_Weapon")
            );
        
        weapon.CharacterReference = _elyseCharacter;
        weapon.transform.localPosition = weapon.WeaponOffset;
        weapon.transform.localRotation = quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        _activeWeapon = weapon;
    }

    private void UpdateWeaponOnViewChanged(EViewMode viewMode)
    {
        switch (viewMode)
        {
            case EViewMode.FPS:
                Utils.SetLayerRecursively(_activeWeapon.gameObject, LayerMask.NameToLayer("FP_Weapon"));
                _activeWeapon.transform.SetParent(_fpsSpawnLocation.transform);
                _activeWeapon.transform.localPosition = _activeWeapon.WeaponOffset;
                _activeWeapon.transform.localRotation = quaternion.identity;
                _activeWeapon.transform.localScale = Vector3.one;
                
                break;
            case EViewMode.TPS:
                Utils.SetLayerRecursively(_activeWeapon.gameObject, LayerMask.NameToLayer("TP_Weapon"));
                _activeWeapon.transform.SetParent(_tpsSpawnLocation.transform);
                _activeWeapon.transform.localPosition = _activeWeapon.WeaponOffset;
                _activeWeapon.transform.localRotation = quaternion.identity;
                _activeWeapon.transform.localScale = Vector3.one;
                break;
        }
    }

    private void UpdateWeaponFire()
    {
        switch (_activeWeapon.FireModeType)
        {
            case FireMode.Auto:
                if (_inputManager.GetFireInputHeld())
                {
                    _activeWeapon.StartFire();
                }

                break;
            case FireMode.Single:
            case FireMode.Burst:
                if (_inputManager.GetFireInputDown())
                {
                    _activeWeapon.StartFire();
                }

                break;
        }
    }

    private void UpdateWeaponSwitching()
    {
        int switchInput = _inputManager.GetSwitchWeaponInput();
        if (switchInput != 0)
        {
            SwitchWeapon(switchInput);
        }
    }
}