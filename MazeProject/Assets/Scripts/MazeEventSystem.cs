using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
public class MazeEventSystem : MonoBehaviour
{
    public MapLoader mapLoader;

    private string nextpage;

    private bool isPaused;
    private bool isLearning=false;
    private bool isPerforming=false;
    private bool isPracticing = false;
    public int mazenum;//mazenum is the number of mazes the player has completed
    public int mazeindex;//mazeindex is the current maze completed(the current maze is saved just in case)
    //UI
    [SerializeField]
    public TextAsset PracticeMaze;

    [SerializeField]
    public Texture MainInstructions;
    [SerializeField]
    public Texture PracticeBlock;

    [SerializeField]
    public Texture LearningInstructions;
    [SerializeField]
    public Texture PerformanceInstructions;
    [SerializeField]
    public Texture JOL;
    [SerializeField]
    public Texture RCJ;
    [SerializeField]
    public Texture MainBlock;

    private bool hasTriggered = false;

    List<int> PracPhaseList = new List<int> { 0, 1 };
    List<int> MainPhaseList = new List<int> {};
    public GameObject UImage_Q;
    public GameObject UImage_I;
    public GameObject UICanvas;
    public GameObject player;
    public GameObject Timer;

    public GameObject BreakButton;
    public float targetTime;

    public ToggleGroup toggleGroup;
    public List<Toggle> toggles = new List<Toggle>();
    public bool enter;
    //if the maze was a learning maze the next maze is the same else pick a new maze  and mazenum+1
    void Start()
    {
        enter=false;
        UImage_I = GameObject.FindGameObjectWithTag("UI_I");
        UImage_Q = GameObject.FindGameObjectWithTag("UI_Q");
        UICanvas = GameObject.FindGameObjectWithTag("UIcanvas");
        player = GameObject.FindGameObjectWithTag("Player");
        Timer = GameObject.FindGameObjectWithTag("Timer");
        BreakButton = GameObject.FindGameObjectWithTag("BreakButton");
        player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
        Start_PracticeMazeBlock();
        BreakButton.SetActive(false);
        UImage_I.SetActive(true);
        UImage_Q.SetActive(false);
        UICanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        RawImage rawImageI = UImage_I.GetComponent<RawImage>();
        rawImageI.texture = MainInstructions;

        //MUST BE LOADED ONCE
        mazenum = 0;
        Debug.Log(mazenum);
    }
    public void Start_PracticeMazeBlock()
    {
        
        isPracticing = true;
        Debug.Log("Prac!");
        nextpage = "LearningPhase";

        UImage_I.SetActive(true);
        UImage_Q.SetActive(false);

        RawImage rawImageI = UImage_I.GetComponent<RawImage>();
        rawImageI.texture = PracticeBlock;
        mapLoader.LoadPracticeMaze();
        Pause();
    }
    public void Start_MainMazeBlock()
    {
        
        isPracticing = false;
        Debug.Log("Action!");
        nextpage = "LearningPhase";

        
        UImage_I.SetActive(true);
        UImage_Q.SetActive(false);
        
        RawImage rawImageI = UImage_I.GetComponent<RawImage>();
        rawImageI.texture = MainBlock;
        mapLoader.LoadNewMaze();
        Pause();

    }
    public void LearningPhase()
    {
        Debug.Log("Learn! "+mazenum);
        nextpage = "resume";
        isLearning = true;
        isPerforming = false;
        targetTime = 45.0f;


        UImage_I.SetActive(true);
        UImage_Q.SetActive(false);

        RawImage rawImageI = UImage_I.GetComponent<RawImage>();
        rawImageI.texture = LearningInstructions;

        Pause();
    }
    public void PerformancePhase()
    {

        Debug.Log("Perform! "+mazenum);
        nextpage = "resume";
        isLearning = false;
        isPerforming = true;
        targetTime = 240.0f;

        UImage_I.SetActive(true);
        UImage_Q.SetActive(false);

        RawImage rawImageI = UImage_I.GetComponent<RawImage>();
        rawImageI.texture = PerformanceInstructions;
        Pause();
    }
    public void JOLQuestion()
    {
        Debug.Log("JOL! " + mazenum);
        nextpage = "PerformancePhase";

        UImage_I.SetActive(false);
        UImage_Q.SetActive(true);

        RawImage rawImageQ = UImage_Q.GetComponent<RawImage>();
        rawImageQ.texture = JOL;
        Pause();
    }
    public void RCJQuestion()
    {
        Debug.Log("RCJ! " + mazenum);
        nextpage = "NextBlock";

        UImage_I.SetActive(false);
        UImage_Q.SetActive(true);

        RawImage rawImageQ = UImage_Q.GetComponent<RawImage>();
        rawImageQ.texture = RCJ;
        BreakButton.SetActive(true);
        Pause();

    }
    public int GetSelectedRating()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
            {
                int rating = i * 25; // 0%, 25%, 50%, 75%, 100%
                Debug.Log("Selected Rating: " + rating + "%");
                return rating;
            }
        }

        Debug.LogWarning("No rating selected.");
        return -1; // or some sentinel value
    }
    public void OnPlayerTouchedCheese()
    {
        if (!hasTriggered)
        {
            Pause();
            
            if (isLearning)
            {
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                isLearning = false;
                nextpage = "JOL";
                hasTriggered = true;
                JOLQuestion();
            }
            else if (isPerforming)
            {
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                isPerforming = false;
                nextpage = "RCJ";
                hasTriggered = true;
                RCJQuestion();
            }

            
        }
    }
    public void TimerEnd()
    {
        if (!hasTriggered)
        {
            Pause();

            if (isLearning)
            {
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                isLearning = false;
                nextpage = "JOL";
                hasTriggered = true;
                JOLQuestion();
            }
            else if (isPerforming)
            {
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                isPerforming = false;
                nextpage = "RCJ";
                hasTriggered = true;
                RCJQuestion();
            }

            
        }
    }
     public void pressed_enter()
     {
         Debug.Log("enter");
         if (!enter)
         {
             enter=true;
         }
         return;
     }

    public void Update()
    {
        UICanvas.transform.position = player.transform.position + player.transform.forward * 0.5f;
        Timer.transform.position = player.transform.position + player.transform.forward * 0.5f + Vector3.up * 0.32f+Vector3.left*0.3f;

        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     pressed_enter();
        // }
        // Enter
        if (mazenum >= 20)
        {
            takeAbreak();
            Debug.Log("ALL MAZES COMPLETED");
        }
        //Input.GetKeyDown(KeyCode.Return))
        if (isPaused && enter)
        {
            enter = false;

            // Player just finished JOL -> go to Performance
            if (nextpage == "PerformancePhase")
            {
                SaveAnswerToFile("JOL", mazenum, GetSelectedRating());
                hasTriggered = false;
                PerformancePhase();
                return;
            }

            // Player just finished RCJ -> go to next learning or finish
            if (nextpage == "NextBlock")
            {
                SaveAnswerToFile("RCJ", mazenum, GetSelectedRating());
                hasTriggered = false;
                if (isPracticing)
                {
                    // End of practice block, start real
                    Start_MainMazeBlock();
                }
                else if (mazenum < 20)
                {
                    mazenum += 1;
                    mapLoader.LoadNewMaze();
                    LearningPhase();
                }
                else
                {
                    // All real mazes completed
                    takeAbreak();
                    Debug.Log("ALL MAZES COMPLETED");
                }
                return;
            }


            // Start learning
            if (nextpage == "LearningPhase")
            {
                BreakButton.SetActive(false);
                LearningPhase();
                return;
            }

            // Resume after instruction screen
            if (nextpage == "resume")
            {
                Resume();
                
                return;
            }
        }
        if (isPaused == false)
        {
            targetTime -= Time.deltaTime;
            updateTimer(targetTime);
        }
        if (targetTime <= 0.0f)
        {
     
            Pause();
            TimerEnd();
            
        }
      
    }
    void updateTimer(float currentTime)
    {
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        TextMeshProUGUI TimeText= Timer.GetComponent<TextMeshProUGUI>();
        TimeText.text = string.Format("{0:00} : {1:00}",minutes,seconds);
    }

    public void Resume()
    {

            Time.timeScale = 1f;
            isPaused = false;
            UICanvas.SetActive(false);
            


    }

    void Pause()
    {
        
        Time.timeScale = 0f;
        isPaused = true;

        UICanvas.SetActive (true);
    }

    public void takeAbreak()
    {
        UImage_I.SetActive(false);
        UImage_Q.SetActive(false);
        Pause();
    }
    public void leavebreak()
    {
        UImage_I.SetActive(false);
        UImage_Q.SetActive(true);
        Pause();
    }
    void SaveAnswerToFile(string questionType, int mazeNumber, int rating)
    {
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string fileName = "MazeSurveyResults.txt";
        string filePath = Path.Combine(desktopPath, fileName);

        string entry = $"Maze {mazeNumber}, {questionType}: {rating}% at {System.DateTime.Now}\n";

        File.AppendAllText(filePath, entry);
        Debug.Log("Answer saved to file: " + filePath);
    }
}
