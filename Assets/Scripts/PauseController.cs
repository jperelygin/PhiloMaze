using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    private TextMeshProUGUI pauseLable;
    private Button exitButton;
    private TextMeshProUGUI exitButtonText;

    private string PAUSE_TEXT_RUS = "Пауза";
    private string PAUSE_TEXT_ENG = "Pause";
    private string EXIT_BUTTON_TEXT_RUS = "Выйти в меню";
    private string EXIT_BUTTON_TEXT_ENG = "Exit to menu";

    private void ExitButtonPressed()
    {
        GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
        if (gc != null)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            gc.LoadMenuScene();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseLable = GameObject.Find("PauseLable").GetComponent<TextMeshProUGUI>();
        exitButton = GameObject.Find("ToMenuButton").GetComponent<Button>();
        exitButtonText = exitButton.GetComponentInChildren<TextMeshProUGUI>();

        if (MazeParameters.language == "rus")
        {
            pauseLable.text = PAUSE_TEXT_RUS;
            exitButtonText.text = EXIT_BUTTON_TEXT_RUS;
        }
        else
        {
            pauseLable.text = PAUSE_TEXT_ENG;
            exitButtonText.text = EXIT_BUTTON_TEXT_ENG;
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitButtonPressed);
        }
        else
        {
            Debug.LogError("There is no exitButton");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
