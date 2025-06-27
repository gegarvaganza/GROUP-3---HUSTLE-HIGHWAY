using UnityEngine;
using UnityEngine.SceneManagement;

public class AiCloseCheck : MonoBehaviour
{
    public Transform aiTransform;
    public Transform playerTransform;
    public float triggerDistance = 2f;

    void Update()
    {
        float distance = Vector3.Distance(aiTransform.position, playerTransform.position);

        if (distance <= triggerDistance)
        {
            Debug.Log("AI is too close to the player. Game Over or Reset!");
            
            // Option 1: Reset the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Option 2: If you have a game over UI, you could instead
            // call a GameOver() function here.
            // Example:
            // GameOver();
        }
    }

    // Uncomment if you want to display game over instead of reloading
    /*
    void GameOver()
    {
        // Show game over UI
        // Disable player controls, etc.
        Debug.Log("Game Over triggered");
    }
    */
}
