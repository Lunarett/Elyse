using System;
using System.Collections;
using UnityEngine;
using FMODUnity;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public enum FireMode
{
    Single,
    Automatic,
    Burst
}

public class WeaponBase : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Sprite _weaponIcon;
    [SerializeField] private string _weaponName = "Weapon";
    [SerializeField] private Vector3 _weaponOffset;

    [Header("Ammo Properties")]
    [SerializeField] private AmmoType _ammoType;
    [SerializeField] private int _clipSize = 10;
    [Space]
    [SerializeField] private bool _autoReload;
    [SerializeField] private float _reloadDelay = 1.0f;

    [Header("Damage Properties")]
    [SerializeField] protected float _damage = 10.0f;
    [SerializeField] protected LayerMask _hitLayers;

    [Header("Firing Properties")]
    [SerializeField] private FireMode _fireMode;
    [Space]
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private float _recoilAmount = 1.0f;
    [SerializeField] private float _maxSpreadAngle = 5f;
    [SerializeField] private float _burstFireRate = 0.1f;
    [SerializeField] private int _burstShotCount = 3;

    [Header("Fire Effects")]
    [SerializeField] private float _fireRecoilForce = 1.0f;
    [SerializeField] protected Transform _fireTransform;
    [SerializeField] protected ParticleSystem _muzzleEffect;

    [Header("Audio")]
    [SerializeField] private EventReference _fireEventRef;
    [SerializeField] private EventReference _reloadEventRef;

    protected Pawn _owner;
    protected WeaponManager _weaponManager;

    protected Camera _playerCamera;
    protected DamageCauseInfo _damageInfo;
    
    private float _lastFireTime;
    private int _currentAmmo;
    private bool _isReloading = false;

    public event Action OnWeaponFire;
    public event Action OnWeaponReloadStarted;
    public event Action OnWeaponReloadEnded;
    
    private FMOD.Studio.EventInstance _weaponSoundInstance;

    public bool IsReloading => _isReloading;
    public Vector3 WeaponOffset => _weaponOffset;
    public FireMode FireMode => _fireMode;
    public string Name => _weaponName;
    public float RecoilForce => _fireRecoilForce;
    public float ReloadDelay => _reloadDelay;
    public int CurrentAmmo => _currentAmmo;
    public int ClipSize => _clipSize;
    public Sprite Icon => _weaponIcon;
    public AmmoType AmmoType => _ammoType;

    protected virtual void Awake()
    {
        _weaponSoundInstance = RuntimeManager.CreateInstance(_fireEventRef);
        _currentAmmo = _clipSize;
    }

    protected virtual void Start()
    {
    }

    protected void OnDisable()
    {
        _isReloading = false;
    }

    public void InitializeWeaponProps(Pawn owner, WeaponManager weaponManager)
    {
        _owner = owner;
        _playerCamera = owner.PawnCameraController.MainCamera;
        _weaponManager = weaponManager;

        _damageInfo = new DamageCauseInfo
        {
            damage = _damage,
            CauseOfDeath = ECauseOfDeath.KilledByPlayer,
            causer = _owner
        };
    }

    public void StartFire()
    {
        if (_currentAmmo <= 0 && _autoReload && CanReload()) StartCoroutine(Reload());
        if (_isReloading || _currentAmmo <= 0) return;
        
        switch (_fireMode)
        {
            case FireMode.Single:
            case FireMode.Automatic:
                TryFire();
                break;
            case FireMode.Burst:
                if(_currentAmmo >= _burstShotCount) StartCoroutine( BurstFire());
                break;
        }
    }

    public void StartReload()
    {
        if (_weaponManager == null)
        {
            Debug.LogError("WeaponBase: WeaponManager  is null!");
            return;
        }

        if (CanReload()) StartCoroutine(Reload());
    }

    private void TryFire()
    {
        if (Time.time < _lastFireTime + _fireRate) return;
        _lastFireTime = Time.time;
        _currentAmmo--;
        Fire(CalculateSpreadDirection());
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < _burstShotCount && _currentAmmo > 0; i++)
        {
            _currentAmmo--;
            if (CanFire()) Fire(CalculateSpreadDirection());
            yield return new WaitForSeconds(_burstFireRate);
        }
    }

    protected virtual void Fire(Vector3 direction)
    {
        PlayFireSound();
        float rightRecoilAmount = _recoilAmount;
        Vector3 recoilDirection = new Vector3(Random.Range(-rightRecoilAmount, rightRecoilAmount), _recoilAmount, 0); 
        _owner.PawnCameraController.AddDirectionalForce(recoilDirection);
        
        OnWeaponFire?.Invoke();
    }

    private IEnumerator Reload()
    {
        _isReloading = true;
        PlayReloadSound();
        
        OnWeaponReloadStarted?.Invoke();
    
        // Wait for the reload animation duration before proceeding with the ammo logic
        yield return new WaitForSeconds(_reloadDelay);

        int ammoNeeded = _clipSize - _currentAmmo;
        if (_weaponManager.AmmoManager.UseAmmo(_ammoType, ammoNeeded))
        {
            _currentAmmo = _clipSize;
        }
        else
        {
            int availableAmmo = _weaponManager.AmmoManager.CheckAmmo(_ammoType);
            _currentAmmo += availableAmmo;
            _weaponManager.AmmoManager.UseAmmo(_ammoType, availableAmmo);
        }

        _isReloading = false;
        OnWeaponReloadEnded?.Invoke();
    }
    
    protected bool CanFire()
    {
        return !_isReloading && _currentAmmo < _clipSize;
    }

    protected bool CanReload()
    {
        return !_isReloading &&
               _weaponManager.AmmoManager.CheckAmmo(_ammoType) > 0 &&
               _currentAmmo < _clipSize;
    }
    
    protected Vector3 CalculateSpreadDirection()
    {
        float spreadAngleX = Random.Range(-_maxSpreadAngle, _maxSpreadAngle);
        float spreadAngleY = Random.Range(-_maxSpreadAngle, _maxSpreadAngle);
        Vector3 direction = Quaternion.Euler(spreadAngleX, spreadAngleY, 0) * _playerCamera.transform.forward;
        return direction;
    }

    private void PlayFireSound()
    {
        RuntimeManager.PlayOneShot(_fireEventRef, transform.position);
    }

    private void PlayReloadSound()
    {
        RuntimeManager.PlayOneShot(_reloadEventRef, transform.position);
    }
}
