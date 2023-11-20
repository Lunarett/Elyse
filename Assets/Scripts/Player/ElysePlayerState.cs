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
        _numberOfDeaths++;
        UpdatePlayerProperty(_photonView.Owner, "Deaths", _numberOfDeaths);
    }
    
    public void AddKill()
    {
        _photonView.RPC(nameof(RPC_AddKill), _photonView.Owner);
    }

    [PunRPC]
    public void RPC_AddKill()
    {
        _numberOfKills++;
        UpdatePlayerProperty(_photonView.Owner, "Kills", _numberOfKills);
    }

    public void ResetStats()
    {
        if (!_photonView.IsMine) return;
        
        _numberOfKills = 0;
        _numberOfDeaths = 0;
        UpdatePlayerProperty(_photonView.Owner, "Kills", _numberOfKills);
        UpdatePlayerProperty(_photonView.Owner, "Deaths", _numberOfDeaths);
    }
}
