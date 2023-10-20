using UnityEngine;
using Photon.Pun;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    private PanelManager _panelManager;

    private void Awake()
    {
        _panelManager = GetComponent<PanelManager>();
        
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _panelManager.ShowPanel(1);
    }
}
