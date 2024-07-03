using UnityEngine;

public class RaycastMousePosition : MonoBehaviour
{
    void Update()
    {
        // Ottieni la posizione del mouse nello spazio dello schermo
        Vector3 mousePosition = Input.mousePosition;
        // Crea un ray dalla posizione del mouse
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // Esegui il Raycast e ferma il Ray all'altezza 0
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPosition = hit.point;
            hitPosition.y = 0;

            // Visualizza un valore alla posizione del Raycast
            Debug.Log("Raycast Position: " + hitPosition);
        }
    }
}
