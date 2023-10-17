using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera m_camera;
    private CinemachineVirtualCamera m_virtualCamera;
    private Cinemachine3rdPersonFollow m_tpFollow;

    public Camera MainCamera => m_camera;
    public CinemachineVirtualCamera VirtualCamera => m_virtualCamera;
    public float FOV => m_virtualCamera != null ? m_virtualCamera.m_Lens.FieldOfView : 0;

    private void Awake()
    {
        m_virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        m_tpFollow = m_virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        Transform t = transform;
        t.position = Vector3.zero;
        t.rotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    public void SetFollowTarget(Transform target)
    {
        m_virtualCamera.Follow = target;
    }

    public void SetDistance(float distance)
    {
        m_tpFollow.CameraDistance = distance;
    }

    public void SetShoulderOffset(Vector3 offset)
    {
        m_tpFollow.ShoulderOffset = offset;
    }

    public void SetDamping(Vector3 damping)
    {
        m_tpFollow.Damping = damping;
    }

    public void SetFOV(float fov)
    {
        m_virtualCamera.m_Lens.FieldOfView = fov;
    }

    public void SetLayerRendering(string layerName, bool shouldRender)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (shouldRender)
        {
            m_camera.cullingMask |= (1 << layer);
        }
        else
        {
            m_camera.cullingMask &= ~(1 << layer);
        }
    }
}