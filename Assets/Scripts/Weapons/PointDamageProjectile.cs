using UnityEngine;

public class PointDamageProjectile : ProjectileBase
{
    private bool _isDestroyed = false;
    
    protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        base.HandleImpact(position, normal, collider);
        
        Debug.Log($"Projectile hit registered! object: {collider.gameObject.name}");

        BodyDamageMultiplier bodyDamageMultiplier = collider.GetComponent<BodyDamageMultiplier>();
        if (bodyDamageMultiplier != null)
        {
            bodyDamageMultiplier.TakeDamage(Damage);
        }

        Destroy(gameObject);
    }
}