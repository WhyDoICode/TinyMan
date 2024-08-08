using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    public GameObject spawnItemPrefab;
    private Transform _spawnLocation;
    private GameObject _currentSpawnedItem;

    private void Awake()
    {
        _spawnLocation = transform;
    }

    public void CreateEntity()
    {
        DestroyCurrentItem();

        if (spawnItemPrefab != null && _spawnLocation != null)
        {
            _currentSpawnedItem = Instantiate(spawnItemPrefab, _spawnLocation.position, _spawnLocation.rotation);
        }
        else
        {
            Debug.LogWarning("SpawnItemPrefab or SpawnLocation is not assigned.");
        }
    }

    private void DestroyCurrentItem()
    {
        if (_currentSpawnedItem == null) return;
        Destroy(_currentSpawnedItem);
    }
}