using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _fireLocation;
    [SerializeField] private ParticleSystem _imactEffect;
    [SerializeField] private ParticleSystem _muzzleEffect;

    [Header("Weapon Info")]
    [SerializeField] private WeaponInfo _weaponInfo;

    [Header("Fire Properties")]
    [SerializeField] private bool _singleFire = false;
    [SerializeField] private float _fireRate = 600.0f;
    [Space]
    [SerializeField] private Vector3 _offset;

    public Vector3 Offset => _offset;
}
