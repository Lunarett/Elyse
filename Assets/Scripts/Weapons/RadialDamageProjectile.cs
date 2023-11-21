using UnityEngine;
using Photon.Pun;

public class RadialDamageProjectile : ProjectileBase
{
    [SerializeField] private float _blastRadius = 5f;
    [SerializeField] private AnimationCurve _damageFalloffCurve;

    // protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    // {
    //     PhotonView targetView = collider.gameObject.GetPhotonView();
    //
    //     if (targetView != null)
    //     {
    //         photonView.RPC(nameof(ApplyRadialDamage), RpcTarget.All, targetView.ViewID, DamageInfo);
    //     }
    //     else
    //     {
    //         SpawnImpactEffect(position, normal);
    //         Destroy(gameObject);
    //     }
    // }

    [PunRPC]
    private void ApplyRadialDamage(int viewID, DamageCauserInfo damageInfo)
    {
        SpawnImpactEffect(transform.position, Vector3.zero);
        
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            BodyDamageMultiplier bodyDamageMultiplier = targetView.GetComponent<BodyDamageMultiplier>();
            if (bodyDamageMultiplier != null)
            {
                bodyDamageMultiplier.TakeDamage(Damage, damageInfo);
            }
        }

        Destroy(gameObject);
    }
}