// File: EnemyAI.cs
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;
    private Transform player;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        player = enemyMovement.player;
    }

    void Update()
    {
        if (enemyMovement.disableLogic || enemyMovement.currentState == EnemyState.Dying)
        {
            Debug.Log("Enemy is in Dying state or logic is disabled");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (enemyMovement.CanSeePlayer())
        {
            if (distanceToPlayer < enemyMovement.strafeDistance)
            {
                enemyMovement.currentState = EnemyState.ActiveMove;
            }
            else if (distanceToPlayer < enemyMovement.dodgeDistance)
            {
                // Dodge quando il proiettile del player è vicino
            }
            else if (distanceToPlayer < enemyMovement.retreatDistance)
            {
                if (enemyMovement.currentState != EnemyState.Retreating)
                {
                    enemyMovement.currentState = EnemyState.Retreating;
                }
            }
            else
            {
                if (enemyMovement.currentState != EnemyState.Chasing)
                {
                    enemyMovement.currentState = EnemyState.Chasing;
                }
            }

            if (distanceToPlayer < enemyMovement.retreatDistance)
            {
                if (!enemyAttack.IsBursting())
                {
                    enemyAttack.StartBurst();
                }
            }
        }
        else
        {
            if (enemyMovement.currentState == EnemyState.Chasing && Time.time > enemyMovement.chaseStartTime + enemyMovement.chaseDuration)
            {
                enemyMovement.StartSearching();
            }
            else if (enemyMovement.currentState == EnemyState.Searching && Time.time > enemyMovement.searchStartTime + enemyMovement.searchDuration)
            {
                enemyMovement.StartPatrolling();
            }

            enemyAttack.StopBurst();
        }

        if (enemyMovement.currentState == EnemyState.Searching)
        {
            enemyMovement.LookAtDestination();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (enemyMovement.disableLogic || enemyMovement.currentState == EnemyState.Dying)
        {
            Debug.Log("Enemy is in Dying state or logic is disabled");
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            enemyMovement.currentState = EnemyState.Dodging;
        }
    }
}
