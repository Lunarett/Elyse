using System.IO;
using Photon.Pun;
using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [SerializeField] private bool _enableFallDamage = true;
    [SerializeField][FilePath] private string _deathIconPath;
    
    [Header("Fall Damage Properties")]
    [SerializeField] private float _minSpeed = 10.0f;
    [SerializeField] private float _maxSpeed = 30.0f;
    [SerializeField] private float _minFallDamage = 10.0f;
    [SerializeField] private float _maxFallDamage = 50.0f;
    
    private PlayerMovement _playerMovement;
    private PlayerHealth _playerHealth;
    private DamageCauserInfo _info;
    private PhotonView _photonView;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerHealth = GetComponent<PlayerHealth>();
        _photonView = GetComponent<PhotonView>();

        _info = new DamageCauserInfo(_photonView.Owner);
    }

    private void Update()
    {
        if (!_playerMovement.IsGrounded || _playerMovement.WasGrounded) return;
        float fallSpeed = -Mathf.Min(_playerMovement.CharacterVelocity.y, _playerMovement.LatestImpactSeed.y);
        float fallSpeedRatio = (fallSpeed - _minSpeed) / (_maxSpeed - _minSpeed);
        
        if (_enableFallDamage && fallSpeedRatio > 0f)
        {
            float dmgFromFall = Mathf.Lerp(_minFallDamage, _maxFallDamage, fallSpeedRatio);
            _playerHealth.TakeDamage(dmgFromFall, _info);

            // fall damage SFX
        }
        else
        {
            // land SFX
        }
    }
}
