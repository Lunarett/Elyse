using System.Collections;
using UnityEngine;

public class PlayerController : Controller
{
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