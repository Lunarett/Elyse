using System.Collections.Generic;
using UnityEngine;

public class AISightSensor : MonoBehaviour
{
    [Header("Sight Properties")]
    [SerializeField] private Camera visionCamera;
    [SerializeField] private float sightDistance = 10.0f;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private LayerMask occlusionLayerMask;
    [SerializeField] private int scanFrequency = 30;

    private List<GameObject> detectedObjects = new List<GameObject>();
    private Collider[] colliders = new Collider[50];
    private int count;
    private float scanInterval;
    private float scanTimer;

    public float SightDistance => sightDistance;
    public LayerMask TargetLayerMask => targetLayerMask;

    public List<GameObject> DetectedObjects
    {
        get
        {
            detectedObjects.RemoveAll(obj => !obj);
            return detectedObjects;
        }
    }

    private void Start()
    {
        scanInterval = 1.0f / scanFrequency;
    }

    private void Update()
    {
        scanTimer -= Time.deltaTime;

        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, sightDistance, colliders, targetLayerMask, QueryTriggerInteraction.Collide);

        detectedObjects.Clear();

        for (int i = 0; i < count; ++i)
        {
            GameObject go = colliders[i].gameObject;

            if (IsInSight(go))
            {
                detectedObjects.Add(go);
            }
        }
    }

    private bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 direction = obj.transform.position - origin;

        if (direction.magnitude > sightDistance)
        {
            return false;
        }

        if (!FrustumContainsPoint(visionCamera, obj.transform.position))
        {
            return false;
        }

        if (Physics.Linecast(origin, obj.transform.position, occlusionLayerMask))
        {
            return false;
        }

        return true;
    }

    private bool FrustumContainsPoint(Camera cam, Vector3 point)
    {
        Vector3[] frustumCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        for (int i = 0; i < frustumCorners.Length; i++)
        {
            frustumCorners[i] = cam.transform.TransformVector(frustumCorners[i]);
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }
    
    public int Filter(GameObject[] buffer, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int count = 0;

        foreach (var obj in DetectedObjects)
        {
            if (obj != null && obj.layer == layer)
            {
                buffer[count++] = obj;
                if (count == buffer.Length) break;
            }
        }

        return count;
    }
}
