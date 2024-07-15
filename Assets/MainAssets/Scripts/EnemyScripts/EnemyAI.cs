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
        if (enemyMovement.disableLogic)
        {
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
                enemyMovement.currentState = EnemyState.Retreating;
            }
            else
            {
                enemyMovement.currentState = EnemyState.Chasing;
            }

            if (distanceToPlayer < enemyMovement.retreatDistance)
            {
                if (!enemyAttack.IsBursting()) // Verifica se non sta già attaccando
                {
                    enemyAttack.StartBurst(); // Inizia l'attacco burst
                }
            }
        }
        else
        {
            enemyAttack.StopBurst();
            enemyMovement.currentState = EnemyState.Patrolling;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (enemyMovement.disableLogic)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            enemyMovement.currentState = EnemyState.Dodging;
        }
    }
}
