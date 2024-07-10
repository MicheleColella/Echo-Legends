// InteractionDroppedWeapon.cs
using UnityEngine;
using System.Collections;

public class InteractionDroppedWeapon : MonoBehaviour, IInteractionHandler
{
    public string interactionMessage;
    public WeaponData weaponData; // Dati dell'arma che può essere raccolta

    private WeaponSystem weaponSystem;
    public Animator animator; // Riferimento all'animator per gestire le animazioni

    private void Start()
    {
        weaponSystem = FindObjectOfType<WeaponSystem>();
        if (weaponSystem == null)
        {
            Debug.LogError("WeaponSystem non trovato nella scena.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator non trovato sul prefab dell'arma.");
        }
        else
        {
            animator.Play("ScaleUp");
        }
    }

    public void HandleInteraction(InteractableObject interactableObject)
    {
        PickupWeapon();
    }

    private void PickupWeapon()
    {
        if (weaponSystem != null && weaponData != null)
        {
            StartCoroutine(MoveToPlayerAndAnimate());
        }
        else
        {
            Debug.LogError("WeaponSystem o WeaponData non assegnati.");
        }
    }

    private IEnumerator MoveToPlayerAndAnimate()
    {
        float duration = 0.5f; // Durata dell'animazione di movimento
        float elapsed = 0f;

        // Avvia l'animazione ScaleDown
        if (animator != null)
        {
            animator.Play("ScaleDown");
        }

        // Movimento dell'oggetto verso il giocatore
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 targetPosition = new Vector3(weaponSystem.transform.position.x, weaponSystem.transform.position.y + 1.0f, weaponSystem.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            yield return null;
        }

        /*
        // Attendi la durata dell'animazione ScaleDown
        if (animator != null)
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - duration);
        }
        */


        weaponSystem.PickupWeapon(weaponData);
        Debug.Log(interactionMessage);
        Destroy(gameObject); // Distrugge il prefab dell'arma a terra
    }
}
