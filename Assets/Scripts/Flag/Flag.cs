using System;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class Flag : MonoBehaviour
{

    public Transform FlagBasePosition;
    public float ReturnCooldown = 10.0f;

    private TeamMember _team;
    private Rigidbody _flagRigidbody;
    private Pawn _holderPawn;

    public event Action<Pawn> OnFlagPickedUp;
    public event Action<Pawn> OnFlagDropped;
    public event Action OnFlagReturnedToBase;
    public event Action<Pawn> OnTeamScored;

    private bool isAtBase = true;

    private void Awake()
    {
        _team = GetComponent<TeamMember>();
        _flagRigidbody = GetComponent<Rigidbody>();
        ReturnToBase();
    }

    private void OnTriggerEnter(Collider other)
    {
        _holderPawn = other.GetComponent<Pawn>();
        TeamMember opposingTeam = other.GetComponent<TeamMember>();
        if (opposingTeam == null) return;
        FlagReceiver flagReceiver = other.GetComponent<FlagReceiver>();
        
        if (TeamManager.Instance.AreAllies(_team, opposingTeam))
        {
            if (!flagReceiver.HasFlag()) return;
            flagReceiver.ReturnCapturedFlagToBase();
            OnTeamScored?.Invoke(flagReceiver.GetComponent<Pawn>());
        }
        else
        {
            if (flagReceiver.GetComponent<PlayerHealth>().IsDead()) return;
            flagReceiver.AttachFlag(this);
            transform.localPosition = Vector3.zero;
            transform.localRotation= quaternion.identity;
            
            OnFlagPickedUp?.Invoke(_holderPawn);
            _flagRigidbody.isKinematic = true;
            isAtBase = false;
        }
    }

    public void DropFlag()
    {
        OnFlagDropped?.Invoke(_holderPawn);
        _holderPawn = null;
        _flagRigidbody.isKinematic = false;
        transform.rotation = Quaternion.identity;
        StartCoroutine(ReturnToBaseAfterCooldown());
    }

    IEnumerator ReturnToBaseAfterCooldown()
    {
        yield return new WaitForSeconds(ReturnCooldown);
        ReturnToBase();
        OnFlagReturned();
    }

    public void ReturnToBase()
    {
        _flagRigidbody.isKinematic = true;
        transform.SetParent(FlagBasePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation= quaternion.identity;
        isAtBase = true;
    }

    private void OnFlagReturned()
    {
        OnFlagReturnedToBase?.Invoke();
    }
}