using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PointDamageProjectile : ProjectileBase
{
    private bool _isDestroyed = false;
    
    // protected override void HandleImpact(Vector3 position, Vector3 normal, Collider collider)
    // {
    //     string localIdentifier = collider.gameObject.name; // Example, use any unique identifier
    //     int parentViewID = collider.transform.root.GetComponent<PhotonView>().ViewID;
    //
    //     photonView.RPC(nameof(RPC_ApplyDamage), RpcTarget.All, parentViewID, localIdentifier, Damage, DamageInfo);
    //     photonView.RPC(nameof(RPC_SpawnImpactEffects), RpcTarget.All, position, normal);
    // }


    [PunRPC]
    private void RPC_ApplyDamage(int parentViewID, string localIdentifier, float damage, DamageCauserInfo damageInfo)
    {
        PhotonView parentPV = PhotonView.Find(parentViewID);
        if (parentPV != null)
        {
            Transform targetTransform = parentPV.transform.Find(localIdentifier); // Or any other method to find the child
            if (targetTransform != null)
            {
                BodyDamageMultiplier bodyDamageMultiplier = targetTransform.GetComponent<BodyDamageMultiplier>();
                if (bodyDamageMultiplier != null)
                {
                    bodyDamageMultiplier.TakeDamage(damage, damageInfo);
                }
            }
            else
            {
                Debug.LogError("Target object not found for damage application.");
            }
        }
        else
        {
            Debug.LogError("Parent object with PhotonView ID " + parentViewID + " not found.");
        }
    }

    
    [PunRPC]
    private void RPC_SpawnImpactEffects(Vector3 position, Vector3 normal)
    {
        SpawnImpactEffect(position, normal);
        PhotonNetwork.Destroy(gameObject);
    }

    private void SafeDestroyProjectile()
    {
        if (_isDestroyed) return;

        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
            _isDestroyed = true;
        }
        else
        {
            // Request the MasterClient to destroy this object
            photonView.RPC(nameof(RequestMasterClientDestroy), RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void RequestMasterClientDestroy()
    {
        if (_isDestroyed) return;

        // Only the MasterClient will execute this
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
            _isDestroyed = true;
        }
    }
}