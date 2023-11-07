using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DeathMatchMode : GameModeBase
{
    [SerializeField] private float preMatchCountdownTime = 3.0f;
    [SerializeField] private float matchDuration = 120.0f;  // 2 minutes

    private bool isMatchStarted = false;

    protected override void Start()
    {
        base.Start();
        if (PhotonNetwork.IsConnectedAndReady)
        {
            DisablePlayerMovement();
            StartCoroutine(PreMatchCountdown());
        }
    }

    private IEnumerator PreMatchCountdown()
    {
        float countdown = preMatchCountdownTime;
        while (countdown > 0)
        {
            // You can update a UI element here to show the countdown to the players
            countdown -= Time.deltaTime;
            yield return null;
        }

        StartMatch();
    }

    private void StartMatch()
    {
        isMatchStarted = true;
        EnablePlayerMovement();
        StartCoroutine(MatchTimer());
    }

    private IEnumerator MatchTimer()
    {
        float remainingTime = matchDuration;
        while (remainingTime > 0)
        {
            // You can update a UI element here to show the remaining match time to the players
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        EndMatch();
    }

    private void EndMatch()
    {
        isMatchStarted = false;
        DisablePlayerMovement();
        // Display scoreboard here
    }

    private void EnablePlayerMovement()
    {
        foreach (var playerController in _playerControllerList)
        {
            var character = playerController.Pawn as Character;
            if (character != null)
            {
                character.EnableMovement(true);
            }
        }
    }

    private void DisablePlayerMovement()
    {
        foreach (var playerController in _playerControllerList)
        {
            var character = playerController.Pawn as Character;
            if (character != null)
            {
                character.EnableMovement(false);
            }
        }
    }

}