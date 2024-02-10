using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class RaycastWeapon : WeaponBase
{
    [Header("Raycast Properties")]
    [SerializeField] private ParticleSystem _impactEffectPrefab;
    [SerializeField] private LineRenderer _laserPrefab;
    [SerializeField] private float _raycastRange = 100f;
    [SerializeField] private float _lineRendererDuration = 0.1f;

    private LineRenderer _lr;

    protected override void Start()
    {
        base.Start();
        _lr = Instantiate(_laserPrefab, _fireTransform);
        HideLine();
    }
    
    protected override void Fire(Vector3 direction)
    {
        base.Fire(direction);
        
        Vector3 eyeFirePosition = _owner.PawnCameraController.MainCamera.transform.position;
        
        Debug.LogWarning($"EyeLocation: {_owner.PawnCameraController.MainCamera.transform.position}");
        
        Ray ray = new Ray(eyeFirePosition, direction);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, _raycastRange, _hitLayers))
        {
            BodyDamageMultiplier bodyDamageMultiplier = hitInfo.collider.GetComponent<BodyDamageMultiplier>();
            if (bodyDamageMultiplier != null)
            {
                bodyDamageMultiplier.ApplyDamage(_damageInfo);
            }
            
            PlayImpactEffect(hitInfo.point);
        }
        
        Vector3 lineEndPoint = hitInfo.point == Vector3.zero ? _fireTransform.position + direction * _raycastRange : hitInfo.point;
        PlayFireEffect(_fireTransform.position, lineEndPoint);
    }
    
    private void PlayFireEffect(Vector3 start, Vector3 end)
    {
        _muzzleEffect.Play();
        _lr.SetPosition(0, start);
        _lr.SetPosition(1, end);
        _lr.enabled = true;
        Invoke(nameof(HideLine), _lineRendererDuration);
    }

    private void PlayImpactEffect(Vector3 pos)
    {
        ParticleSystem impactEffect = Instantiate(_impactEffectPrefab, pos, quaternion.identity);
        impactEffect.Play();
        StartCoroutine(DestroyParticleAfterSeconds(impactEffect, 1.0f));
    }

    private IEnumerator DestroyParticleAfterSeconds(ParticleSystem particleEffect, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(particleEffect);
    }

    private void HideLine()
    {
        _lr.enabled = false;
    }
}