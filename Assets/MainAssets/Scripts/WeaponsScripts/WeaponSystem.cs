// WeaponSystem.cs
using UnityEngine;
using MidniteOilSoftware.ObjectPoolManager;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples;
using UnityEngine.UI; // Per gestire il pulsante UI

public class WeaponSystem : MonoBehaviour
{
    public WeaponData[] weapons; // Array di ScriptableObject delle armi
    public Transform firePoint; // Punto di fuoco
    public ExampleCharacterController playerController; // Riferimento al controller del giocatore
    public Button switchWeaponUIButton; // Pulsante UI per cambiare arma

    private int currentWeaponIndex = 0; // Indice dell'arma attualmente selezionata
    private float nextFireTime = 0f; // Tempo di attesa per il prossimo sparo
    private bool isFiring = false; // Stato di fuoco continuo

    public bool isMobileFiring = false;
    public LayerMask collisionLayers; // LayerMask dei layer con cui i proiettili possono collidere

    private InputActions playerInputActions;

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
            Debug.Log("Fire started.");
        }
    }

    private void OnFireStopped(InputAction.CallbackContext context)
    {
        var currentState = CheckInputManager.Instance.GetCurrentInputState();

        if ((currentState == CheckInputManager.InputState.MouseAndKeyboard && context.control.device is Mouse) ||
            (currentState == CheckInputManager.InputState.Gamepad && context.control.device is Gamepad))
        {
            isFiring = false;
            Debug.Log("Fire stopped.");
        }
    }

    public void SelectWeapon(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            currentWeaponIndex = index;
            Debug.Log("Selected Weapon: " + weapons[currentWeaponIndex].weaponName);
        }
    }

    public void FireWeapon()
    {
        Debug.Log("Attempting to fire weapon...");
        if (Time.time >= nextFireTime)
        {
            WeaponData currentWeapon = weapons[currentWeaponIndex];
            nextFireTime = Time.time + 1f / currentWeapon.fireRate;

            for (int i = 0; i < currentWeapon.projectilesPerShot; i++)
            {
                float angle = CalculateSpreadAngle(i, currentWeapon.projectilesPerShot, currentWeapon.spreadAngle);
                Quaternion rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                Vector3 direction = rotation * firePoint.forward;

                if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z))
                {
                    Debug.LogError("Direzione del proiettile non valida");
                    continue;
                }

                GameObject projectileObject = ObjectPoolManager.SpawnGameObject(currentWeapon.projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
                if (projectileObject == null)
                {
                    Debug.LogError("Failed to spawn projectile object.");
                    continue;
                }

                Rigidbody rb = projectileObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    Debug.LogError("Rigidbody non trovato sul proiettile");
                    continue;
                }

                rb.velocity = direction * currentWeapon.projectileSpeed;
                Debug.Log("Projectile fired!");

                Projectile projectile = projectileObject.GetComponent<Projectile>();
                if (projectile == null)
                {
                    Debug.LogError("Componente Projectile non trovato sul proiettile");
                    continue;
                }

                projectile.Initialize(currentWeapon.damageRange, transform);
                projectile.collisionLayers = collisionLayers; // Assegna i layer con cui può collidere
            }

            Vector3 recoilDirection = -firePoint.forward * currentWeapon.recoilForce;
            playerController.AddVelocity(recoilDirection);
        }
        else
        {
            Debug.Log("Weapon is cooling down. Next fire time: " + nextFireTime);
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
        // Passa alla prossima arma
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        SelectWeapon(currentWeaponIndex);
    }

    private void OnSwitchWeaponUI()
    {
        // Gestisci il cambio arma tramite il pulsante UI
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        SelectWeapon(currentWeaponIndex);
    }
}
