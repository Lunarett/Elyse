using UnityEngine;
using System.Collections;

public abstract class PickupBase : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private GameObject _pickupObject;
    [SerializeField] private float _cooldownDuration = 5f;

    private bool _isCooldown;

    private void OnTriggerEnter(Collider other)
    {
        if(_isCooldown) return;
        Pawn pawn = other.gameObject.GetComponent<Pawn>();
        if (pawn == null) return;
        OnPickup(pawn);
        StartCoroutine(Cooldown());
    }

    protected abstract void OnPickup(Pawn pawn);

    private IEnumerator Cooldown()
    {
        _isCooldown = true;
        _pickupObject.SetActive(false);

        yield return new WaitForSeconds(_cooldownDuration);

        _pickupObject.SetActive(true);
        _isCooldown = false;
    }
}