using UnityEngine;

public class PointDamageProjectile : ProjectileBase
{
    protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        base.HandleImpact(position, normal, collider);
    }
}