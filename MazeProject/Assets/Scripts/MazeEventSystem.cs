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
    //if the maze was a learning maze the next maze is the same else pick a new maze  and mazenum+1
    void Start()
    {
        UImage_I = GameObject.FindGameObjectWithTag("UI_I");
        UImage_Q = GameObject.FindGameObjectWithTag("UI_Q");
        UICanvas = GameObject.FindGameObjectWithTag("UIcanvas");
        player = GameObject.FindGameObjectWithTag("Player");
        Timer = GameObject.FindGameObjectWithTag("Timer");
        BreakButton = GameObject.FindGameObjectWithTag("BreakButton");
        nextpage = "PracticeBlock";

        UImage_I.SetActive(true);
        UImage_Q.SetActive(false);
        UICanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        RawImage rawImageI = UImage_I.GetComponent<RawImage>();
        rawImageI.texture = MainInstructions;
        Pause();

        //MUST BE LOADED ONCE
        mazenum = 0;
        Debug.Log(mazenum);
    }
    public void Start_PracticeMazeBlock()
    {
        player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
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
        player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
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
        player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
        Pause();
        if (!hasTriggered)
        {
            Resume();
            if (nextpage == "JOL")
            {
                JOLQuestion();
                isLearning = false;
                return;
            }
            if (nextpage == "RCJ")
            {
                RCJQuestion();
                isPerforming = false;
                return;

            }
            hasTriggered = true; // prevent it from repeating
        }
    }
    public void TimerEnd()
    {
        Pause();
        if (!hasTriggered)
        {
            Resume();
            if (nextpage == "JOL")
            {
                JOLQuestion();
                isLearning = false;
                return;
            }
            if (nextpage == "RCJ")
            {
                RCJQuestion();
                isPerforming = false;
                return;

            }
            hasTriggered = true; // prevent it from repeating
        }
    }
    public void Update()
    {
        UICanvas.transform.position = player.transform.position + player.transform.forward * 0.5f;
        Timer.transform.position = player.transform.position + player.transform.forward * 0.5f + Vector3.up * 0.32f+Vector3.left*0.3f;


        // Enter
        if (mazenum >= 20)
        {
            takeAbreak();
            Debug.Log("ALL MAZES COMPLETED");
        }
        
        if ((isPaused && Input.GetKeyDown(KeyCode.Return)))
        {
            if (nextpage == "PracticeBlock")   //from the main instructions
            {
                Start_PracticeMazeBlock();
                hasTriggered = false;
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                return;
            }

            if (nextpage == "LearningPhase")
            {
                BreakButton.SetActive(false);
                Debug.Log("lerm");
                LearningPhase();
                hasTriggered = false;
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                return;
            }

            if (nextpage == "NextBlock" && isPracticing==true) //actual block starts
            {
                BreakButton.SetActive(false);
                Start_MainMazeBlock();
                hasTriggered = false;
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                return;
            }else if (nextpage=="NextBlock" && isPracticing == false) //regular next block
            {

                if (mazenum != 9)
                {
                    BreakButton.SetActive(false);
                    LearningPhase();
                    hasTriggered = false;
                    player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);

                    GetSelectedRating();
                    mazenum += 1;
                    mapLoader.LoadNewMaze();
                    return;
                }
                else if(mazenum == 9)
                {
                    BreakButton.SetActive(true);
                    takeAbreak();
                    hasTriggered = false;
                    player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);

                    GetSelectedRating();

                    mapLoader.LoadNewMaze();
                    return;
                }
                return;
            }


            if (nextpage == "PerformancePhase") //block starts
            {
                GetSelectedRating();
                PerformancePhase();
                hasTriggered = false;
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                return;
            }
            if (nextpage == "resume")
            {
                if (isLearning == true)
                {
                    nextpage = "JOL";
                    
                }
                if (isPerforming == true)
                {
                    nextpage = "RCJ";
                    
                }
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

}
