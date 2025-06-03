using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float currentSpeed = 5f;
    public float speedIncreaseRate = 0.1f;

    private void Awake()
    {
        // Make sure there's only one instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }

        // Optional: Keep this object across scenes
        // DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Increase speed over time
        currentSpeed += speedIncreaseRate * Time.deltaTime;
    }
}
