using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class PlayerState : MonoBehaviour
{
    protected PhotonView _photonView;

    protected virtual void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    protected void UpdatePlayerProperty(string propertyName, object propertyValue)
    {
        Hashtable hash = new Hashtable
        {
            { propertyName, propertyValue }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}