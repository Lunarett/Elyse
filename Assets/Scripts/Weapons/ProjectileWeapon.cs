using UnityEngine;
using Photon.Pun;

public class ProjectileWeapon : WeaponBase
{
    [Header("Projectile Properties")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed = 100f;
    [SerializeField] private float _projectileMaxDistance = 100f;
    [SerializeField] private float _projectileDownForce = 0.0f;
    [SerializeField] private ParticleSystem _impactEffectPrefab;

    protected override void Fire(Vector3 position, Vector3 direction)
    {
        _photonView.RPC(nameof(RPC_Fire), RpcTarget.All, position, direction);
    }

    [PunRPC]
    private void RPC_Fire(Vector3 position, Vector3 direction)
    {
        GameObject projectileObject = PhotonNetwork.Instantiate(_projectilePrefab.name, position, Quaternion.LookRotation(direction));
        ProjectileBase projectileBaseScript = projectileObject.GetComponent<ProjectileBase>();

        projectileBaseScript.Damage = _damage;
        projectileBaseScript.Speed = _projectileSpeed;
        projectileBaseScript.MaxDistance = _projectileMaxDistance;
        projectileBaseScript.HitLayers = _hitLayers;
        projectileBaseScript.ImpactEffectPrefab = _impactEffectPrefab;
        projectileBaseScript.DamageInfo = _info;
    
        _muzzleEffect.Play();
    }
}