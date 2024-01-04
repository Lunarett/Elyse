using UnityEngine;

public class SightSensorGizmos : MonoBehaviour
{
    private AISightSensor sightSensor;
    [SerializeField] private Color detectedColor = Color.green;

    private void Awake()
    {
        sightSensor = GetComponent<AISightSensor>();
    }

    private void OnDrawGizmos()
    {
        if (sightSensor == null || sightSensor.DetectedObjects == null) return;

        Gizmos.color = detectedColor;

        if (sightSensor.DetectedObjects.Count <= 0)
        {
            Debug.LogError("No Detected Objects!");
        }
        
        foreach (var obj in sightSensor.DetectedObjects)
        {
            if (obj != null)
            {
                Gizmos.DrawSphere(obj.transform.position, 0.5f);
            }
            else
            {
                Debug.LogError("obj has returned null");
            }
        }
    }
}