using System.Collections;
using System.Collections.Generic;
using Pulsar.Debug;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected string _nickName = "Player";
    protected Pawn _controlledPawn;
    protected Pawn _pawnPrefab;

    public Pawn ControlledPawn => _controlledPawn;
    public string NickName => _nickName;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    public virtual void CreatePawn(Pawn pawn)
    {
        if (DebugUtils.CheckForNull<Pawn>(pawn, "Controller: Failed to create pawn, passed pawn is null!")) return;
        Transform spawnpoint = PlayerSpawner.Instance.GetSpawnpoint();
        _pawnPrefab = pawn;
        _controlledPawn = Instantiate(pawn, spawnpoint.position, spawnpoint.rotation);
        _controlledPawn.SetOwner(this);
    }
    
    public void DestroyPawn()
    {
        if (_controlledPawn != null) Destroy(_controlledPawn.gameObject);
    }
    public virtual void Possess(Pawn pawn)
    {
        if (pawn == null) return;
        Unpossess();
        
        _controlledPawn = pawn;
        pawn.SetOwner(this);
    }

    public virtual void Unpossess()
    {
        if (_controlledPawn == null) return;
        _controlledPawn.RemoveOwner();
        _controlledPawn = null;
    }
}
