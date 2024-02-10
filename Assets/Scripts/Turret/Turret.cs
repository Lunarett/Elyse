using System;
using UnityEngine;

public class Turret : Pawn
{
    [Header("Detection Properties")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float _idleRotationSpeed = 30f;
    [SerializeField] private float maxAimDistance = 100f;
    
    [Header("Firing Properties")]
    [SerializeField] private ProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float _damage = 20.0f; 
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private LayerMask hitLayers;

    private Transform target;
    private float currentRotation = 0;
    private float lastFireTime;
    private TeamMember _team;
    private DamageCauseInfo _damageInfo;

    private enum State
    {
        Idle,
        Detected
    }

    private State currentState = State.Idle;

    private void Awake()
    {
        _team = GetComponent<TeamMember>();

        _damageInfo = new DamageCauseInfo()
        {
            CauseOfDeath = ECauseOfDeath.KilledByPlayer,
            causer = this,
            damage = _damage
        };
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleBehaviour();
                break;
            case State.Detected:
                DetectedBehaviour();
                break;
        }

        DetectTargets();
    }

    private void IdleBehaviour()
    {
        currentRotation += _idleRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, currentRotation, 0);
    }

    private void DetectedBehaviour()
    {
        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) <= maxAimDistance && Time.time > lastFireTime + 1 / fireRate)
            {
                FireProjectile();
                lastFireTime = Time.time;
            }
        }
        else
        {
            currentState = State.Idle;
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;
        ProjectileBase projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(target.position - firePoint.position));
        projectile.Speed = projectilePrefab.Speed;
        projectile.DownForce = projectilePrefab.DownForce;
        projectile.MaxDistance = projectilePrefab.MaxDistance;
        projectile.HitLayers = hitLayers;
        projectile.DamageInfo = _damageInfo;
    }

    private void DetectTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        foreach (var hit in hits)
        {
            Pawn pawn = hit.GetComponent<Pawn>();
            if (pawn == null || TeamManager.Instance.AreAllies(_team, pawn.GetComponent<TeamMember>())) continue;
            target = hit.transform;
            currentState = State.Detected;
            return;
        }
        
        if (currentState == State.Detected && target == null)
        {
            currentState = State.Idle;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
