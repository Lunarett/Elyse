using System.Collections.Generic;
using UnityEngine;
using Pulsar.Utils;
using Unity.Mathematics;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon View Layers")]
    [SerializeField] private LayerMask _firstPersonLayer;
    [SerializeField] private LayerMask _thirdPersonLayer;
    
    [Header("Weapon Spawn Properties")]
    [SerializeField] private Transform _fpsSpawnTransform;
    [SerializeField] private Transform _tpsSpawnTransform;
    
    [Header("Weapon Switching")]
    [SerializeField] private float _switchCooldown = 0.5f;

    [Header("Starter Inventory Properties")]
    [SerializeField] private int _weaponStartingIndex;
    [SerializeField] private List<WeaponBase> _weaponPrefabs;
    [Space]
    [SerializeField] private List<Ammo> _startingAmmo;

    private WeaponBase _activeWeapon;
    private Pawn _owner;
    private WeaponAnimation _weaponAnimation;
    private AmmoManager _ammoManager;
    
    private int _activeWeaponIndex;
    private float _lastSwitchTime = -1f;
    private EViewMode _viewMode;

    private readonly List<WeaponBase> _weapons = new List<WeaponBase>();

    public Pawn Owner => _owner;
    public AmmoManager AmmoManager => _ammoManager;
    public WeaponBase ActiveWeapon => _activeWeapon;
    public WeaponAnimation WeaponAnimation => _weaponAnimation;
    public int ActiveWeaponIndex => _activeWeaponIndex;
    
    private void Awake()
    {
        _ammoManager = new AmmoManager(_startingAmmo);
        
        _weaponAnimation = GetComponentInChildren<WeaponAnimation>();
        if (_weaponAnimation == null)
        {
            Debug.LogError("[WeaponManager] WeaponAnimation is missing on the children of the GameObject.");
        }

        _owner = GetComponent<Pawn>();
        if (_owner == null)
        {
            Debug.LogError("[WeaponManager] Pawn is missing on the GameObject.");
        }
    }

    private void Start()
    {
        // Initialize Weapons
        foreach (var weaponPrefab in _weaponPrefabs)
        {
            AddWeapon(weaponPrefab);
        }

        if (_weaponPrefabs.Count <= 0) return;
        _activeWeaponIndex = Mathf.Clamp(_weaponStartingIndex, 0, _weaponPrefabs.Count - 1);
        SwitchWeapon(0);
    }

    public void AddWeapon(WeaponBase weapon)
    {
        WeaponBase newWeapon = Instantiate(weapon, GetWeaponSpawnTransform(_viewMode));
        newWeapon.gameObject.SetActive(false);
        _weapons.Add(newWeapon);
        newWeapon.InitializeWeaponProps(_owner, this, _weaponAnimation);
    }
    
    public void SwitchWeapon(int direction)
    {
        if (Time.time < _lastSwitchTime + _switchCooldown)
        {
            return;
        }

        //_weaponAnimation.PlaySwitchAnimation(_switchCooldown);
        
        _lastSwitchTime = Time.time;
        int newWeaponIndex = (_activeWeaponIndex + direction + _weapons.Count) % _weapons.Count;

        if (newWeaponIndex == _activeWeaponIndex) return;
        // Deactivate the current weapon
        if (_activeWeapon != null)
        {
            _activeWeapon.gameObject.SetActive(false);
        }

        // Update the active weapon index to the new one
        _activeWeaponIndex = newWeaponIndex;
        _activeWeapon = _weapons[_activeWeaponIndex];

        // Activate the new weapon
        _activeWeapon.gameObject.SetActive(true);
        UpdateWeaponOnViewChanged(_viewMode);
    }


    public void SetViewMode(EViewMode viewMode)
    {
        _viewMode = viewMode;
        UpdateWeaponOnViewChanged(viewMode);
    }
    
    private void UpdateWeaponOnViewChanged(EViewMode viewMode)
    {
        if (_activeWeapon == null) return;

        int targetLayer = viewMode == EViewMode.FPS ? LayerMaskToLayer(_firstPersonLayer) : LayerMaskToLayer(_thirdPersonLayer);
        Utils.SetLayerRecursively(_activeWeapon.gameObject, targetLayer);

        Transform weaponTransform = _activeWeapon.transform;
        weaponTransform.SetParent(viewMode == EViewMode.FPS ? _fpsSpawnTransform : _tpsSpawnTransform);
        weaponTransform.localPosition = _activeWeapon.WeaponOffset;
        weaponTransform.localRotation = quaternion.identity;
        weaponTransform.localScale = Vector3.one;
    }
    
    private Transform GetWeaponSpawnTransform(EViewMode viewMode)
    {
        return viewMode == EViewMode.FPS ? _fpsSpawnTransform : _tpsSpawnTransform;
    }
    
    private int LayerMaskToLayer(LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }
}