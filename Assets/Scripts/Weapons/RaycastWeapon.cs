using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;

public class RaycastWeapon : WeaponBase
{
    [Header("Raycast Properties")]
    [SerializeField] private ParticleSystem _impactEffectPrefab;
    [SerializeField] private LineRenderer _laserPrefab;
    [SerializeField] private float _raycastRange = 100f;
    [SerializeField] private float _lineRendererDuration = 0.1f;

    private LineRenderer _lr;
    private ParticleSystem _impactEffect;
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _lr = Instantiate(_laserPrefab, _fireTransform);
    }
    
    protected override void Fire(Vector3 position, Vector3 direction)
    {
        _photonView.RPC(nameof(RPC_Fire), RpcTarget.All, position, direction);
    }

    [PunRPC]
    private void RPC_Fire(Vector3 position, Vector3 direction)
    {
        Vector3 eyeFirePosition = _playerCamera.transform.position;
        Ray ray = new Ray(eyeFirePosition, direction);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, _raycastRange, _hitLayers))
        {
            BodyDamageMultiplier bodyDamageMultiplier = hitInfo.collider.GetComponent<BodyDamageMultiplier>();
            if (bodyDamageMultiplier != null)
            {
                bodyDamageMultiplier.TakeDamage(_damage, _info);
            }
        }

        Vector3 lineEndPoint = hitInfo.point == Vector3.zero ? position + direction * _raycastRange : hitInfo.point;
        PlayFireEffect(position, lineEndPoint);
    }
    
    private void PlayFireEffect(Vector3 start, Vector3 end)
    {
        _impactEffect = Instantiate(_impactEffectPrefab, end, quaternion.identity);
        
        _muzzleEffect.Play();
        _impactEffect.Play();

        _lr.SetPosition(0, start);
        _lr.SetPosition(1, end);
        _lr.enabled = true;
        Invoke(nameof(HideLine), _lineRendererDuration);
        Invoke(nameof(DestroyImpactParticle), 0.5f);
    }

    private void HideLine()
    {
        _lr.enabled = false;
    }

    private void DestroyImpactParticle()
    {
        if(_impactEffect != null) Destroy(_impactEffect.gameObject);   
    }
}
