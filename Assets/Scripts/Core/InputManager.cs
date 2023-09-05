using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private PhotonView m_photonView;
    private bool m_isInputActive = true;

    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_photonView = GetComponent<PhotonView>();
    }

    public void SetInputActive(bool isActive)
    {
        m_isInputActive = isActive;
    }

    protected bool CanProcessInput()
    {
        return m_isInputActive && m_photonView.IsMine;
    }

    protected bool CheckInputActionPhase(string actionName, InputActionPhase phase)
    {
        return CanProcessInput() && m_playerInput.actions[actionName].phase == phase;
    }

    protected T GetInputActionValue<T>(string actionName) where T : struct
    {
        return m_playerInput.actions[actionName].ReadValue<T>();
    }
}