using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(Power))]
public class Jetpack : MonoBehaviour
{
    [Header("Jetpack Settings")]
    [SerializeField] private float _thrustPower = 4.0f;
    [SerializeField] private float _jetpackForce = 10f;
    [SerializeField] private float _powerConsumptionRate = 1f;

    [Header("Sound Settings")]
    [SerializeField] private EventReference jetpackSoundEvent;

    private PlayerMovement _playerMovement;
    private Power power;
    private PlayerInputManager _inputManager;
    private FMOD.Studio.EventInstance jetpackSoundInstance;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        power = GetComponent<Power>();
        _inputManager = GetComponent<PlayerInputManager>();

        // Create an instance of the jetpack sound
        jetpackSoundInstance = RuntimeManager.CreateInstance(jetpackSoundEvent);
        
        // Attach the sound instance to this GameObject for 3D sound positioning
        RuntimeManager.AttachInstanceToGameObject(jetpackSoundInstance, transform, GetComponent<Rigidbody>());
    }

    private void Update()
    {
        if (!_inputManager.GetFlyInputHeld())
        {
            StopJetpackSound();
            return;
        }

        float powerNeeded = _powerConsumptionRate * Time.deltaTime;
        if (!power.ConsumePower(powerNeeded))
        {
            StopJetpackSound();
            return;
        }

        PlayJetpackSound();

        if (_playerMovement.IsGrounded)
        {
            _playerMovement.CharacterVelocity = new Vector3(_playerMovement.CharacterVelocity.x, 0f, _playerMovement.CharacterVelocity.z);
            _playerMovement.CharacterVelocity += Vector3.up * _thrustPower;
        }
        
        Vector3 jetpackBoost = Vector3.up * (_jetpackForce * Time.deltaTime);
        _playerMovement.CharacterVelocity += jetpackBoost;
    }

    private void PlayJetpackSound()
    {
        // Start playing the sound if it's not already playing
        FMOD.ATTRIBUTES_3D attributes = gameObject.To3DAttributes();
        jetpackSoundInstance.set3DAttributes(attributes);
        FMOD.Studio.PLAYBACK_STATE playbackState;
        jetpackSoundInstance.getPlaybackState(out playbackState);
        if (playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            jetpackSoundInstance.start();
        }
    }

    private void StopJetpackSound()
    {
        // Stop the sound if it's playing
        FMOD.Studio.PLAYBACK_STATE playbackState;
        jetpackSoundInstance.getPlaybackState(out playbackState);
        if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            jetpackSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDestroy()
    {
        // Release the FMOD sound instance when the object is destroyed
        jetpackSoundInstance.release();
    }
}
