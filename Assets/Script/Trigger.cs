using UnityEngine;

public class Trigger : MonoBehaviour
{
    [Header("Spawn Position Settings")]
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private float spawnZOffset = 100f;
    [SerializeField] private float spawnXPosition = -59f;
    [SerializeField] private float spawnYPosition = 0f;

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
                spawnXPosition,
                spawnYPosition,
                transform.position.z + spawnZOffset
            );

            GameObject newRoad = Instantiate(roadPrefab, spawnPos, Quaternion.identity);

            // Ensure it has a Move script
            if (!newRoad.GetComponent<RoadMover>())
            {
                newRoad.AddComponent<RoadMover>();
            }
        }
        else
        {
            Debug.LogError("Road prefab not assigned in RoadTrigger!", this);
        }
    }

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
