using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(Power))]
public class Jetpack : MonoBehaviour
{
    [SerializeField] private float jetpackForce = 10f;
    [SerializeField] private float powerConsumptionRate = 1f;

    private PlayerMovement playerMovement;
    private Power power;
    private PlayerInputManager _inputManager;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        power = GetComponent<Power>();
        _inputManager = GetComponent<PlayerInputManager>();
    }

    private void Update()
    {
        if (!_inputManager.GetFlyInputHeld() || !power.ConsumePower(powerConsumptionRate * Time.deltaTime)) return;
        Vector3 jetpackBoost = Vector3.up * jetpackForce * Time.deltaTime;
        playerMovement.CharacterVelocity += jetpackBoost;
    }
}