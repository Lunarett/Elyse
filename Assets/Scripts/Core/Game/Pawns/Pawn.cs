using Photon.Pun;
using Photon.Realtime;
using Pulsar.Debug;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public abstract class Pawn : MonoBehaviourPunCallbacks
{
    protected InputManager _inputManager;
    protected PhotonView _photonView;
    
    public PlayerController PlayerController { get; private set; }
    public Player Owner { get; private set; }

    public void Initialize(Player owner)
    {
        Owner = owner;
    }
    
    protected virtual void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        if (DebugUtils.CheckForNull<PhotonView>(_photonView, "Pawn: PhotonView is missing!")) return;
        
        _inputManager = GetComponent<InputManager>();
        if (DebugUtils.CheckForNull<InputManager>(_inputManager, "Pawn: PhotonView is missing!")) return;
        
        PlayerController = PhotonView.Find((int) photonView.InstantiationData[0]).GetComponent<PlayerController>();
        if (DebugUtils.CheckForNull<PlayerController>(PlayerController, "Pawn: PhotonView is missing!")) return;
    }
    
    public void ShowMouseCursor(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void EnableControl(bool isEnabled, bool ignoreMouse = false)
    {
        _inputManager.EnableControl(isEnabled, ignoreMouse);
    }
}
