using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using VInspector;

public class EnemyMovement : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public LayerMask obstacleLayer;

    [Header("Movement Settings")]
    public float patrolRadius = 30f;
    public float approachDistance = 20f;
    public float strafeDistance = 8f;
    public float dodgeDistance = 5f;
    public float retreatDistance = 15f;
    public float dashDistance = 3f;
    public float dashSpeed = 15f;
    public float lookSpeed = 10f;
    public float chaseDuration = 5f;
    public float dodgeProbability = 0.3f;
    public float activeMoveDuration = 3f;
    public float searchDuration = 5f;
    public float maxTimeToReachDestination = 2f;

    [Header("Debug Settings")]
    public bool disableLogic = false;

    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private bool isChasing = false;
    public float chaseStartTime;
    public EnemyState currentState;

    private bool isStrafing = false;
    private float nextActiveMoveTime = 0f;
    public float searchStartTime;
    private float destinationStartTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomPatrolTarget();
        currentState = EnemyState.Patrolling;
    }

    void Update()
    {
        if (disableLogic) return;

        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.ActiveMove:
                ActiveMove();
                break;
            case EnemyState.Dodging:
                // Logica per il Dodge
                break;
            case EnemyState.Retreating:
                Retreat();
                break;
            case EnemyState.Searching:
                SearchPlayer();
                break;
            case EnemyState.Idle:
                // Stato inattivo
                break;
        }

        if (currentState != EnemyState.Chasing && currentState != EnemyState.Patrolling && currentState != EnemyState.Searching)
        {
            LookAtPlayer();
        }

        CheckDestinationTimeout();
    }

    void SetRandomPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
        }
        destinationStartTime = Time.time;
    }

    void CheckDestinationTimeout()
    {
        if (Time.time > destinationStartTime + maxTimeToReachDestination)
        {
            switch (currentState)
            {
                case EnemyState.Patrolling:
                    SetRandomPatrolTarget();
                    break;
                case EnemyState.Chasing:
                    agent.SetDestination(player.position);
                    destinationStartTime = Time.time;
                    break;
                case EnemyState.ActiveMove:
                    StartActiveMove();
                    break;
                case EnemyState.Retreating:
                    Retreat();
                    break;
                case EnemyState.Searching:
                    StartSearching();
                    break;
            }
        }
    }

    public void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolTarget) < 1f || patrolTarget == Vector3.zero)
        {
            SetRandomPatrolTarget();
        }
        agent.SetDestination(patrolTarget);

        if (CanSeePlayer())
        {
            StartChasing();
        }
    }

    public void StartChasing()
    {
        isChasing = true;
        chaseStartTime = Time.time;
        currentState = EnemyState.Chasing;
        destinationStartTime = Time.time;
    }

    public void ChasePlayer()
    {
        if (Time.time > chaseStartTime + chaseDuration)
        {
            StartSearching();
            return;
        }
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) < approachDistance)
        {
            StartActiveMove();
        }
    }

    public void StartSearching()
    {
        currentState = EnemyState.Searching;
        searchStartTime = Time.time;
        destinationStartTime = Time.time;
    }

    public void SearchPlayer()
    {
        if (Time.time > searchStartTime + searchDuration)
        {
            StartPatrolling();
            return;
        }
        agent.SetDestination(player.position);

        if (CanSeePlayer())
        {
            StartChasing();
        }
    }

    public void StartActiveMove()
    {
        currentState = EnemyState.ActiveMove;
        agent.isStopped = false;
        nextActiveMoveTime = Time.time + activeMoveDuration;
        destinationStartTime = Time.time;
    }

    public void StartPatrolling()
    {
        isChasing = false;
        currentState = EnemyState.Patrolling;
        agent.isStopped = false;
        SetRandomPatrolTarget();
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
    }

    public void ActiveMove()
    {
        if (Time.time > nextActiveMoveTime)
        {
            if (Random.value < 0.5f)
            {
                Strafe();
                isStrafing = true;
            }
            else
            {
                SetRandomPatrolTarget();
                agent.SetDestination(patrolTarget);
                isStrafing = false;
            }
            nextActiveMoveTime = Time.time + activeMoveDuration;
        }
        else if (isStrafing)
        {
            Strafe();
        }

        if (!CanSeePlayer())
        {
            StartSearching();
        }
    }

    public void Retreat()
    {
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + retreatDirection * retreatDistance);
        destinationStartTime = Time.time;
    }

    [Button]
    public void Dodge()
    {
        if (!disableLogic && Random.value <= dodgeProbability)
        {
            Vector3 dodgeDirection = Vector3.Cross(Vector3.up, (player.position - transform.position)).normalized;
            dodgeDirection *= Random.Range(0, 2) == 0 ? 1 : -1;
            StartDash(dodgeDirection);
        }
    }

    [Button]
    public void StartDash(Vector3 direction)
    {
        if (!disableLogic && !isDashing)
        {
            isDashing = true;
            dashDirection = direction.normalized * dodgeDistance;
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        float dashStartTime = Time.time;
        while (Time.time < dashStartTime + (dodgeDistance / dashSpeed))
        {
            agent.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
        isDashing = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (disableLogic) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            currentState = EnemyState.Dodging;
        }
    }

    public bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, player.position, out hit, obstacleLayer))
        {
            if (hit.transform != player)
            {
                return false;
            }
        }
        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, strafeDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, dodgeDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, approachDistance);
    }

    void Strafe()
    {
        Vector3 strafeDirection = Vector3.Cross(Vector3.up, (player.position - transform.position)).normalized;
        agent.SetDestination(transform.position + strafeDirection * strafeDistance);
        destinationStartTime = Time.time;
    }

    public void LookAtDestination()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
    }
}
