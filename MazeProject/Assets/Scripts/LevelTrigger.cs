using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.Events;
public class LevelTrigger : MonoBehaviour
{

    public MapLoader mapLoader; // Reference to MapLoader script
    public MazeEventSystem mazeSystem;// Reference to MazeEventSystem script
    public UnityEvent nextLevelEvent;

  
    void resetplayer()
    {

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
