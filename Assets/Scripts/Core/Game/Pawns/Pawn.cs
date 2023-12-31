using Photon.Pun;
using Photon.Realtime;
using Pulsar.Debug;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public abstract class Pawn : MonoBehaviour
{
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
        if (DebugUtils.CheckForNull<PhotonView>(_photonView)) return;
        
        PlayerController = PhotonView.Find((int) _photonView.InstantiationData[0]).GetComponent<PlayerController>();
        if (DebugUtils.CheckForNull<PlayerController>(PlayerController)) return;
    }
    
    public void ShowMouseCursor(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
