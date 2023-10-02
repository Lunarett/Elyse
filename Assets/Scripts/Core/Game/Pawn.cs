using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public abstract class Pawn : MonoBehaviour
{
    public Controller Owner { get; set; }
    protected PhotonView _photonView;

    protected void Awake()
    {
        Debug.Log("P");
        _photonView = GetComponent<PhotonView>();
        if (_photonView == null)
        {
            Debug.LogError("Pawn: pv null");
            return;
        }
        Owner = PhotonView.Find((int) _photonView.InstantiationData[0]).GetComponent<Controller>();
    }
    
    public void ShowMouseCursor(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
