using UnityEngine;

public class AILookController : MonoBehaviour
{
    [SerializeField] private AISightSensor aiSightSensor;
    [SerializeField] private Transform headTransform; // The part of the AI that should turn to face the enemy
    [SerializeField] private float lookSpeed = 5f; // How quickly the AI turns to face the target

    private GameObject currentTarget;

    private void Update()
    {
        UpdateTarget();
        LookAtTarget();
    }

    private void UpdateTarget()
    {
        // Simplified target selection - chooses the first enemy found. You can make this more sophisticated.
        //var enemiesInView = sightSensor.GetEnemiesInView(); // Assume you implement this method to return enemies
        //currentTarget = enemiesInView.Count > 0 ? enemiesInView[0] : null; // Picks the first enemy or null if none
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
}