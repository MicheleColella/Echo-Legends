using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using System.Collections.Generic;

public class WeaponUIManager : MonoBehaviour
{
    public WeaponSystem weaponSystem; // Riferimento allo script WeaponSystem
    public TextMeshProUGUI ammoText; // Riferimento al TextMeshPro per le munizioni
    public Image[] weaponIcons; // Riferimento agli Image per gli sprite delle armi

    public MMF_Player[] toMainTransitions; // Array per i feedback delle transizioni verso l'icona principale
    public MMF_Player[] toLeftTransitions; // Array per i feedback delle transizioni verso l'icona sinistra
    public MMF_Player[] toRightTransitions; // Array per i feedback delle transizioni verso l'icona destra

    public List<Image> weaponImages; // Lista delle immagini delle armi

    private int lastWeaponIndex = -1; // Per tenere traccia dell'ultima arma

    private void Start()
    {
        weaponSystem.OnWeaponChanged += UpdateWeaponIcons;
        UpdateWeaponIcons();
    }

    private void OnDestroy()
    {
        weaponSystem.OnWeaponChanged -= UpdateWeaponIcons;
    }

    private void Update()
    {
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        // Aggiorna il testo delle munizioni con formato a 3 cifre
        ammoText.text = $"{weaponSystem.currentAmmo.ToString("D3")}/{weaponSystem.maxAmmo.ToString("D3")}";
    }

    public void UpdateWeaponIcons()
    {
        int currentWeaponIndex = weaponSystem.GetCurrentWeaponIndex();
        int nextWeaponIndex = (currentWeaponIndex + 1) % weaponSystem.inventory.Length;
        int prevWeaponIndex = (currentWeaponIndex - 1 + weaponSystem.inventory.Length) % weaponSystem.inventory.Length;

        // Aggiorna gli sprite delle icone
        for (int i = 0; i < weaponSystem.inventory.Length; i++)
        {
            if (weaponSystem.inventory[i] != null)
            {
                weaponIcons[i].sprite = weaponSystem.inventory[i].weaponSprite;
                weaponIcons[i].enabled = true;
            }
            else
            {
                weaponIcons[i].sprite = null;
                weaponIcons[i].enabled = false;
            }
        }

        // Solo se l'arma è cambiata eseguiamo le transizioni
        if (currentWeaponIndex != lastWeaponIndex)
        {
            if (lastWeaponIndex != -1) // Assicuriamoci che non sia la prima chiamata all'avvio
            {
                toMainTransitions[currentWeaponIndex].PlayFeedbacks();
                toLeftTransitions[nextWeaponIndex].PlayFeedbacks();
                toRightTransitions[prevWeaponIndex].PlayFeedbacks();
            }
            lastWeaponIndex = currentWeaponIndex;
        }

        UpdateSiblingOrder(currentWeaponIndex, nextWeaponIndex, prevWeaponIndex);
    }

    private void UpdateSiblingOrder(int currentWeaponIndex, int nextWeaponIndex, int prevWeaponIndex)
    {
        // Assicurati che l'arma corrente sia sopra le altre
        weaponImages[currentWeaponIndex].transform.SetSiblingIndex(2); // Portalo in cima
        weaponImages[nextWeaponIndex].transform.SetSiblingIndex(1); // Porta il prossimo in mezzo
        weaponImages[prevWeaponIndex].transform.SetSiblingIndex(0); // Porta il precedente in fondo
    }
}
