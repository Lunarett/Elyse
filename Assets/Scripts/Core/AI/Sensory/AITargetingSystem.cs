using UnityEngine;
using System.Collections;

/*
 * - Responsible for target selection for AI entities
 * - Evaluates targets based on distance and angle
 * - Works with AISightSensor for detection and AIMemory for remembering targets
 * - Attach to GameObject with AISightSensor and AIMemory
 * - Gizmo properties adjustable in the inspector for visualization
 */

[RequireComponent(typeof(AISightSensor), typeof(AIMemory))]
public class AITargetingSystem : MonoBehaviour
{
    [Header("Targeting Parameters")]
    [SerializeField] private float _angleWeight = 1.0f;
    [SerializeField] private float _distanceWeight = 1.0f;

    [Header("Gizmo Settings")]
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private Color _gizmoColor = Color.red;
    [SerializeField] private float _gizmoSize = 0.5f;

    [Header("Performance Settings")]
    [SerializeField] private float _updateInterval = 0.5f;

    private AISightSensor _sightSensor;
    private AIMemory _aiMemory;
    private GameObject _currentTarget;

    public GameObject CurrentTarget => _currentTarget;

    private void Start()
    {
        _sightSensor = GetComponent<AISightSensor>();
        _aiMemory = GetComponent<AIMemory>();
        StartCoroutine(TargetEvaluationRoutine());
    }

    private IEnumerator TargetEvaluationRoutine()
    {
        while (true)
        {
            EvaluateTargets();
            yield return new WaitForSeconds(_updateInterval);
        }
    }

    private void EvaluateTargets()
    {
        var detectedObjects = _sightSensor.DetectedObjects;
        float bestScore = float.MinValue;
        GameObject bestTarget = null;

        foreach (var obj in detectedObjects)
        {
            if (obj == null) continue;

            float score = CalculateScore(obj);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = obj;
            }
        }

        if (bestTarget == null)
        {
            GameObject memoryTarget = _aiMemory.GetMostRecentTarget();
            if (memoryTarget != null)
            {
                Vector3? lastKnownPosition = _aiMemory.GetLastKnownPosition(memoryTarget);
                if (lastKnownPosition.HasValue)
                {
                    float memoryScore = CalculateScoreFromPosition(memoryTarget, lastKnownPosition.Value);
                    if (memoryScore > bestScore)
                    {
                        bestTarget = memoryTarget;
                    }
                }
            }
        }

        _currentTarget = bestTarget;

        if (_currentTarget != null)
        {
            _aiMemory.UpdateMemory(_currentTarget);
        }
    }

    private float CalculateScore(GameObject target)
    {
        return CalculateScoreFromPosition(target, target.transform.position);
    }

    private float CalculateScoreFromPosition(GameObject target, Vector3 position)
    {
        float distanceScore = 1.0f / (Vector3.Distance(transform.position, position) * _distanceWeight);
        Vector3 directionToTarget = (position - transform.position).normalized;
        float angleScore = Vector3.Dot(directionToTarget, transform.forward) * _angleWeight;

        return distanceScore + angleScore;
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos || _currentTarget == null) return;
        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(_currentTarget.transform.position, _gizmoSize);
    }
}