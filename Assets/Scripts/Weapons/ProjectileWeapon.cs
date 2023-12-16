using UnityEngine;

public class ProjectileWeapon : WeaponBase
{
    [Header("Projectile Properties")]
    [SerializeField] private ProjectileBase _projectilePrefab;
    [SerializeField] private float _projectileSpeed = 100f;
    [SerializeField] private float _projectileMaxDistance = 100f;
    [SerializeField] private float _projectileDownForce = 0.0f;
    [SerializeField] private ParticleSystem _impactEffectPrefab;

    protected override void Fire(Vector3 position, Vector3 direction)
    {
        base.Fire(position, direction);
        
        ProjectileBase projectile = Instantiate(_projectilePrefab, position, Quaternion.LookRotation(direction));
        projectile.Damage = _damage;
        projectile.Speed = _projectileSpeed;
        projectile.MaxDistance = _projectileMaxDistance;
        projectile.HitLayers = _hitLayers;
        projectile.ImpactEffectPrefab = _impactEffectPrefab;
        
        _muzzleEffect.Play();
    }
}