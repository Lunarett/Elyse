using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using Pulsar.Debug;

public class PlayerHealth : BaseHealth
{
    // Define an event for when the player dies
    public event Action<Player> OnPlayerDied;

    private ElyseCharacter _elyseCharacter;

    protected override void Awake()
    {
        base.Awake();
        _elyseCharacter = GetComponent<ElyseCharacter>();
        DebugUtils.CheckForNull<ElyseCharacter>(_elyseCharacter);
    }

    protected override void OnDeath(WeaponInfo damageCauserInfo)
    {
        // This method is called when this player's health reaches 0
        photonView.RPC(nameof(RPC_OnDeath), RpcTarget.All, damageCauserInfo.WeaponOwner);
    }

    [PunRPC]
    private void RPC_OnDeath(Player causerPlayer)
    {
        var causerController = Controller.Find(causerPlayer) as ElyseController;
        DebugUtils.CheckForNull<ElyseController>(causerController);

        if (causerController != null)
        {
            causerController.ElysePlayerState.AddKill();
        }
        else
        {
            Debug.LogError("Failed to cast Controller to ElyseController");
        }

        OnPlayerDied?.Invoke(causerPlayer);
    }
}