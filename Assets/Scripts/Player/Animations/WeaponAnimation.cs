using System;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    [Header("Bobbing")]
    [SerializeField] private float BobFrequency = 10f;
    [SerializeField] private float MidAirFrequency = 2.0f;
    [SerializeField] private float BobSharpness = 10f;
    [SerializeField] private float DefaultBobAmount = 0.02f;

    [Header("Recoil Animation")]
    [SerializeField] private float RecoilSharpness = 50f;
    [SerializeField] private float MaxRecoilDistance = 0.5f;
    [SerializeField] private float RecoilRestitutionSharpness = 10f;

    private float AimingBobAmount = 0.02f;
    private Vector3 m_WeaponMainLocalPosition;
    private Vector3 m_WeaponBobLocalPosition;
    private Vector3 m_WeaponRecoilLocalPosition;
    private Vector3 m_AccumulatedRecoil;
    private bool IsAiming = false;
    private Vector3 m_LastCharacterPosition;
    private float m_WeaponBobFactor;

    private ElyseCharacter _elyseCharacter;

    private void Awake()
    {
        _elyseCharacter = transform.root.GetComponent<ElyseCharacter>();
    }

    private void Start()
    {
        m_LastCharacterPosition = transform.position;
    }

    private void Update()
    {
        UpdateWeaponBob();
        UpdateWeaponRecoil();
    }

    private void LateUpdate()
    {
        UpdateWeaponPosition();
    }

    private void UpdateWeaponBob()
    {
        if (!(Time.deltaTime > 0f)) return;
        
        Vector3 playerCharacterVelocity =
            (transform.position - m_LastCharacterPosition) / Time.deltaTime;

        float characterMovementFactor = 0f;
        characterMovementFactor = Mathf.Clamp01(playerCharacterVelocity.magnitude / 10f);

        m_WeaponBobFactor = Mathf.Lerp(m_WeaponBobFactor, characterMovementFactor, BobSharpness * Time.deltaTime);

        float bobAmount = IsAiming ? AimingBobAmount : DefaultBobAmount;
        float frequency = _elyseCharacter.PlayerMovement.IsGrounded ? BobFrequency : MidAirFrequency;
        float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * m_WeaponBobFactor;
        float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * m_WeaponBobFactor;

        m_WeaponBobLocalPosition.x = hBobValue;
        m_WeaponBobLocalPosition.y = Mathf.Abs(vBobValue);

        m_LastCharacterPosition = transform.position;
    }

    private void UpdateWeaponRecoil()
    {
        if (m_WeaponRecoilLocalPosition.z >= m_AccumulatedRecoil.z * 0.99f)
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, m_AccumulatedRecoil,
                RecoilSharpness * Time.deltaTime);
        }
        else
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, Vector3.zero,
                RecoilRestitutionSharpness * Time.deltaTime);
            m_AccumulatedRecoil = m_WeaponRecoilLocalPosition;
        }
    }

    private void UpdateWeaponPosition()
    {
        transform.localPosition = m_WeaponMainLocalPosition + m_WeaponBobLocalPosition + m_WeaponRecoilLocalPosition;
    }

    public void FireRecoil(float recoilForce)
    {
        m_AccumulatedRecoil += Vector3.back * recoilForce;
        m_AccumulatedRecoil = Vector3.ClampMagnitude(m_AccumulatedRecoil, MaxRecoilDistance);
    }
}