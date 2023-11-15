using UnityEngine;
using Photon.Pun;

public class PointDamageProjectile : ProjectileBase
{
    protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        PhotonView targetView = collider.gameObject.GetPhotonView();

        if (targetView != null)
        {
            photonView.RPC(nameof(ApplyDamage), RpcTarget.All, targetView.ViewID, DamageInfo);
        }
        else
        {
            SpawnImpactEffect(position, normal);
            Destroy(gameObject);
        }
    }

    [PunRPC]
    private void ApplyDamage(int viewID, DamageCauserInfo damageInfo)
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