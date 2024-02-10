using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    private float _minChaseDistance = 10f;
    private float _maxChaseDistance = 15f;
    private float _flankingRandomness = 4f; // Random factor for flanking movement
    private float _effectiveFiringRange = 12f;
    private float _rotationSpeed = 5f;
    private float _repositionInterval = 1.0f; // Interval to change the flanking position
    private float _repositionTimer;

    public override EStateID GetStateID()
    {
        return EStateID.Chase;
    }

    public override void BeginState(AIPawn pawn)
    {
        _repositionTimer = _repositionInterval;
    }

    public override void UpdateState(AIPawn pawn)
    {
        GameObject currentTarget = pawn.TargetSystem.CurrentTarget;

        if (currentTarget == null)
        {
            pawn.StateMachine.ChangeState(EStateID.Roam);
            return;
        }

        RotateTowardsTarget(pawn, currentTarget);
        pawn.LookController.SetTarget(currentTarget);

        if (Vector3.Distance(pawn.transform.position, currentTarget.transform.position) <= _effectiveFiringRange)
        {
            //pawn.WeaponManager.Fire();
        }

        // Flanking and chasing logic
        _repositionTimer -= Time.deltaTime;
        if (_repositionTimer <= 0)
        {
            _repositionTimer = _repositionInterval;
            FlankingMovement(pawn, currentTarget);
        }
    }

    public override void EndState(AIPawn pawn)
    {
        pawn.LookController.SetTarget(null);
    }

    private void RotateTowardsTarget(AIPawn pawn, GameObject target)
    {
        Vector3 directionToTarget = target.transform.position - pawn.transform.position;
        directionToTarget.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        pawn.transform.rotation = Quaternion.Slerp(pawn.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void FlankingMovement(AIPawn pawn, GameObject target)
    {
        Vector3 directionToTarget = (target.transform.position - pawn.transform.position).normalized;
        Vector3 right = Quaternion.Euler(0, 90, 0) * directionToTarget;
        Vector3 randomFlankDirection = directionToTarget * UnityEngine.Random.Range(-_flankingRandomness, _flankingRandomness) 
                                       + right * UnityEngine.Random.Range(-_flankingRandomness, _flankingRandomness);

        Vector3 newPosition = pawn.transform.position + randomFlankDirection;
        pawn.Agent.SetDestination(newPosition);
    }
}