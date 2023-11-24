using System;
using Photon.Pun;
using Pulsar.Debug;
using UnityEngine;

public enum EViewMode
{
    FPS,
    TPS
}

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerView : MonoBehaviour
{
    [Header("Camera References")] [SerializeField]
    private Transform m_cameraFollowTarget;

    [SerializeField] private Camera m_weaponCamera;

    [Header("Camera Settings")] [SerializeField]
    private float m_playerFOV = 60.0f;

    [SerializeField] private float m_weaponFOV = 60.0f;

    [Header("Spring Arm Settings")] [SerializeField]
    private Vector3 _damping;

    [SerializeField] private Vector3 _shoulderOffset = new Vector3(0.5f, 0.0f, 0.0f);
    [SerializeField] private float _cameraDistance = 3.0f;

    [Header("Camera Controls")] [Range(0.0f, 0.5f)] [SerializeField]
    private float m_lookSpeed = 0.1f;

    [Range(0.1f, 1.0f)] [SerializeField] private float AimingRotationMultiplier = 0.4f;
    [SerializeField] private Vector2 m_pitchClamp = new Vector2(-89.0f, 89.0f);

    private float m_CameraVerticalAngle;
    private bool m_isAiming;
    private PlayerCamera m_playerCamera;
    private PlayerInputManager m_inputManager;
    private float RotationMultiplier => m_isAiming ? AimingRotationMultiplier : 1f;

    public Transform CameraFollowTarget => m_cameraFollowTarget;
    public PlayerCamera PlayerCamera => m_playerCamera;
    public Camera WeaponCamera => m_weaponCamera;
    public float PlayerFOV => m_playerCamera.FOV;
    public float WeaponFOV => m_weaponCamera.fieldOfView;
    public bool EnableView = true;

    private PhotonView pv;

    private void Awake()
    {
        m_playerCamera = FindObjectOfType<PlayerCamera>();
        m_inputManager = GetComponent<PlayerInputManager>();
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!pv.IsMine) return;
        if (!EnableView) return;

        m_playerCamera.SetFollowTarget(m_cameraFollowTarget);
        m_playerCamera.SetDistance(0);
        m_playerCamera.SetDamping(Vector3.zero);
        m_playerCamera.SetShoulderOffset(Vector3.zero);
        SetFOV(m_playerFOV);
        SetWeaponCameraFOV(m_weaponFOV);
    }

    private void Update()
    {
        if (!EnableView) return;

        // Handle Player Viewing
        MousePitchRotation(m_inputManager.GetMouseInput());
        MouseYAWRotation(transform, m_inputManager.GetMouseInput());

        // Update Weapon Position with Camera
        m_weaponCamera.transform.position = m_playerCamera.MainCamera.transform.position;
        m_weaponCamera.transform.rotation = m_playerCamera.MainCamera.transform.rotation;
    }

    public void SetCameraViewMode(EViewMode viewMode)
    {
        switch (viewMode)
        {
            case EViewMode.FPS:
                m_playerCamera.SetDistance(0);
                m_playerCamera.SetDamping(Vector3.zero);
                m_playerCamera.SetShoulderOffset(Vector3.zero);
                m_playerCamera.SetLayerRendering("CharacterMesh", false);
                break;
            case EViewMode.TPS:
                m_playerCamera.SetDistance(_cameraDistance);
                m_playerCamera.SetDamping(_damping);
                m_playerCamera.SetShoulderOffset(_shoulderOffset);
                m_playerCamera.SetLayerRendering("CharacterMesh", true);
                break;
            default:
                return;
        }
    }

    public void MouseYAWRotation(Transform targetTransform, Vector2 mouseInputAxis)
    {
        targetTransform.Rotate
        (
            new Vector3(0f, (mouseInputAxis.x * m_lookSpeed * RotationMultiplier),
                0f),
            Space.Self
        );
    }

    public void MousePitchRotation(Vector2 mouseInputAxis)
    {
        m_CameraVerticalAngle += mouseInputAxis.y * m_lookSpeed * RotationMultiplier;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, m_pitchClamp.x, m_pitchClamp.y);
        m_cameraFollowTarget.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
    }

    public void SetFOV(float fov)
    {
        m_playerCamera.SetFOV(fov);
    }

    public void SetWeaponCameraFOV(float fov)
    {
        m_weaponCamera.fieldOfView = fov;
    }
}