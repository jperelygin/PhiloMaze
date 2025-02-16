using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private string language = "";
    [SerializeField, ReadOnly]
    private int cols;
    [SerializeField, ReadOnly]
    private int rows;
    [SerializeField, ReadOnly]
    private int numberOfKeys;
    [SerializeField] private Button rusLangButton;
    [SerializeField] private Button engLangButton;
    [SerializeField] private Button difficultyEasyButton;
    [SerializeField] private Button difficultyHardButton;
    [SerializeField] private Button difficultyInsaneButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI dificultyText;

    private string EASY_RUS = "Легко";
    private string HARD_RUS = "Сложно";
    private string INSANE_RUS = "Невозможно";
    private string DIFICULTY_RUS = "Сложность";
    private string START_RUS = "Старт";
    private string QUIT_RUS = "Выход";
    private string EASY_ENG = "Easy";
    private string HARD_ENG = "Hard";
    private string INSANE_ENG = "Insane";
    private string DIFICULTY_ENG = "Difficulty";
    private string START_ENG = "Start";
    private string QUIT_ENG = "Quit";

    private Color INACTIVE_COLOR = Color.white;
    private Color ACTIVE_COLOR = Color.yellow;

    private ScreenFade fade;

    public void SetLanguage(string language)
    {
        if (language != null)
        {
            this.language = language;
        }
        if (language == "rus")
        {
            difficultyEasyButton.GetComponentInChildren<TextMeshProUGUI>().text = EASY_RUS;
            difficultyHardButton.GetComponentInChildren<TextMeshProUGUI>().text = HARD_RUS;
            difficultyInsaneButton.GetComponentInChildren<TextMeshProUGUI>().text = INSANE_RUS;
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = START_RUS;
            quitButton.GetComponentInChildren<TextMeshProUGUI>().text = QUIT_RUS;
            dificultyText.text = DIFICULTY_RUS;
        }
        if (language == "eng")
        {
            difficultyEasyButton.GetComponentInChildren<TextMeshProUGUI>().text = EASY_ENG;
            difficultyHardButton.GetComponentInChildren<TextMeshProUGUI>().text = HARD_ENG;
            difficultyInsaneButton.GetComponentInChildren<TextMeshProUGUI>().text = INSANE_ENG;
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = START_ENG;
            quitButton.GetComponentInChildren<TextMeshProUGUI>().text = QUIT_ENG;
            dificultyText.text = DIFICULTY_ENG;
        }
    }

    private void RusLangPressed()
    {
        SetLanguage("rus");
        ActivateButton(rusLangButton, true);
        ActivateButton(engLangButton, false);
    }

    private void EngLangPressed()
    {
        SetLanguage("eng");
        ActivateButton(rusLangButton, false);
        ActivateButton(engLangButton, true);
    }

    private void EasyButtonPressed()
    {
        ActivateButton(difficultyEasyButton, true);
        ActivateButton(difficultyHardButton, false);
        ActivateButton(difficultyInsaneButton, false);
        cols = 10;
        rows = 10;
        numberOfKeys = 2;
    }
    private void HardButtonPressed()
    {
        ActivateButton(difficultyEasyButton, false);
        ActivateButton(difficultyHardButton, true);
        ActivateButton(difficultyInsaneButton, false);
        cols = 15;
        rows = 15;
        numberOfKeys = 2;
    }
    private void InsaneButtonPressed()
    {
        ActivateButton(difficultyEasyButton, false);
        ActivateButton(difficultyHardButton, false);
        ActivateButton(difficultyInsaneButton, true);
        cols = 20;
        rows = 20;
        numberOfKeys = 3;
    }
    private void StartButtonPressed()
    {
        MazeParameters.rows = rows;
        MazeParameters.cols = cols;
        MazeParameters.numberOfKeys = numberOfKeys;
        MazeParameters.language = language;
        StartCoroutine(fade.Fade(true));
        StartCoroutine(LoadSceneWithDelay(fade.fadeSeconds, "GameScene"));
    }
    private void QuitButtonPressed()
    {
        Application.Quit();
    }

    private void ActivateButton(Button button, bool state)
    {
        if (state)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().color = ACTIVE_COLOR;
        }
        else
        {
            button.GetComponentInChildren<TextMeshProUGUI>().color = INACTIVE_COLOR;
        }
    }

    private IEnumerator LoadSceneWithDelay(float delaySec, string sceneName)
    {
        yield return new WaitForSeconds(delaySec);
        SceneManager.LoadScene(sceneName);
    }

    // Start is called before the first frame update
    void Start()
    {
        fade = GetComponent<ScreenFade>();

        if (language == "")
        {
            if (MazeParameters.language == null)
            {
                language = "eng";
            }
            else 
            {
                language = MazeParameters.language;
            }
        }

        if (rusLangButton != null)
        {
            if (language == "rus")
            {
                RusLangPressed();
            }
            rusLangButton.onClick.AddListener(RusLangPressed);
        }
        if (engLangButton != null)
        {
            if (language == "eng")
            {
                EngLangPressed();
            }
            engLangButton.onClick.AddListener(EngLangPressed);
        }
        if (difficultyEasyButton != null)
        {
            EasyButtonPressed();
            difficultyEasyButton.onClick.AddListener(EasyButtonPressed);
        }
        if (difficultyHardButton != null)
        {
            difficultyHardButton.onClick.AddListener(HardButtonPressed);
        }
        if (difficultyInsaneButton != null)
        {
            difficultyInsaneButton.onClick.AddListener(InsaneButtonPressed);
        }
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartButtonPressed);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitButtonPressed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
