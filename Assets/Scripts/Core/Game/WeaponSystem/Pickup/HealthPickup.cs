using UnityEngine;

public class HealthPickup : PickupBase
{
    [SerializeField] private float _healAmount = 15.0f;
    
    protected override void OnPickup(Pawn pawn)
    {
        HealthBase health = pawn.GetComponent<HealthBase>();
        if (health == null) return;
        
        health.ApplyHeal(_healAmount);
    }
}
