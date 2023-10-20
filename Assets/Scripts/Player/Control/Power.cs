using UnityEngine;

public class Power : MonoBehaviour
{
    [SerializeField] private float maxPower = 100f;
    [SerializeField] private float rechargeRate = 5f;
    [SerializeField] private float rechargeDelay = 1f;
    
    private float currentPower;
    private float lastConsumeTime;

    private void Awake()
    {
        currentPower = maxPower;
    }

    private void Update()
    {
        if (Time.time >= lastConsumeTime + rechargeDelay)
        {
            RechargePower(rechargeRate * Time.deltaTime);
        }
    }

    public bool ConsumePower(float amount)
    {
        if (!(currentPower >= amount)) return false;
        currentPower -= amount;
        lastConsumeTime = Time.time;
        return true;
    }

    public void RechargePower(float amount)
    {
        currentPower = Mathf.Min(maxPower, currentPower + amount);
    }

    public float GetCurrentPower()
    {
        return currentPower;
    }
}