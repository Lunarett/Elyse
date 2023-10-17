using UnityEngine;
using Photon.Pun;


public class ElysePlayerState : PlayerState
{
    private int _numberOfKills = 0;
    private int _numberOfDeaths = 0;
    
    public int Kills => _numberOfKills;
    public int Deaths => _numberOfDeaths;

    public void AddDeath()
    {
        _photonView.RPC(nameof(RPC_AddDeath), _photonView.Owner);
    }

    [PunRPC]
    public void RPC_AddDeath()
    {
        Debug.Log($"{_photonView.Owner.NickName} has died!");
        _numberOfDeaths++;
        UpdatePlayerProperty("Deaths", _numberOfDeaths);
    }
    
    public void AddKill()
    {
        _photonView.RPC(nameof(RPC_AddKill), _photonView.Owner);
    }

    [PunRPC]
    public void RPC_AddKill()
    {
        Debug.Log($"{_photonView.Owner.NickName} has killed!");
        _numberOfKills++;
        UpdatePlayerProperty("Kills", _numberOfKills);
    }
}
