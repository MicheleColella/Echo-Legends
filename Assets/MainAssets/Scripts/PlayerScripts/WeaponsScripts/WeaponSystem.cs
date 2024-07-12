using UnityEngine;
using MidniteOilSoftware.ObjectPoolManager;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples;
using UnityEngine.UI;
using System;

public class WeaponSystem : MonoBehaviour
{
    public Transform firePoint; // Punto di fuoco
    public ExampleCharacterController playerController; // Riferimento al controller del giocatore
    public Button switchWeaponUIButton; // Pulsante UI per cambiare arma
    public WeaponData[] inventory = new WeaponData[3]; // Inventario delle armi

    public int currentWeaponIndex = 0; // Indice dell'arma attualmente selezionata
    private float nextFireTime = 0f; // Tempo di attesa per il prossimo sparo
    private bool isFiring = false; // Stato di fuoco continuo

    public bool isMobileFiring = false;
    public LayerMask collisionLayers; // LayerMask dei layer con cui i proiettili possono collidere

    private InputActions playerInputActions;

    public int maxAmmo = 420; // Massimo numero di munizioni che il player può avere
    public int currentAmmo = 100; // Munizioni attualmente disponibili

    public event Action OnWeaponChanged; // Evento per notificare il cambio dell'arma

    // Aggiungere un campo per il GameObject target dove istanziare il modello dell'arma melee
    public Transform meleeWeaponParent;

    // Memorizza l'istanza del modello dell'arma melee attualmente equipaggiata
    private GameObject currentMeleeWeaponModel;

    // Riferimento all'Animator del modello dell'arma melee
    private Animator currentMeleeWeaponAnimator;

    private void Awake()
    {
        playerInputActions = new InputActions();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.Player.Fire.performed += OnFireStarted;
        playerInputActions.Player.Fire.canceled += OnFireStopped;
        playerInputActions.Player.SwitchWeapon.performed += OnSwitchWeapon; // Gestisci il cambio arma con "Tab" e "Y"

        if (switchWeaponUIButton != null)
        {
            switchWeaponUIButton.onClick.AddListener(OnSwitchWeaponUI); // Aggiungi listener per il pulsante UI
        }
    }

    private void OnDisable()
    {
        playerInputActions.Player.Fire.performed -= OnFireStarted;
        playerInputActions.Player.Fire.canceled -= OnFireStopped;
        playerInputActions.Player.SwitchWeapon.performed -= OnSwitchWeapon; // Rimuovi gestione cambio arma

        if (switchWeaponUIButton != null)
        {
            switchWeaponUIButton.onClick.RemoveListener(OnSwitchWeaponUI); // Rimuovi listener per il pulsante UI
        }

        playerInputActions.Disable();
    }

    private void Update()
    {
        if (isFiring)
        {
            FireWeapon();
        }

        if (isMobileFiring)
        {
            FireWeapon();
        }
    }

    private void OnFireStarted(InputAction.CallbackContext context)
    {
        var currentState = CheckInputManager.Instance.GetCurrentInputState();

        if ((currentState == CheckInputManager.InputState.MouseAndKeyboard && context.control.device is Mouse) ||
            (currentState == CheckInputManager.InputState.Gamepad && context.control.device is Gamepad))
        {
            isFiring = true;
            // Se l'arma corrente è melee, avvia l'animazione di attacco
            if (currentMeleeWeaponAnimator != null)
            {
                currentMeleeWeaponAnimator.SetBool("isAttacking", true);
            }
        }
    }

    private void OnFireStopped(InputAction.CallbackContext context)
    {
        var currentState = CheckInputManager.Instance.GetCurrentInputState();

        if ((currentState == CheckInputManager.InputState.MouseAndKeyboard && context.control.device is Mouse) ||
            (currentState == CheckInputManager.InputState.Gamepad && context.control.device is Gamepad))
        {
            isFiring = false;
            // Se l'arma corrente è melee, ferma l'animazione di attacco
            if (currentMeleeWeaponAnimator != null)
            {
                currentMeleeWeaponAnimator.SetBool("isAttacking", false);
            }
        }
    }

    public void SelectWeapon(int index)
    {
        if (index >= 0 && index < inventory.Length && inventory[index] != null)
        {
            currentWeaponIndex = index;
            OnWeaponChanged?.Invoke(); // Notifica il cambio dell'arma

            // Gestione del modello dell'arma melee
            HandleMeleeWeaponModel();
        }
    }

    private void HandleMeleeWeaponModel()
    {
        WeaponData currentWeapon = inventory[currentWeaponIndex];

        // Rimuove il modello dell'arma melee corrente se esiste
        if (currentMeleeWeaponModel != null)
        {
            Destroy(currentMeleeWeaponModel);
        }

        // Istanzia il modello dell'arma melee se l'arma corrente è di tipo melee
        if (currentWeapon != null && currentWeapon.weaponModel != null)
        {
            currentMeleeWeaponModel = Instantiate(currentWeapon.weaponModel, meleeWeaponParent);

            // Ottieni l'Animator del modello dell'arma melee
            currentMeleeWeaponAnimator = currentMeleeWeaponModel.GetComponentInChildren<Animator>();

            // Aggiungi il componente MeleeWeapon
            MeleeWeapon meleeWeaponComponent = currentMeleeWeaponModel.AddComponent<MeleeWeapon>();
        }
    }

    public void FireWeapon()
    {
        WeaponData currentWeapon = inventory[currentWeaponIndex];

        // Se l'arma è di tipo melee, non eseguire la logica di sparo dei proiettili
        if (currentWeapon == null || currentWeapon.weaponModel != null)
        {
            return;
        }

        if (currentAmmo < currentWeapon.ammoPerShot)
        {
            return;
        }

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / currentWeapon.fireRate;

            for (int i = 0; i < currentWeapon.projectilesPerShot; i++)
            {
                float angle = CalculateSpreadAngle(i, currentWeapon.projectilesPerShot, currentWeapon.spreadAngle);
                Quaternion rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                Vector3 direction = rotation * firePoint.forward;

                if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z))
                {
                    continue;
                }

                GameObject projectileObject = ObjectPoolManager.SpawnGameObject(currentWeapon.projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
                if (projectileObject == null)
                {
                    continue;
                }

                Rigidbody rb = projectileObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    continue;
                }

                rb.velocity = direction * currentWeapon.projectileSpeed;

                Projectile projectile = projectileObject.GetComponent<Projectile>();
                if (projectile == null)
                {
                    continue;
                }

                projectile.Initialize(currentWeapon.damageRange, transform);
                projectile.collisionLayers = collisionLayers; // Assegna i layer con cui può collidere
            }

            currentAmmo -= currentWeapon.ammoPerShot;
            Vector3 recoilDirection = -firePoint.forward * currentWeapon.recoilForce;
            playerController.AddVelocity(recoilDirection);
        }
    }

    private float CalculateSpreadAngle(int index, int totalProjectiles, float spreadAngle)
    {
        if (totalProjectiles == 1)
        {
            return 0f;
        }

        float step = spreadAngle / (totalProjectiles - 1);
        return -spreadAngle / 2 + step * index;
    }

    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        SwitchToNextWeapon();
    }

    private void OnSwitchWeaponUI()
    {
        SwitchToNextWeapon();
    }

    private void SwitchToNextWeapon()
    {
        int startIndex = currentWeaponIndex;
        do
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % inventory.Length;
        } while (inventory[currentWeaponIndex] == null && currentWeaponIndex != startIndex);

        if (inventory[currentWeaponIndex] != null)
        {
            SelectWeapon(currentWeaponIndex);
        }
    }

    public void PickupWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            return;
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = weapon;
                OnWeaponChanged?.Invoke(); // Notifica il cambio dell'arma
                SelectWeapon(currentWeaponIndex); // Assicurati di selezionare l'arma corrente
                return;
            }
        }

        WeaponData droppedWeapon = inventory[currentWeaponIndex];
        inventory[currentWeaponIndex] = weapon;

        DropCurrentWeapon(droppedWeapon);
        OnWeaponChanged?.Invoke(); // Notifica il cambio dell'arma
        SelectWeapon(currentWeaponIndex); // Assicurati di selezionare l'arma corrente
    }

    private void DropCurrentWeapon(WeaponData droppedWeapon)
    {
        if (droppedWeapon == null || droppedWeapon.weaponPrefab == null)
        {
            return;
        }

        float dropDistance = 1.0f;
        float dropHeight = 0.5f;
        Vector3 dropPosition = playerController.transform.position + playerController.transform.forward * dropDistance + Vector3.up * dropHeight;
        Quaternion dropRotation = Quaternion.Euler(0, -45, 0);

        GameObject droppedWeaponObject = Instantiate(droppedWeapon.weaponPrefab, dropPosition, dropRotation);

        Rigidbody rb = droppedWeaponObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float throwForce = 5f;
            rb.AddForce(playerController.transform.forward * throwForce, ForceMode.Impulse);
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
    }

    // Metodo per ottenere l'indice dell'arma corrente
    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
}
