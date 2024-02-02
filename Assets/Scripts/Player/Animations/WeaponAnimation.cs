using System.Collections;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    [Header("Bobbing")]
    [SerializeField] private float _bobFrequency = 10f;
    [SerializeField] private float _midAirFrequency = 2.0f;
    [SerializeField] private float _bobSharpness = 10f;
    [SerializeField] private float _defaultBobAmount = 0.02f;

    [Header("Recoil Animation")]
    [SerializeField] private float _recoilSharpness = 50f;
    [SerializeField] private float _maxRecoilDistance = 0.5f;
    [SerializeField] private float _recoilRestitutionSharpness = 10f;

    [Header("Reload Animation")]
    [SerializeField] private Vector3 _reloadOffset = new Vector3(0, -0.1f, 0);

    private Vector3 _initialLocalPosition;
    private Vector3 _weaponBobLocalPosition;
    private Vector3 _weaponRecoilLocalPosition;
    private Vector3 _accumulatedRecoil;
    private bool _isReloading = false;

    private Vector3 _lastCharacterPosition;
    private float _weaponBobFactor;
    private bool _isGrounded = true;

    private void Start()
    {
        _initialLocalPosition = transform.localPosition;
        _lastCharacterPosition = transform.position;
    }

    private void Update()
    {
        if (!_isReloading) // Skip bob and recoil updates during reload animation
        {
            UpdateWeaponBob();
            UpdateWeaponRecoil();
        }
    }

    private void LateUpdate()
    {
        if (!_isReloading) // Apply combined position adjustments only if not reloading
        {
            transform.localPosition = _initialLocalPosition + _weaponBobLocalPosition + _weaponRecoilLocalPosition;
        }
    }

    public void FireRecoil(float recoilForce)
    {
        _accumulatedRecoil += Vector3.back * recoilForce;
        _accumulatedRecoil = Vector3.ClampMagnitude(_accumulatedRecoil, _maxRecoilDistance);
    }

    public IEnumerator ReloadAnimation(float reloadDuration)
    {
        _isReloading = true;
        Vector3 startReloadingPosition = transform.localPosition;
        Vector3 targetReloadingPosition = _initialLocalPosition + _reloadOffset;

        float elapsedTime = 0;
        while (elapsedTime < reloadDuration)
        {
            float t = Mathf.Sin((elapsedTime / reloadDuration) * Mathf.PI); // Smooth in-out using sin
            transform.localPosition = Vector3.Lerp(startReloadingPosition, targetReloadingPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure weapon returns to its exact initial position
        transform.localPosition = _initialLocalPosition;
        _isReloading = false;
    }

    public void SetIsGrounded(bool isGrounded)
    {
        _isGrounded = isGrounded;
    }

    private void UpdateWeaponBob()
    {
        Vector3 playerVelocity = (transform.position - _lastCharacterPosition) / Time.deltaTime;
        _weaponBobFactor = Mathf.Lerp(_weaponBobFactor, Mathf.Clamp01(playerVelocity.magnitude / 10f), _bobSharpness * Time.deltaTime);

        float bobAmount = _defaultBobAmount;
        float frequency = _isGrounded ? _bobFrequency : _midAirFrequency;
        _weaponBobLocalPosition.x = Mathf.Sin(Time.time * frequency) * bobAmount * _weaponBobFactor;
        _weaponBobLocalPosition.y = Mathf.Abs(Mathf.Sin(Time.time * frequency * 2f) * bobAmount * _weaponBobFactor);

        _lastCharacterPosition = transform.position;
    }

    private void UpdateWeaponRecoil()
    {
        _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, _accumulatedRecoil, _recoilSharpness * Time.deltaTime);
        _accumulatedRecoil = Vector3.Lerp(_accumulatedRecoil, Vector3.zero, _recoilRestitutionSharpness * Time.deltaTime);
    }
}