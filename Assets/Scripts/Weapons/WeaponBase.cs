using System;
using System.Collections;
using UnityEngine;
using Pulsar.Debug;
using FMODUnity;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public enum FireMode
{
    Auto,
    Single,
    Burst
}

public class WeaponBase : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private string _weaponName;
    [SerializeField] private Vector3 _weaponOffset;

    [Header("Damage Properties")]
    [SerializeField] protected float _damage = 10.0f;
    [SerializeField] protected LayerMask _hitLayers;

    [Header("Firing Properties")]
    [SerializeField] private FireMode _fireMode;
    [Space]
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private float _maxSpreadAngle = 5f;
    [SerializeField] private float _burstFireRate = 0.1f;
    [SerializeField] private int _burstShotCount = 3;

    [Header("Fire Effects")]
    [SerializeField] private float _fireRecoilForce = 1.0f;
    [SerializeField] protected Transform _fireTransform;
    [SerializeField] protected ParticleSystem _muzzleEffect;

    [Header("SFX")]
    [SerializeField] private EventReference _fireEventRef; 

    protected Camera _playerCamera;
    protected DamageCauseInfo _damageInfo;
    private FMOD.Studio.EventInstance weaponSoundInstance;

    private float _lastBurstFireTime;
    private float _lastFireTime;
    private WeaponAnimation _weaponAnimation;

    public event Action<float> OnWeaponFired; 
    
    public Pawn Pawn { get; set; }
    public Vector3 WeaponOffset => _weaponOffset;
    public FireMode FireModeType => _fireMode;

    protected virtual void Awake()
    {
        weaponSoundInstance = RuntimeManager.CreateInstance(_fireEventRef);
    }

    protected virtual void Start()
    {
        if (DebugUtils.CheckForNull(Pawn)) return;
        _playerCamera = Pawn.GetComponentInChildren<SpringArm>().AttachedCamera;
        if (DebugUtils.CheckForNull(_playerCamera, "WeaponBase: SpringArm was not found")) return;

        _damageInfo = new DamageCauseInfo()
        {
            damage = _damage,
            causer = Pawn,
            CauseOfDeath = ECauseOfDeath.KilledByPlayer
        };
    }

    public void SetWeaponAnimation(WeaponAnimation weaponAnimation)
    {
        _weaponAnimation = weaponAnimation;
    }

    public void StartFire()
    {
        switch (_fireMode)
        {
            case FireMode.Auto:
            case FireMode.Single:
                TryFire();
                break;
            case FireMode.Burst:
                TryBurstFire();
                break;
            default:
                Debug.LogError("Unsupported fire mode!");
                break;
        }
    }

    private void TryFire()
    {
        if (!(Time.time >= _lastFireTime + _fireRate)) return;
        _lastFireTime = Time.time;

        // Trigger recoil animation
        _weaponAnimation.FireRecoil(_fireRecoilForce);

        OnWeaponFired?.Invoke(_fireRecoilForce);
        Vector3 finalFireDirection = GetFireSpreadDirection(_playerCamera.transform.forward);
        Fire(_fireTransform.position, finalFireDirection);
    }

    private void TryBurstFire()
    {
        if (!(Time.time >= _lastBurstFireTime + _fireRate)) return;
        _lastBurstFireTime = Time.time;
        StartCoroutine(BurstFire());
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < _burstShotCount; i++)
        {
            Vector3 direction = GetFireSpreadDirection(_playerCamera.transform.forward);
            _weaponAnimation?.FireRecoil(_fireRecoilForce);
            OnWeaponFired?.Invoke(_fireRecoilForce);
            Fire(_fireTransform.position, direction);
            yield return new WaitForSeconds(_burstFireRate);
        }
    }

    protected Vector3 GetFireSpreadDirection(Vector3 direction)
    {
        Quaternion spreadRotation = Quaternion.Euler(
            Random.Range(-_maxSpreadAngle, _maxSpreadAngle),
            Random.Range(-_maxSpreadAngle, _maxSpreadAngle),
            0f
        );

        return spreadRotation * direction;
    }

    protected virtual void Fire(Vector3 position, Vector3 direction)
    {
        FMOD.ATTRIBUTES_3D attributes = gameObject.To3DAttributes();
        weaponSoundInstance.set3DAttributes(attributes);
        weaponSoundInstance.start();
    }

    private void OnDestroy()
    {
        weaponSoundInstance.release();
    }
}
