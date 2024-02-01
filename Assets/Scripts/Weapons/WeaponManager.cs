using System.Collections.Generic;
using UnityEngine;
using Pulsar.Debug;
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
    private List<WeaponBase> _weapons = new List<WeaponBase>();
    private WeaponAnimation _weaponAnimation;

    private void Awake()
    {
        _inputManager = GetComponent<PlayerInputManager>();
        if (DebugUtils.CheckForNull<PlayerInputManager>(_inputManager, $"WeaponManager: PlayerInputManager not attached to {gameObject.name}!")) return;

        _elyseCharacter = GetComponent<ElyseCharacter>();
        if (DebugUtils.CheckForNull<ElyseCharacter>(_elyseCharacter, $"WeaponManager: ElyseCharacter not attached to {gameObject.name}!")) return;

        _weaponAnimation = GetComponentInChildren<WeaponAnimation>();
    }

    void Start()
    {
        if (DebugUtils.CheckForNull<Transform>(_fpsSpawnLocation, "WeaponManager: FPS spawn point is missing!")) return;
        if (DebugUtils.CheckForNull<Transform>(_tpsSpawnLocation, "WeaponManager: TPS spawn point is missing!")) return;
        InitializeWeapons();
        _elyseCharacter.OnViewChanged += UpdateWeaponOnViewChanged;
    }

    private void InitializeWeapons()
    {
        foreach (var weaponPrefab in _weaponPrefabs)
        {
            WeaponBase weapon = Instantiate(weaponPrefab, _fpsSpawnLocation);
            weapon.gameObject.SetActive(false);
            _weapons.Add(weapon);
            SetupWeapon(weapon);
        }

        _currentWeaponIndex = _startingIndex;
        SwitchWeapon(0); // Activate the starting weapon
    }

    private void SetupWeapon(WeaponBase weapon)
    {
        Utils.SetLayerRecursively(weapon.gameObject, _elyseCharacter.ViewMode == EViewMode.FPS ? 
            LayerMask.NameToLayer("FP_Weapon") : 
            LayerMask.NameToLayer("TP_Weapon")
        );

        weapon.Pawn = _elyseCharacter;
        weapon.transform.localPosition = weapon.WeaponOffset;
        weapon.transform.localRotation = quaternion.identity;
        weapon.transform.localScale = Vector3.one;
        weapon.SetWeaponAnimation(_weaponAnimation);
    }

    private void Update()
    {
        if (_activeWeapon == null) return;
        UpdateWeaponFire();
        UpdateWeaponSwitching();
    }

    public void SwitchWeapon(int direction)
    {
        _weapons[_currentWeaponIndex].gameObject.SetActive(false);

        _currentWeaponIndex = (_currentWeaponIndex + direction + _weapons.Count) % _weapons.Count;
        _weapons[_currentWeaponIndex].gameObject.SetActive(true);

        _activeWeapon = _weapons[_currentWeaponIndex];
        UpdateWeaponOnViewChanged(_elyseCharacter.ViewMode);
    }

    private void UpdateWeaponOnViewChanged(EViewMode viewMode)
    {
        switch (viewMode)
        {
            case EViewMode.FPS:
                Utils.SetLayerRecursively(_activeWeapon.gameObject, LayerMask.NameToLayer("FP_Weapon"));
                Transform weaponTransformFPS;
                (weaponTransformFPS = _activeWeapon.transform).SetParent(_fpsSpawnLocation.transform);
                weaponTransformFPS.localPosition = _activeWeapon.WeaponOffset;
                weaponTransformFPS.localRotation = quaternion.identity;
                weaponTransformFPS.localScale = Vector3.one;
                break;
            case EViewMode.TPS:
                Utils.SetLayerRecursively(_activeWeapon.gameObject, LayerMask.NameToLayer("TP_Weapon"));
                Transform weaponTransformTPS;
                (weaponTransformTPS = _activeWeapon.transform).SetParent(_tpsSpawnLocation.transform);
                weaponTransformTPS.localPosition = _activeWeapon.WeaponOffset;
                weaponTransformTPS.localRotation = quaternion.identity;
                weaponTransformTPS.localScale = Vector3.one;
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
