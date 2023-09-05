using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Plusar.Core;
using Plusar.Player;
using UnityEngine;

[System.Serializable]
public struct WeaponInfo
{
    public string name;
    public float damage;
}

public class WeaponBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _fireLocation;
    [SerializeField] private GameObject _impactPrefab;

    [Header("Weapon Info")]
    [SerializeField] private WeaponInfo _weaponInfo;

    [Header("Fire Properties")]
    [SerializeField] private float _fireDistance = 1000.0f;
    [SerializeField] private bool _singleFire = false;
    [SerializeField] private float _fireRate = 600.0f;
    [Space]
    [SerializeField] private Vector3 _offset;

    public Vector3 Offset => _offset;
    public Plusar.Player.PlayerController PlayerController { get; set; }

    private PhotonView _pv;
    private WeaponAnimation _weaponAnim;

    private void Awake()
    {
        _weaponAnim = GetComponent<WeaponAnimation>();
        _pv = GetComponent<PhotonView>();
        if (!_pv || _pv.ViewID <= 0)
        {
            Debug.LogError("Invalid PhotonView on weapon");
        }
    }

    private void Start()
    {
        if(PlayerController != null)
        {
            _weaponAnim.PlayerController = PlayerController;
        }
        else
        {
            Debug.LogError("pc is null");
        }
    }

    public void Fire()
    {
        Ray ray = PlayerController.PlayerView.WeaponCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = PlayerController.PlayerView.WeaponCamera.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health)
            {
                int targetViewID = health.GetComponent<PhotonView>().ViewID;
                _pv.RPC("RPC_ApplyDamage", RpcTarget.All, _weaponInfo.damage, targetViewID);
            }
            _pv.RPC("RPC_Fire", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    private void RPC_Fire(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(_impactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * _impactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }

    [PunRPC]
    private void RPC_ApplyDamage(float damage, int targetViewID)
    {
        PhotonView targetView = PhotonView.Find(targetViewID);
        Health health = targetView.GetComponent<Health>();
        if (health)
        {
            health.Damage(damage);
        }
    }
}
