using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;

    [SerializeField]
    private int currentHealth;

    private EnemyDropSystem dropSystem;
    public Animator animator;

    private bool isDying = false;

    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;
    private Collider enemyCollider;
    private EnemyAnimatorController enemyAnimatorController;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    [Header("Death Settings")]
    public bool destroyAfterDeath = true;
    public string deathAnimationName = "Dam_StandDie"; // Nome dell'animazione di morte

    void Start()
    {
        currentHealth = maxHealth;
        dropSystem = GetComponent<EnemyDropSystem>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyCollider = GetComponent<Collider>();
        enemyAnimatorController = GetComponent<EnemyAnimatorController>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDying) return;

        isDying = true;
        //Debug.Log("Enemy is dying");

        if (dropSystem != null)
        {
            dropSystem.DropItems();
        }

        // Set enemy state to Dying
        if (enemyMovement != null)
        {
            enemyMovement.currentState = EnemyState.Dying;
            enemyMovement.disableLogic = true; // Disabilita la logica di movimento
        }

        if (enemyAttack != null)
        {
            enemyAttack.StopBurst();
            enemyAttack.enabled = false;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        animator.SetTrigger("Die");
        float deathAnimationDuration = GetAnimationClipDuration(deathAnimationName);

        // Start coroutine to reset shoot layer weight and destroy or disable object
        StartCoroutine(ResetShootLayerWeightAndHandleDeath(deathAnimationDuration));
    }

    private float GetAnimationClipDuration(string clipName)
    {
        if (animator == null) return 0f;

        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    private IEnumerator ResetShootLayerWeightAndHandleDeath(float waitTime)
    {
        float elapsedTime = 0f;
        float initialWeight = animator.GetLayerWeight(animator.GetLayerIndex("ShootingLayer"));

        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(initialWeight, 0f, elapsedTime / waitTime);
            animator.SetLayerWeight(animator.GetLayerIndex("ShootingLayer"), newWeight);
            yield return null;
        }

        animator.SetLayerWeight(animator.GetLayerIndex("ShootingLayer"), 0f);

        if (destroyAfterDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
