using UnityEngine;
using UnityEngine.SceneManagement;

public class BusFrontKillZone : MonoBehaviour
{
    // This assumes the front-of-bus box collider is on the same GameObject as this script
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit the front of the bus. Instant death!");
            
            // Option 1: Reset the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Option 2: trigger a game over instead of resetting
            // GameOver();
        }
    }

    // Uncomment if you have a game over system
    /*
    void GameOver()
    {
        Debug.Log("Game Over triggered from bus front!");
        // show UI, stop movement, etc.
    }
    */
}
