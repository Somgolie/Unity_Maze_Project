using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelTrigger : MonoBehaviour
{

    public MapLoader mapLoader; // Reference to MapLoader script
    public MazeEventSystem mazeSystem;// Reference to MazeEventSystem script

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the correct tag
        {
            Debug.Log("Player touched the trigger!");
            if (mazeSystem.LearningPhase == true)
            {
                mazeSystem.start_JOC();
                resetplayer(); //restart the maze for the performance phase
                mazeSystem.LearningPhase = false;
            }
            else
            {
                
                mazeSystem.start_RCJ();
                mazeSystem.LearningPhase = true;
                mazeSystem.mazenum++;
                mapLoader.LoadNewMaze();

                

            }

            
           
            
        }
    }
  
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
