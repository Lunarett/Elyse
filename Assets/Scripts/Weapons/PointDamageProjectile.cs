using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PointDamageProjectile : ProjectileBase
{
    protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    {
        BodyDamageMultiplier bodyDamageMultiplier = collider.GetComponent<BodyDamageMultiplier>();

        if (bodyDamageMultiplier != null)
        {
            photonView.RPC(nameof(RPC_ApplyDamage), RpcTarget.All, collider.gameObject.name, Damage, DamageInfo);
        }

        photonView.RPC(nameof(RPC_SpawnImpactEffects), RpcTarget.All, position, normal);
    }
    
    [PunRPC]
    private void RPC_SpawnImpactEffects(Vector3 position, Vector3 normal)
    {
        SpawnImpactEffect(position, normal);
        SafeDestroyProjectile();
    }


    [PunRPC]
    private void RPC_ApplyDamage(string partName, float damage, DamageCauserInfo damageInfo)
    {
        GameObject part = GameObject.Find(partName);
        if (part != null)
        {
            BodyDamageMultiplier bodyDamageMultiplier = part.GetComponent<BodyDamageMultiplier>();
            if (bodyDamageMultiplier != null)
            {
                bodyDamageMultiplier.TakeDamage(damage, damageInfo);
            }
        }
        
        SafeDestroyProjectile();
    }

    private void SafeDestroyProjectile()
    {
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}