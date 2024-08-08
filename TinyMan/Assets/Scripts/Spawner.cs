using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject SpawnItemPrefab; 
    private Transform SpawnLocation; 

    private GameObject CurrentSpawnedItem { get; set; }

    private void Awake()
    {
        SpawnLocation = transform;
    }

    public void CreateEntity()
    {
        DestroyCurrentItem();

        if (SpawnItemPrefab != null && SpawnLocation != null)
        {
            CurrentSpawnedItem = Instantiate(SpawnItemPrefab, SpawnLocation.position, SpawnLocation.rotation);
        }
        else
        {
            Debug.LogWarning("SpawnItemPrefab or SpawnLocation is not assigned.");
        }
    }

    private void DestroyCurrentItem()
    {
        if (!CurrentSpawnedItem) return;
        Destroy(CurrentSpawnedItem);
    }
}