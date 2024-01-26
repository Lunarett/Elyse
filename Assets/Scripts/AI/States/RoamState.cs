using UnityEngine;
using UnityEngine.AI;

public class RoamState : State
{
    private const float RandomRoamRadius = 10f; // Define the radius within which the AI will pick a new destination
    private GameObject _lastSeenTarget;

    public override EStateID GetStateID()
    {
        return EStateID.Roam;
    }

    public override void BeginState(AIPawn pawn)
    {
        _lastSeenTarget = null;
        SetRandomDestination(pawn);
    }

    public override void UpdateState(AIPawn pawn)
    {
        GameObject currentTarget = pawn.TargetSystem.CurrentTarget;

        // Check if a new target is seen and if it's not on the same team
        if (currentTarget != _lastSeenTarget)
        {
            _lastSeenTarget = currentTarget;
            if (currentTarget != null && !IsSameTeam(pawn.Team, currentTarget))
            {
                pawn.StateMachine.ChangeState(EStateID.Chase);
                return;
            }
        }

        // If the AI has reached its destination or doesn't have a path, pick a new random destination
        if (!pawn.Agent.hasPath || pawn.Agent.remainingDistance < 1f)
        {
            SetRandomDestination(pawn);
        }
    }

    public override void EndState(AIPawn pawn)
    {
        // Optional: any cleanup when leaving the Roam state
    }

    private void SetRandomDestination(AIPawn pawn)
    {
        Vector3 randomDirection = Random.insideUnitSphere * RandomRoamRadius;
        randomDirection += pawn.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, RandomRoamRadius, NavMesh.AllAreas))
        {
            pawn.Agent.SetDestination(hit.position);
        }
    }

    private bool IsSameTeam(TeamMember pawnTeam, GameObject target)
    {
        TeamMember targetTeam = target.GetComponent<TeamMember>();
        return targetTeam != null && pawnTeam.TeamId == targetTeam.TeamId;
    }
}