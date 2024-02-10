using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RadialDamageProjectile : ProjectileBase
{
    [SerializeField] private float _blastRadius = 5f;

    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private Color _gizmoColor = Color.red;

    protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        base.HandleImpact(position, normal, collider);

        Collider[] hitColliders = Physics.OverlapSphere(position, _blastRadius);
        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(position, hitCollider.transform.position);
            distance = Mathf.Min(distance, _blastRadius);
            float damageMultiplier = 1 - (distance / _blastRadius);
            float damageToApply = DamageInfo.damage * damageMultiplier;

            HealthBase healthBase = hitCollider.GetComponent<HealthBase>();
            if (healthBase == null) continue;
            DamageInfo.damage = damageToApply;
            healthBase.ApplyDamage(DamageInfo);
        }
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireSphere(transform.position, _blastRadius);
    }
}