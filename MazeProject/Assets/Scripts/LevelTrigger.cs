using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelTrigger : MonoBehaviour
{

    public MapLoader mapLoader; // Reference to MapLoader script
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the correct tag
        {
            Debug.Log("Player touched the trigger!");
            mapLoader.LoadNewMaze();
            
        }
    }

    void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load!");
        }
    }
}
