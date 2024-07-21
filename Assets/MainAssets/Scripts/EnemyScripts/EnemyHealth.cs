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

    [Header("Damage Flash Settings")]
    public Material damageFlashMaterial;
    public float flashDuration = 0.1f;

    private Renderer[] childRenderers;
    private Material[][] originalMaterials;

    void Start()
    {
        currentHealth = maxHealth;
        dropSystem = GetComponent<EnemyDropSystem>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyCollider = GetComponent<Collider>();
        enemyAnimatorController = GetComponent<EnemyAnimatorController>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        enemyMovement.player = GameObject.FindWithTag("Player").transform;
        enemyAttack.player = enemyMovement.player;

        childRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[childRenderers.Length][];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            originalMaterials[i] = childRenderers[i].materials;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        SetMaterial(damageFlashMaterial);
        yield return new WaitForSeconds(flashDuration);
        ResetMaterial();
    }

    private void SetMaterial(Material material)
    {
        foreach (var renderer in childRenderers)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }
            renderer.materials = materials;
        }
    }

    private void ResetMaterial()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            childRenderers[i].materials = originalMaterials[i];
        }
    }

    void Die()
    {
        if (isDying) return;

        isDying = true;

        if (dropSystem != null)
        {
            dropSystem.DropItems();
        }

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

        StartCoroutine(HandleDeath(deathAnimationDuration));
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

    private IEnumerator HandleDeath(float waitTime)
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
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }

            if (enemyMovement != null)
            {
                enemyMovement.enabled = false;
            }

            if (enemyAttack != null)
            {
                enemyAttack.enabled = false;
            }

            if (enemyAnimatorController != null)
            {
                enemyAnimatorController.enabled = false;
            }
        }
    }
}
