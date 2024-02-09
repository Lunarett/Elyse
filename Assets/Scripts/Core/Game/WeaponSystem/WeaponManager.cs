using System;
using System.Collections.Generic;
using Pulsar.Utils;
using UnityEngine;

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

    private Dictionary<string, WeaponBase> _weapons = new Dictionary<string, WeaponBase>();
    private List<string> _weaponOrder = new List<string>();
    private WeaponBase _activeWeapon;
    private Pawn _owner;
    private AmmoManager _ammoManager;
    
    private int _activeWeaponIndex = 0;
    private float _lastSwitchTime = -1f;
    private EViewMode _viewMode;

    public IReadOnlyDictionary<string, WeaponBase> Weapons => _weapons;
    public IReadOnlyList<string> WeaponOrder => _weaponOrder;

    public float SwitchCooldown => _switchCooldown;
    
    public Pawn Owner => _owner;
    public AmmoManager AmmoManager => _ammoManager;
    public WeaponBase ActiveWeapon => _activeWeapon;

    public event Action<WeaponBase> OnWeaponAdded;
    public event Action<string> OnWeaponRemoved;
    public event Action<WeaponBase> OnWeaponSwitched;
    public event Action<WeaponBase> OnAddedAmmo;
    
    private void Awake()
    {
        _ammoManager = new AmmoManager(_startingAmmo);
        _owner = GetComponent<Pawn>();
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        foreach (var weaponPrefab in _weaponPrefabs)
        {
            AddWeapon(weaponPrefab);
        }

        if (_weapons.Count <= 0) return;
        _activeWeaponIndex = Mathf.Clamp(_weaponStartingIndex, 0, _weaponOrder.Count - 1);
        SwitchToWeaponById(_weaponOrder[_activeWeaponIndex]);
    }

    public void AddWeapon(WeaponBase weaponPrefab)
    {
        if (_weapons.ContainsKey(weaponPrefab.Name))
        {
            Debug.LogError($"WeaponManager: Weapon with ID {weaponPrefab.Name} already exists.");
            return;
        }

        WeaponBase newWeapon = Instantiate(weaponPrefab, GetWeaponSpawnTransform(_viewMode));
        newWeapon.gameObject.SetActive(false);
        _weapons.Add(newWeapon.Name, newWeapon);
        _weaponOrder.Add(newWeapon.Name);
        newWeapon.InitializeWeaponProps(_owner, this);
        
        OnWeaponAdded?.Invoke(newWeapon);
    }
    
    public void RemoveWeapon(string weaponId)
    {
        if (!_weapons.TryGetValue(weaponId, out WeaponBase weapon)) return;
        if (_activeWeapon == weapon)
        {
            int nextIndex = (_weaponOrder.IndexOf(weaponId) + 1) % _weaponOrder.Count;
            SwitchWeapon(nextIndex == 0 ? 1 : nextIndex - _weaponOrder.IndexOf(weaponId));
        }
        
        OnWeaponRemoved?.Invoke(weaponId);
        
        _weapons.Remove(weaponId);
        _weaponOrder.Remove(weaponId);
        Destroy(weapon.gameObject);
    }
    
    public bool HasWeapon(string weaponId)
    {
        return _weapons.ContainsKey(weaponId);
    }
    
    public void SwitchWeapon(int direction)
    {
        if (Time.time < _lastSwitchTime + _switchCooldown || _weaponOrder.Count <= 1)
        {
            return;
        }

        _activeWeaponIndex = (_activeWeaponIndex + direction + _weaponOrder.Count) % _weaponOrder.Count;
        SwitchToWeaponById(_weaponOrder[_activeWeaponIndex]);
        
        _lastSwitchTime = Time.time;
    }

    public void SwitchToWeaponById(string weaponId)
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.gameObject.SetActive(false);
        }

        _activeWeapon = _weapons[weaponId];
        _activeWeapon.gameObject.SetActive(true);
        OnWeaponSwitched?.Invoke(_activeWeapon);
        UpdateWeaponOnViewChanged(_viewMode);
    }

    private void UpdateWeaponOnViewChanged(EViewMode viewMode)
    {
        if (_activeWeapon == null) return;

        int targetLayer = viewMode == EViewMode.FPS ? LayerMaskToLayer(_firstPersonLayer) : LayerMaskToLayer(_thirdPersonLayer);
        Utils.SetLayerRecursively(_activeWeapon.gameObject, targetLayer);

        Transform weaponTransform = _activeWeapon.transform;
        weaponTransform.SetParent(viewMode == EViewMode.FPS ? _fpsSpawnTransform : _tpsSpawnTransform);
        weaponTransform.localPosition = _activeWeapon.WeaponOffset;
        weaponTransform.localRotation = Quaternion.identity;
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

    public void SetViewMode(EViewMode viewMode)
    {
        _viewMode = viewMode;
        UpdateWeaponOnViewChanged(viewMode);
    }
}