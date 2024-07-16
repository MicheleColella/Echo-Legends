using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;

    [SerializeField] // Aggiungi questo attributo per visualizzare il campo nell'Inspector
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
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
        Destroy(gameObject);
    }
}
