using UnityEngine;

[RequireComponent(typeof(Power))]
public class Jetpack : MonoBehaviour
{
    [SerializeField] private float _thrustPower = 4.0f;
    [SerializeField] private float _jetpackForce = 10f;
    [SerializeField] private float _powerConsumptionRate = 1f;

    private PlayerMovement _playerMovement;
    private Power power;
    private PlayerInputManager _inputManager;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        power = GetComponent<Power>();
        _inputManager = GetComponent<PlayerInputManager>();
    }

    private void Update()
    {
        if (!_inputManager.GetFlyInputHeld()) return;
        float powerNeeded = _powerConsumptionRate * Time.deltaTime;
        if (!power.ConsumePower(powerNeeded)) return;

        if (_playerMovement.IsGrounded)
        {
            _playerMovement.CharacterVelocity = new Vector3(_playerMovement.CharacterVelocity.x, 0f, _playerMovement.CharacterVelocity.z);
            _playerMovement.CharacterVelocity += Vector3.up * _thrustPower;
        }
        
        Vector3 jetpackBoost = Vector3.up * (_jetpackForce * Time.deltaTime);
        _playerMovement.CharacterVelocity += jetpackBoost;
    }
}