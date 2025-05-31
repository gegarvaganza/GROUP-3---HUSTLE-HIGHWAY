using UnityEngine;

public class RoadTrigger : MonoBehaviour
{
    [Header("Spawn Position Settings")]
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private float spawnZOffset = 100f;  // Distance ahead to spawn
    [SerializeField] private float spawnXPosition = -59f; // Adjustable X-axis position
    [SerializeField] private float spawnYPosition = 0f;   // Adjustable Y-axis (height)

    private bool _hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasSpawned && other.CompareTag("Player"))
        {
            SpawnNewRoad();
            _hasSpawned = true;
        }
    }

    private void SpawnNewRoad()
    {
        if (roadPrefab != null)
        {
            Vector3 spawnPos = new Vector3(
                spawnXPosition,  // Use custom X position
                spawnYPosition,  // Use custom Y position
                transform.position.z + spawnZOffset  // Spawn ahead of current road
            );
            
            Instantiate(roadPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Road prefab not assigned in RoadTrigger!", this);
        }
    }

    // Visualize spawn position in Scene view (for debugging)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 spawnPos = new Vector3(
            spawnXPosition,
            spawnYPosition,
            transform.position.z + spawnZOffset
        );
        Gizmos.DrawWireCube(spawnPos, new Vector3(10f, 0.1f, 10f));
    }
}