using UnityEngine;

public class MazeEventSystem : MonoBehaviour
{
    public bool LearningPhase;
    public bool PracticePhase;
    public int mazenum;//mazenum is the number of mazes the player has completed
    public int mazeindex;//mazeindex is the current maze completed(the current maze is saved just in case)
    [SerializeField]
    private TextAsset instructions;
    [SerializeField]
    private TextAsset JOC;
    [SerializeField]
    private TextAsset RCJ;

    //if the maze was a learning maze the next maze is the same else pick a new maze  and mazenum+1
    void Start()
    {
        //MUST BE LOADED ONCE
        mazenum = 0;
    }
    public void start_JOC()
    {
        pause_game();
        //show the JOC UI


    }
    public void start_RCJ()
    {
        pause_game();
        //show the RCJ UI
        
        //after the user finishes the RCJ
        if (mazenum == 10)
        {
            takeAbreak();
            LearningPhase = true;
        }
        if (mazenum == 20)
        {
            endSession();
        }
    }
    public void pause_game()
    {
        //playercantmove + nothing is happening
    }
    public void resume_game()
    {
        //playerresumes
    }


    public void takeAbreak()
    {
        //basically the pause menu
        pause_game();
        
    }

    public void newSession()
    {
        //show instructions

    }
    public void endSession()
    {
        //show end title
    }
}
