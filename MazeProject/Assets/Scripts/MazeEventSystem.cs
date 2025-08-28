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
    private string sessionFileName;
    public MapLoader mapLoader;
    public GameObject mazeGroup;

    private string nextpage;

    private bool isPaused;
    private bool isLearning = false;
    private bool isPerforming = false;
    private bool isPracticing = false;
    public int mazenum;
    public int mazeindex;
    public int maze_no;
    public GameObject leftline;
    public GameObject rightline;
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
    List<int> MainPhaseList = new List<int> { };
    public GameObject UImage_Q;
    public GameObject UImage_I;
    public GameObject UICanvas;
    public GameObject player;
    public GameObject Timer;

    public TMP_Text screenText;
    public GameObject BreakButton;
    public float targetTime;

    public ToggleGroup toggleGroup;
    public List<Toggle> toggles = new List<Toggle>();
    public bool enter;
    public bool breakOver;
    private string userName = "DefaultUser";

    public GameObject TimeCanvas;
    public GameObject chooseSessionType;
    
    public GameObject resume_button;
    public Array errors_made;
    public int break_point;
    public List<int> errors= new List<int>();
    // Survey response class
    [System.Serializable]
    public class SurveyResponse
    {
        public int mazeNumber;
        public string questionType;
        public int rating;
        public string timestamp;
        public int maze_errors;
        public SurveyResponse(int mazeNumber, string questionType, int rating,int maze_errors)
        {
            this.mazeNumber = mazeNumber;
            this.questionType = questionType;
            this.rating = rating;
            this.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.maze_errors=maze_errors;
        }
    }

    // In-memory list of responses
    private List<SurveyResponse> surveyResponses = new List<SurveyResponse>();


        void start()
        {
            chooseSessionType.SetActive(true);
        }

        void Start_maze()
        {
            chooseSessionType.SetActive(false);
            enter = false;
            UImage_I = GameObject.FindGameObjectWithTag("UI_I");
            UImage_Q = GameObject.FindGameObjectWithTag("UI_Q");
            UICanvas = GameObject.FindGameObjectWithTag("UIcanvas");
            player = GameObject.FindWithTag("Player");
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
            sessionFileName = $"{userName}_JOL_RCJ_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv"; // NEW
            mazenum = 0;
            Debug.Log(mazenum);
        }


        public void New_session()
        {
            Start_maze();
            breakOver = false;
            Debug.Log("MAZE START!");
        }

    public void Start_again()
    {
        
        resume_button.SetActive(false);
        chooseSessionType.SetActive(true);
        
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
        EnterVoid();
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
            Debug.Log("Learn! " + mazenum);
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
            Debug.Log("Perform! " + mazenum);
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
            EnterVoid();
            RawImage rawImageQ = UImage_Q.GetComponent<RawImage>();
            rawImageQ.texture = JOL;

            BreakButton.SetActive(false);
            resume_button.SetActive(false);
            Pause();
        }

    public void RCJQuestion()
    {
        Debug.Log("RCJ! " + mazenum);
        nextpage = "NextBlock";
        UImage_I.SetActive(false);
        UImage_Q.SetActive(true);
        EnterVoid();
        RawImage rawImageQ = UImage_Q.GetComponent<RawImage>();
        rawImageQ.texture = RCJ;
        BreakButton.SetActive(true);
        
        resume_button.SetActive(false);
        Pause();
            
        }

        public int GetSelectedRating()
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].isOn)
                {
                    int rating = i * 25;
                    Debug.Log("Selected Rating: " + rating + "%");
                    return rating;
                }
            }

            Debug.LogWarning("No rating selected.");
            return -1;
        }

        public void OnPlayerTouchedCheese()
        {
            
            
            
            mazeindex = mapLoader.currentMazeIndex;
            

            if (!hasTriggered)
            {
                Pause();

                if (isLearning)
                {
                    player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                    isLearning = false;
                    nextpage = "JOL";
                    hasTriggered = true;
                                   errors.Add(player.GetComponent<PlayerMovementVR>().errors);
                 player.GetComponent<PlayerMovementVR>().errors=0;
                    JOLQuestion();
                }
                else if (isPerforming)
                {
                    player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                    isPerforming = false;
                    nextpage = "RCJ";
                    hasTriggered = true;
                                   errors.Add(player.GetComponent<PlayerMovementVR>().errors);
                 player.GetComponent<PlayerMovementVR>().errors=0;
                    RCJQuestion();
                }
            }
        }

        public void TimerEnd()
        {
            

            mazeindex = mapLoader.currentMazeIndex;

            
            if (!hasTriggered)
            {
                Pause();
 
                if (isLearning)
                {
                    player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                    isLearning = false;
                    nextpage = "JOL";
                    hasTriggered = true;
                    errors.Add(player.GetComponent<PlayerMovementVR>().errors);
                    player.GetComponent<PlayerMovementVR>().errors=0;
                    JOLQuestion();
                }
                else if (isPerforming)
                {
                    player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
                    isPerforming = false;
                    nextpage = "RCJ";
                    hasTriggered = true;
                    errors.Add(player.GetComponent<PlayerMovementVR>().errors);
                    player.GetComponent<PlayerMovementVR>().errors=0;
                    RCJQuestion();
                }
            }
        }

        public void pressed_enter()
        {
            Debug.Log("enter");
            if (!enter)
            {
                enter = true;
            }
            return;
        }



        public void Update()
        {
            
            Transform cam = Camera.main.transform;

            UICanvas.transform.position = cam.position + cam.forward * 1.0f;
            UICanvas.transform.rotation = Quaternion.LookRotation(cam.forward);

            float canvasDistance = 1.2f;
            Vector3 upwardOffset = new Vector3(0, 0.6f, 0); // timer above head
            Vector3 targetPos = cam.position + cam.forward * canvasDistance;
            TimeCanvas.transform.position = targetPos + upwardOffset;
            TimeCanvas.transform.rotation = Quaternion.LookRotation(cam.forward);

        if (isPaused && enter)
        {
            enter = false;

            if (nextpage == "PerformancePhase")
            {
                RecordAnswerInMemory("JOL", mazeindex, GetSelectedRating());
                hasTriggered = false;
                PerformancePhase();
                return;
            }

            if (nextpage == "NextBlock")
            {
                RecordAnswerInMemory("RCJ", mazeindex, GetSelectedRating());
                hasTriggered = false;
                if (isPracticing)
                {
                    Start_MainMazeBlock();
                }
                else if (mazenum < 19)
                {
                    SaveAllResponsesToCSV(); // Save answers at the end
                    mazenum += 1;
                    mapLoader.LoadNewMaze();
                    LearningPhase();
                    if (mazenum == break_point && breakOver == false)
                    {
                        breakOver = true;
                        takeAbreak();
                    }
                }
                else
                {
                    TextMeshProUGUI tmp = screenText.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.text = "all mazes completed thank you";
                    }
                    else
                    {
                        Debug.LogError("TextMeshProUGUI component not found on screenText!");
                    }
                    SaveAllResponsesToCSV(); // Save answers at the end
                    Debug.Log("ALL MAZES COMPLETED");
                    end_Session();
                }
                return;
            }

            if (nextpage == "LearningPhase")
            {
                BreakButton.SetActive(false);
                LearningPhase();
                return;
            }

            if (nextpage == "resume")
            {
                Resume();
                return;
            }
        }

            if (!isPaused)
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
            TextMeshProUGUI TimeText = Timer.GetComponent<TextMeshProUGUI>();
            TimeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            isPaused = false;
            UICanvas.SetActive(false);
            removeLines();
            ExitVoid();
        }

        void Pause()
        {
            Time.timeScale = 0f;
            isPaused = true;
            UICanvas.SetActive(true);
            addLines();
        }

        public void takeAbreak()
        {
            UImage_I.SetActive(false);
            UImage_Q.SetActive(false);
            resume_button.SetActive(true);
            Pause();
        }
    public void end_Session()
    {
        UImage_I.SetActive(false);
        UImage_Q.SetActive(false);
        resume_button.SetActive(false);
        Pause();
    }
    
        public void leavebreak()
    {
        if (mazenum == break_point)
        {
            UImage_I.SetActive(true);
            UImage_Q.SetActive(false);
            resume_button.SetActive(false);
            Pause();
        }
        else
        {
            UImage_I.SetActive(false);
            UImage_Q.SetActive(true);
            resume_button.SetActive(false);
            Pause();
        }

    }

        // Save answers into memory
        void RecordAnswerInMemory(string questionType, int mazeNumber, int rating)
        {
            
            if (mazeNumber == 0 && isPracticing == true)
            {
                mazeNumber = -1;
            }
            surveyResponses.Add(new SurveyResponse(mazeNumber, questionType, rating,errors[maze_no]));
            Debug.Log("ERRORS:"+errors[maze_no]);
            maze_no+=1;
        }

        // Save all responses to CSV
        void SaveAllResponsesToCSV()
        {
            string fileName = sessionFileName;
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("MazeNumber,Condition,QuestionType,Rating,Timestamp,#Errors");
            if (mapLoader.sessionType == 0)
            {
                writer.WriteLine("SESSION TYPE: DISTAL CUES ONLY");
            }
            else
            {
                writer.WriteLine("SESSION TYPE: COMBINED");  
            }
            int error_count=0;
            foreach (var response in surveyResponses)
            {
                if (response.mazeNumber == -1)
                {
                    writer.WriteLine($"Practice_Maze: ,{mapLoader.sessionType},{response.questionType},{response.rating},{response.timestamp}");
                }
                else
                {

                    writer.WriteLine($"Maze#: {response.mazeNumber},{mapLoader.sessionType},{response.questionType},{response.rating},{response.timestamp},{response.maze_errors}");
                    error_count+=1;
                }

            }
            }
            Debug.Log("All responses saved to file: " + filePath);
        }

        void removeLines()
        {
            leftline.SetActive(false);
            rightline.SetActive(false);
        }
        void addLines()
        {
            leftline.SetActive(true);
            rightline.SetActive(true);
        }
        void EnterVoid()
        {
            if (mazeGroup != null)
            {
                mazeGroup.SetActive(false);
            }
        }

        void ExitVoid()
        {
            if (mazeGroup != null)
            {
                mazeGroup.SetActive(true);
            }
        }
    }


