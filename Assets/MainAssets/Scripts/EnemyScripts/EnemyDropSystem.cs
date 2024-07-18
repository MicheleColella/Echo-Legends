using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DropItem
{
    public GameObject itemPrefab;
    public int dropChance; // Probabilità di drop (1-100)
}

public class EnemyDropSystem : MonoBehaviour
{
    [Header("Drop Settings")]
    public List<DropItem> dropItems;

    [Header("Drop Quantity")]
    public int maxDropQuantity = 2;

    [Header("Drop Offset")]
    public Vector3 positionOffset = new Vector3(1f, 0.5f, 1f);
    public float launchForce = 5f;

    public void DropItems()
    {
        List<DropItem> itemsToDrop = new List<DropItem>();

        // Aggiungi tutti gli oggetti con probabilità di drop 100
        foreach (var item in dropItems)
        {
            if (item.dropChance == 100)
            {
                itemsToDrop.Add(item);
            }
        }

        // Aggiungi altri oggetti in base alla loro probabilità
        foreach (var item in dropItems)
        {
            if (item.dropChance < 100 && Random.Range(0, 100) < item.dropChance)
            {
                itemsToDrop.Add(item);
                if (itemsToDrop.Count >= maxDropQuantity)
                {
                    break;
                }
            }
        }

        // Limita il numero di oggetti da droppare al massimo consentito
        while (itemsToDrop.Count > maxDropQuantity)
        {
            itemsToDrop.RemoveAt(Random.Range(0, itemsToDrop.Count));
        }

        // Droppa gli oggetti
        foreach (var item in itemsToDrop)
        {
            DropItem(item);
        }
    }

    void DropItem(DropItem item)
    {
        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-positionOffset.x, positionOffset.x),
            positionOffset.y,
            Random.Range(-positionOffset.z, positionOffset.z)
        );

        GameObject droppedItem = Instantiate(item.itemPrefab, spawnPosition, Quaternion.identity);

        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 launchDirection = new Vector3(
                Random.Range(-1f, 1f),
                1f,
                Random.Range(-1f, 1f)
            ).normalized;
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        }
    }
}
