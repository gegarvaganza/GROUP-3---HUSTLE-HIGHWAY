using UnityEngine;

public class RoadMover : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance != null)
        {
            transform.Translate(Vector3.back * GameManager.Instance.currentSpeed * Time.deltaTime);
        }

        if (transform.position.z < -100f)
        {
            Destroy(gameObject);
        }
    }
}
