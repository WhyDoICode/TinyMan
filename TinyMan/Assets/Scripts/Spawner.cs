using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnItem; // Prefab to spawn
    public Transform spawnLocation; // Location to spawn the item

    public GameObject Current;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void CreateEntity()
    {
        if(Current) Destroy(Current);
        
        spawnLocation = this.gameObject.transform;
        // Check if the spawnItem and spawnLocation are assigned
        if (spawnItem != null && spawnLocation != null)
        {
            // Instantiate the item at the specified location and rotation
            Current = Instantiate(spawnItem, spawnLocation.position, spawnLocation.rotation);
        }
        else
        {
            Debug.LogWarning("SpawnItem or SpawnLocation is not assigned.");
        }
    }
}