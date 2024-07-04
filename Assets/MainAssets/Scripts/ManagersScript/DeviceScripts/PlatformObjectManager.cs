using UnityEngine;
using System.Collections.Generic;

public class PlatformObjectManager : MonoBehaviour
{
    // Liste di GameObject per diverse piattaforme
    public List<GameObject> mobileObjects;
    public List<GameObject> pcObjects;

    void Start()
    {
        UpdatePlatformObjects();
    }

    void UpdatePlatformObjects()
    {
        if (PlatformChecker.isPC || PlatformChecker.isEditor)
        {
            // Attiva gli oggetti per PC e disattiva quelli per Mobile
            SetActiveObjects(pcObjects, true);
            SetActiveObjects(mobileObjects, false);
        }
        else if (PlatformChecker.isMobile)
        {
            // Attiva gli oggetti per Mobile e disattiva quelli per PC
            SetActiveObjects(mobileObjects, true);
            SetActiveObjects(pcObjects, false);
        }
        else
        {
            Debug.LogWarning("Piattaforma non riconosciuta, nessun oggetto sarà attivato/disattivato.");
        }
    }

    void SetActiveObjects(List<GameObject> objects, bool isActive)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
