using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeathMatchMode : GameModeBase
{
    [SerializeField] private float preMatchCountdownTime = 3.0f;
    [SerializeField] private float matchDuration = 120.0f;  // 2 minutes

    private bool isMatchStarted = false;
    private HUD _hud;

    protected override void Start()
    {
        base.Start();
        _hud = HUD.Instance;
        if (!PhotonNetwork.IsConnectedAndReady) return;
        _hud.SetMatchText("Match will begin in...");
        StartCoroutine(PreMatchCountdown());
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        DisablePlayerMovement();
    }

    private IEnumerator PreMatchCountdown()
    {
        float countdown = preMatchCountdownTime;
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            _hud.SetTimerText((int)countdown);
            yield return null;
        }

        StartMatch();
    }

    private void StartMatch()
    {
        isMatchStarted = true;
        EnablePlayerMovement();
        StartCoroutine(MatchTimer());
        _hud.SetMatchText("Match has begun!");
        Invoke(nameof(TurnOffMatchText), 3.0f);
    }

    private void TurnOffMatchText()
    {
        _hud.SetMatchText("");
    }

    private IEnumerator MatchTimer()
    {
        float remainingTime = matchDuration;
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            _hud.SetTimerText(remainingTime);
            yield return null;
        }

        EndMatch();
    }

    private void EndMatch()
    {
        isMatchStarted = false;
        DisablePlayerMovement();
        _hud.SetMatchText("Match is over!");
        _hud.SetTimerText(0, false);
    }

    private void EnablePlayerMovement()
    {
        foreach (var playerController in _playerControllerList)
        {
            var character = playerController.Pawn as Character;
            if (character == null) continue;
            Debug.Log("Enable Move");
            character.EnableMovement(true, false);
        }
    }

    private void DisablePlayerMovement()
    {
        foreach (var playerController in _playerControllerList)
        {
            var character = playerController.Pawn as Character;
            if (character == null) continue;
            Debug.Log("Disable Move");
            character.EnableMovement(false, true);
        }
    }

    public void LeaveMatch()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        StartDisconnect();
    }

    private void StartDisconnect()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            OnDisconnectedFromPhoton();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        OnDisconnectedFromPhoton();
    }

    private void OnDisconnectedFromPhoton()
    {
        PhotonNetwork.LoadLevel(0);
    }
}