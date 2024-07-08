using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponUIManager : MonoBehaviour
{
    public WeaponSystem weaponSystem; // Riferimento allo script WeaponSystem
    public TextMeshProUGUI ammoText; // Riferimento al TextMeshPro per le munizioni
    public Image[] weaponIcons; // Riferimento agli Image per gli sprite delle armi

    private void Update()
    {
        UpdateAmmoUI();
        UpdateWeaponIcons();
    }

    private void UpdateAmmoUI()
    {
        // Aggiorna il testo delle munizioni
        ammoText.text = $"{weaponSystem.currentAmmo}/{weaponSystem.maxAmmo}";
    }

    private void UpdateWeaponIcons()
    {
        // Aggiorna gli sprite delle armi e evidenzia l'arma corrente
        for (int i = 0; i < weaponSystem.inventory.Length; i++)
        {
            if (weaponSystem.inventory[i] != null)
            {
                weaponIcons[i].sprite = weaponSystem.inventory[i].weaponSprite; // Assumi che WeaponData abbia un campo weaponSprite
                weaponIcons[i].color = (i == weaponSystem.GetCurrentWeaponIndex()) ? Color.cyan : Color.white;
                weaponIcons[i].transform.localScale = (i == weaponSystem.GetCurrentWeaponIndex()) ? Vector3.one * 1.2f : Vector3.one;
            }
            else
            {
                weaponIcons[i].sprite = null;
                weaponIcons[i].color = Color.clear;
            }
        }
    }
}
