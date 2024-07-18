using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;

    [SerializeField] // Aggiungi questo attributo per visualizzare il campo nell'Inspector
    private int currentHealth;

    private EnemyDropSystem dropSystem;

    void Start()
    {
        currentHealth = maxHealth;
        dropSystem = GetComponent<EnemyDropSystem>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (dropSystem != null)
        {
            dropSystem.DropItems();
        }
        Destroy(gameObject);
    }
}
