using System.Collections;
using System.Collections.Generic;
using Pulsar.Debug;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected string _nickName = "Player";
    protected Pawn _controlledPawn;
    private Pawn _pawnPrefab;

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
        if (_controlledPawn != null) Debug.Log("Controller: Spawned Pawn successfully!");
    }

    public void DestroyPawn()
    {
        if (_controlledPawn != null) Destroy(_controlledPawn.gameObject);
    }
    
    public virtual void RespawnPawn(float delay = 0f)
    {
        StartCoroutine(RespawnCoroutine(delay));
    }
    
    private IEnumerator RespawnCoroutine(float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        DestroyPawn();

        // Check if the prefab is available
        if (_pawnPrefab != null)
        {
            CreatePawn(_pawnPrefab);
        }
        else
        {
            Debug.LogError("Controller: Pawn prefab is null, cannot respawn!");
        }
    }
}
