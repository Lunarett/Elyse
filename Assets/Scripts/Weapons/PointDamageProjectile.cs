using UnityEngine;

public class PointDamageProjectile : ProjectileBase
{
    private bool _isDestroyed = false;
    
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