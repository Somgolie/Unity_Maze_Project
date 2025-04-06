using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
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
    public Texture MainBlock;

    private bool hasTriggered = false;

    List<int> PracPhaseList = new List<int> { 0, 1 };
    List<int> MainPhaseList = new List<int> {};
    public GameObject UImage;
    public GameObject UICanvas;
    public GameObject player;
    public GameObject Timer;
    public float targetTime;
    //if the maze was a learning maze the next maze is the same else pick a new maze  and mazenum+1
    void Start()
    {
        UImage = GameObject.FindGameObjectWithTag("UI");
        UICanvas = GameObject.FindGameObjectWithTag("UIcanvas");
        player = GameObject.FindGameObjectWithTag("Player");
        Timer = GameObject.FindGameObjectWithTag("Timer");
        nextpage = "PracticeBlock";

        RawImage rawImage = UImage.GetComponent<RawImage>();
        rawImage.texture = MainInstructions;
        Pause();

        //MUST BE LOADED ONCE
        mazenum = 0;
        Debug.Log(mazenum);
    }
    public void Start_PracticeMazeBlock()
    {
        isPracticing = true;
        Debug.Log("Prac!");
        nextpage = "LearningPhase";
        RawImage rawImage = UImage.GetComponent<RawImage>();
        rawImage.texture = PracticeBlock;
        mapLoader.LoadPracticeMaze();
        Pause();
    }
    public void Start_MainMazeBlock()
    {
        player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
        isPracticing = false;
        Debug.Log("Action!");
        nextpage = "LearningPhase";
        RawImage rawImage = UImage.GetComponent<RawImage>();
        rawImage.texture = MainBlock;
        mapLoader.LoadNewMaze();
        Pause();

    }
    public void LearningPhase()
    {
        Debug.Log("Learn! "+mazenum);
        nextpage = "resume";
        isLearning = true;
        targetTime = 45.0f;
        RawImage rawImage = UImage.GetComponent<RawImage>();
        rawImage.texture = LearningInstructions;
        Pause();
    }
    public void PerformancePhase()
    {
        Debug.Log("Perform! "+mazenum);
        nextpage = "resume";
        isPerforming = true;
        targetTime = 240.0f;
        RawImage rawImage = UImage.GetComponent<RawImage>();
        rawImage.texture = PerformanceInstructions;
        Pause();
    }
    public void OnPlayerTouchedCheese()
    {
        Pause();
        if (!hasTriggered)
        {
            Resume();
            if (nextpage == "resume" && isLearning == true)
            {
                PerformancePhase();
                isLearning = false;
                return;
            }
            if (nextpage == "resume" && isPerforming == true)
            {
                if (isPracticing == true)
                {
                    Start_MainMazeBlock();
                    return;
                }
                else
                {
                    if (mazenum != 10)
                    {
                        mazenum += 1;
                        LearningPhase();
                        mapLoader.LoadNewMaze();
                        isPerforming = false;
                        return;
                    }

                }

            }
            hasTriggered = true; // prevent it from repeating
        }
    }
    public void TimerEnd()
    {
        if (!hasTriggered)
        {
            Resume();
            if (nextpage == "resume" && isLearning == true)
            {
                PerformancePhase();
                isLearning = false;
                return;
            }
            if (nextpage == "resume" && isPerforming == true)
            {
                if (isPracticing == true)
                {
                    Start_MainMazeBlock();
                    return;
                }
                else
                {
                    if (mazenum != 10)
                    {
                        mazenum += 1;
                        LearningPhase();
                        mapLoader.LoadNewMaze();
                        isPerforming = false;
                        return;
                    }

                }

            }
            hasTriggered = true; // prevent it from repeating
        }
    }
    public void Update()
    {
        // Enter
        if ((isPaused && Input.GetKeyDown(KeyCode.Return)))
        {
            if (nextpage == "PracticeBlock")   //from the main instructions
            {
                Start_PracticeMazeBlock();
                hasTriggered = false;
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                return;
            }
            if (nextpage == "LearningPhase") //block starts
            {

                LearningPhase();
                hasTriggered = false;
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                return;
            }
            if (nextpage == "resume")
            {
                Resume();
                hasTriggered = false;
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
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
            player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
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
        //basically the pause menu
        Pause();
        
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
