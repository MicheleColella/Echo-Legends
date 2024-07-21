using UnityEngine;
using TMPro;

public class DamageCounter : MonoBehaviour
{
    public GameObject damageCounterPrefab;
    public Color minColor = Color.white;
    public Color maxColor = Color.red;
    public float minFontSize = 14f;
    public float maxFontSize = 28f;
    public float displayDuration = 1f;

    public int minColorValue = 10;
    public int maxColorValue = 100;

    public void ShowDamage(int damage, Vector3 position)
    {
        GameObject counterInstance = Instantiate(damageCounterPrefab, position, Quaternion.identity);
        TextMeshPro tmp = counterInstance.GetComponentInChildren<TextMeshPro>();

        if (tmp != null)
        {
            tmp.text = damage.ToString();
            float damagePercentage = Mathf.InverseLerp(minColorValue, maxColorValue, damage);
            tmp.color = Color.Lerp(minColor, maxColor, damagePercentage);
            tmp.fontSize = Mathf.Lerp(minFontSize, maxFontSize, damagePercentage);
        }

        Animator animator = counterInstance.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.Play("DamageCounterAnimation");
        }

        Destroy(counterInstance, displayDuration);
    }
}
