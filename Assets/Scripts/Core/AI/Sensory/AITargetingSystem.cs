using System.Collections.Generic;
using UnityEngine;

public class AiTargetingSystem : MonoBehaviour
{
    [Header("Targeting Properties")]
    [SerializeField] private float forgetTargetTime = 3.0f;

    [Header("Weights")]
    [SerializeField] private float distanceWeight = 1.0f;
    [SerializeField] private float angleWeight = 1.0f;
    [SerializeField] private float ageWeight = 1.0f;

    private AISensoryMemory memory = new AISensoryMemory(10);
    private AISightSensor sightSensor;
    private AIMemory bestEnemyMemory;
    private List<AIMemory> alliesInSight = new List<AIMemory>();

    private TeamMember selfTeamMember;

    public bool HasTarget => bestEnemyMemory != null;
    public GameObject Target => bestEnemyMemory?.GameObject;
    public Vector3 TargetPosition => bestEnemyMemory?.Position ?? Vector3.zero;
    public bool TargetInSight => bestEnemyMemory != null && bestEnemyMemory.Age < 0.5f;
    public float TargetDistance => bestEnemyMemory?.Distance ?? float.MaxValue;

    private void Start()
    {
        sightSensor = GetComponent<AISightSensor>();
        selfTeamMember = GetComponent<TeamMember>();
    }

    private void Update()
    {
        memory.UpdateSensory(sightSensor);
        memory.ForgetMemories(forgetTargetTime);

        EvaluateScores();
    }

    void EvaluateScores()
    {
        bestEnemyMemory = null;
        alliesInSight.Clear();

        foreach (var memory in this.memory.Memories)
        {
            TeamMember otherMember = memory.GameObject.GetComponent<TeamMember>();
            if (otherMember == null) continue;

            if (TeamManager.Instance.AreAllies(selfTeamMember, otherMember))
            {
                alliesInSight.Add(memory);
                continue;
            }

            memory.Score = CalculateScore(memory);

            if (bestEnemyMemory == null || memory.Score > bestEnemyMemory.Score)
            {
                bestEnemyMemory = memory;
            }
        }
    }

    private float Normalize(float value, float maxValue)
    {
        return 1.0f - (value / maxValue);
    }

    float CalculateScore(AIMemory memory)
    {
        // Calculate the angle dynamically based on the AI's and target's positions.
        Vector3 directionToTarget = (memory.Position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        // Normalize the dynamically calculated angle (assuming 180 is the max angle)
        float normalizedAngle = 1.0f - (angleToTarget / 180.0f);

        // Now use this normalized angle in the score calculation
        float distanceScore = Normalize(memory.Distance, sightSensor.SightDistance) * distanceWeight;
        float angleScore = normalizedAngle * angleWeight; // Using dynamically calculated angle now
        float ageScore = Normalize(memory.Age, forgetTargetTime) * ageWeight;

        return distanceScore + angleScore + ageScore;
    }


    private void OnDrawGizmos()
    {
        if (memory == null) return;
        float maxScore = float.MinValue;

        foreach (var mem in memory.Memories)
        {
            maxScore = Mathf.Max(maxScore, mem.Score);
        }

        foreach (var mem in memory.Memories)
        {
            Color color = TeamManager.Instance.AreAllies(selfTeamMember, mem.GameObject.GetComponent<TeamMember>()) ? Color.blue : Color.red;
            color.a = mem.Score / maxScore;

            if (mem == bestEnemyMemory)
            {
                color = Color.yellow;
            }

            Gizmos.color = color;
            Gizmos.DrawSphere(mem.Position, 0.2f);
        }
    }
}
