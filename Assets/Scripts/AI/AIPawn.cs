using System;
using UnityEngine;
using UnityEngine.AI;

public class AIPawn : Pawn
{
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    private NavMeshAgent _agent;
    private StateMachine _stateMachine;
    private AIWeaponManager _weaponManager;
    private Animator _anim;
    
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Grounded = Animator.StringToHash("Grounded");

    public NavMeshAgent Agent => _agent;
    public AIWeaponManager WeaponManager => _weaponManager;
    public StateMachine StateMachine => _stateMachine;

    protected override void Awake()
    {
        base.Awake();
        _anim = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }


    protected override void Start()
    {
        base.Start();
        _stateMachine = new StateMachine(this);
        _stateMachine.InitializeState(new RoamState());
        _stateMachine.InitializeState(new ChaseState());
        _stateMachine.ChangeState(EStateID.Roam);
    }

    private void Update()
    {
        _stateMachine.UpdateStateMachine();
    }

    private void LateUpdate()
    {
        UpdateAIAnimation();
    }

    private void UpdateAIAnimation()
    {
        float speed = _agent.velocity.magnitude;
        Vector3 velocity = _agent.desiredVelocity;
        float verticalValue = Vector3.Dot(velocity.normalized, transform.forward);
        float horizontalValue = Vector3.Dot(velocity.normalized, transform.right);

        _anim.SetFloat(Vertical, verticalValue);
        _anim.SetFloat(Horizontal, horizontalValue);
        _anim.SetBool(Grounded, IsGrounded());
    }
    
    public bool IsGrounded()
    {
        return !_agent.isOnOffMeshLink && Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);
    }
}
