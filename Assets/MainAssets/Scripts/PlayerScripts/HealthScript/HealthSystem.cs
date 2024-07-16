using UnityEngine;
using MoreMountains.Tools;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public MMProgressBar healthBar;
    public RectTransform[] healthRects;

    private bool isInvulnerable = false;

    void Start()
    {
        healthBar.TextValueMultiplier = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
        UpdateRectTransforms();
    }

    public float damageAmount = 10f;
    public float healAmount = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(damageAmount);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(healAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            return; // Non subire danno se invulnerabile
        }

        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateHealthBar();
        CheckDeath();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBar.UpdateBar(currentHealth, 0f, maxHealth);
    }

    void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Add death behavior here (e.g., respawn, game over screen, etc.)
    }

    void UpdateRectTransforms()
    {
        foreach (RectTransform rect in healthRects)
        {
            rect.sizeDelta = new Vector2(maxHealth, rect.sizeDelta.y);
        }
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }
}
