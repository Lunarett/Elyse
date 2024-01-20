using UnityEngine;

public class RadialDamageProjectile : ProjectileBase
{
    [SerializeField] private float _blastRadius = 5f;
    [SerializeField] private AnimationCurve _damageFalloffCurve;

    protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        base.HandleImpact(position, normal, collider);
        
        BodyDamageMultiplier bodyDamageMultiplier = collider.GetComponent<BodyDamageMultiplier>();
        if (bodyDamageMultiplier != null)
        {
            bodyDamageMultiplier.ApplyDamage(DamageInfo);
        }
        
        Destroy(gameObject);
    }
}