using UnityEngine;

public class PlatformChecker : MonoBehaviour
{
    // Variabili statiche per la piattaforma
    public static bool isMobile;
    public static bool isPC;
    public static bool isEditor;

    void Awake()
    {
        // Assicurati che l'oggetto non venga distrutto durante le transizioni di scena
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CheckPlatform();
    }

    void CheckPlatform()
    {
        // Controlla se il gioco è stato avviato su un dispositivo mobile
        if (Application.isMobilePlatform)
        {
            isMobile = true;
            isPC = false;
            isEditor = false;
            Debug.Log("Il gioco è stato avviato su un dispositivo mobile.");
        }
        // Controlla se il gioco è stato avviato su un PC
        else if (Application.platform == RuntimePlatform.WindowsPlayer ||
                 Application.platform == RuntimePlatform.OSXPlayer ||
                 Application.platform == RuntimePlatform.LinuxPlayer)
        {
            isMobile = false;
            isPC = true;
            isEditor = false;
            Debug.Log("Il gioco è stato avviato su un PC.");
        }
        // Controlla se il gioco è in esecuzione nell'editor di Unity
        else if (Application.isEditor)
        {
            isMobile = false;
            isPC = false;
            isEditor = true;
            Debug.Log("Il gioco è in esecuzione nell'editor di Unity.");
        }
        else
        {
            isMobile = false;
            isPC = false;
            isEditor = false;
            Debug.Log("Il gioco è stato avviato su una piattaforma non riconosciuta.");
        }
    }
}
