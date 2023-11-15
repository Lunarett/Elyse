using System.Collections;
using UnityEngine;
using Photon.Pun;
using Pulsar.Debug;

public enum FireMode
{
    Auto,
    Single,
    Burst
}

public abstract class WeaponBase : MonoBehaviour
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

    protected PhotonView _photonView;
    protected Camera _playerCamera;
    protected DamageCauserInfo _info;
    private WeaponAnimation _weaponAnim;

    private float _lastBurstFireTime;
    private float _lastFireTime;

    public ElyseCharacter CharacterReference { get; set; }
    public Vector3 WeaponOffset => _weaponOffset;
    public FireMode FireModeType => _fireMode;

    protected virtual void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _info = new DamageCauserInfo(_photonView.Owner);
    }

    protected virtual void Start()
    {
        if (DebugUtils.CheckForNull(CharacterReference)) return;

        _playerCamera = CharacterReference.PlayerView.PlayerCamera.MainCamera;
        if (DebugUtils.CheckForNull(_playerCamera)) return;

        _weaponAnim = CharacterReference.WeaponAnim;
        if (DebugUtils.CheckForNull(_weaponAnim)) return;
    }

    public void StartFire()
    {
        if (!_photonView.IsMine) return;
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
        
        _weaponAnim.FireRecoil(_fireRecoilForce);
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
            _weaponAnim.FireRecoil(_fireRecoilForce);
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

    protected abstract void Fire(Vector3 position, Vector3 direction);
}
