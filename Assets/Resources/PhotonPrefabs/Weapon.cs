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

public struct WeaponInfo
{
    public Photon.Realtime.Player WeaponOwner;
    public float BaseDamage;
}

public class Weapon : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Vector3 _weaponOffset;

    [Header("Damage Properties")]
    [SerializeField] private float _damage = 10.0f;
    [SerializeField] private LayerMask _hitLayers;

    [Header("Firing Properties")]
    [SerializeField] private FireMode _fireMode;
    [Space]
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private float _maxSpreadAngle = 5f;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _burstFireRate = 0.1f;
    [SerializeField] private int _burstShotCount = 3;

    [Header("Fire Effects")]
    [SerializeField] private float _fireRecoilForce = 1.0f;
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _muzzleEffect;
    [SerializeField] private float _lineRendererDuration = 0.1f;

    private PhotonView _photonView;
    private WeaponAnimation _weaponAnim;
    private Camera _playerCamera;
    private LineRenderer _lineRenderer;
    private WeaponInfo _info;

    private float _lastBurstFireTime;
    private float _lastFireTime;

    public ElyseCharacter CharacterReference { get; set; }
    public Vector3 WeaponOffset => _weaponOffset;
    public FireMode FireModeType => _fireMode;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _lineRenderer = GetComponent<LineRenderer>();

        _info = new WeaponInfo()
        {
            WeaponOwner = _photonView.Owner,
            BaseDamage = _damage
        };
    }

    private void Start()
    {
        if (DebugUtils.CheckForNull(CharacterReference)) return;

        _playerCamera = CharacterReference.PlayerView.PlayerCamera.MainCamera;
        if (DebugUtils.CheckForNull(_playerCamera)) return;

        _weaponAnim = CharacterReference.WeaponAnim;
        if (DebugUtils.CheckForNull(_weaponAnim)) return;

        HideLine();
    }

    public void Fire()
    {
        if (!_photonView.IsMine) return;

        Vector3 firePosition = _fireTransform.position;

        switch (_fireMode)
        {
            case FireMode.Auto:
            case FireMode.Single:
                TryFire(firePosition);
                break;
            case FireMode.Burst:
                TryBurstFire(firePosition);
                break;
            default:
                Debug.LogError("Unsupported fire mode!");
                break;
        }
    }

    private void TryFire(Vector3 firePosition)
    {
        if (!(Time.time >= _lastFireTime + _fireRate)) return;
        _lastFireTime = Time.time;

        Quaternion spreadRotation = Quaternion.Euler(
            UnityEngine.Random.Range(-_maxSpreadAngle, _maxSpreadAngle),
            UnityEngine.Random.Range(-_maxSpreadAngle, _maxSpreadAngle),
            0f
        );

        Vector3 finalFireDirection = spreadRotation * _playerCamera.transform.forward;

        _photonView.RPC("RPC_Fire", RpcTarget.All, firePosition, finalFireDirection);
    }

    private void TryBurstFire(Vector3 firePosition)
    {
        if (!(Time.time >= _lastBurstFireTime + _burstFireRate)) return;
        _lastBurstFireTime = Time.time;
        //StartCoroutine(BurstFire(firePosition));
    }

    private IEnumerator BurstFire(Vector3 firePosition)
    {
        for (int i = 0; i < _burstShotCount; i++)
        {
            //_photonView.RPC("RPC_Fire", RpcTarget.All, firePosition);
            yield return new WaitForSeconds(_burstFireRate);
        }
    }

    [PunRPC]
    private void RPC_Fire(Vector3 firePosition, Vector3 finalFireDirection)
    {
        Vector3 eyeFirePosition = _playerCamera.transform.position;
        Ray ray = new Ray(eyeFirePosition, finalFireDirection);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, _range, _hitLayers))
        {
            BodyDamageMultiplier bodyDamageMultiplier = hitInfo.collider.GetComponent<BodyDamageMultiplier>();
            if (bodyDamageMultiplier != null)
            {
                bodyDamageMultiplier.TakeDamage(_info.BaseDamage, _info);
            }
        }

        PlayFireEffect(_fireTransform.position, _fireTransform.position + finalFireDirection * _range);
    }

    private void PlayFireEffect(Vector3 start, Vector3 end)
    {
        _weaponAnim.FireRecoil(_fireRecoilForce);
        _muzzleEffect.Play();

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
        _lineRenderer.enabled = true;
        Invoke(nameof(HideLine), _lineRendererDuration);
    }

    private void HideLine()
    {
        _lineRenderer.enabled = false;
    }
}
