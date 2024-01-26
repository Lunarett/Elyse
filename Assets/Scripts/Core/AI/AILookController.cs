using UnityEngine;

public class AILookController : MonoBehaviour
{
    [SerializeField] private AISightSensor aiSightSensor;
    [SerializeField] private Transform headTransform; // The part of the AI that should turn to face the enemy
    [SerializeField] private float lookSpeed = 5f; // How quickly the AI turns to face the target

    private GameObject currentTarget;

    private void Update()
    {
        LookAtTarget();
    }

    private void LookAtTarget()
    {
        Quaternion targetRotation;
        if (currentTarget != null)
        {
            Vector3 directionToTarget = currentTarget.transform.position - headTransform.position;
            targetRotation = Quaternion.LookRotation(directionToTarget);
        }
        else
        {
            // When there's no target, return to the default forward orientation
            targetRotation = Quaternion.LookRotation(transform.forward);
        }

        // Smoothly interpolate towards the target or default rotation
        headTransform.rotation = Quaternion.Slerp(headTransform.rotation, targetRotation, Time.deltaTime * lookSpeed);
    }
    
    public void SetTarget(GameObject target)
    {
        currentTarget = target;
    }
}