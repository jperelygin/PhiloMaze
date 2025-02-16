using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField, ReadOnly] private int keysCollected = 0;
    [SerializeField] private UIController uiController;
    
    private int keysNeeded;

    private MazeGeneratior2 mazeGeneratior2;

    [SerializeField, ReadOnly]
    private float timer = 0f;
    private bool isTimerOn = false;

    public void StartTimer()
    {
        isTimerOn = true;
    }

    public void StopTimer()
    {
        isTimerOn = false;
    }

    public string GetTimerValue()
    {
        return string.Format("{0:00}:{1:00}", Mathf.FloorToInt(timer/60), Mathf.FloorToInt(timer % 60));
    }

    public void onKeyCollected(int keyColor) 
    {
        keysCollected++;
        Debug.Log($"Key collected. Total keys: {keysCollected}");
        uiController.OnKeyPicked(keyColor);
    }

    public bool isAllKeysCollected()
    {
        return keysCollected == keysNeeded;
    }

    private IEnumerator LoadSceneWithDelay(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenuScene()
    {
        StartCoroutine(LoadSceneWithDelay(2f, "Menu"));
        StartCoroutine(uiController.fade.Fade(true));
    }

    // Start is called before the first frame update
    void Start()
    {
        mazeGeneratior2 = FindObjectOfType<MazeGeneratior2>();
        mazeGeneratior2.GenerateMaze(MazeParameters.rows, MazeParameters.cols, MazeParameters.numberOfKeys);
        keysNeeded = MazeParameters.numberOfKeys;
        uiController.SetLanguage(MazeParameters.language);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerOn)
        {
            timer += Time.deltaTime;
        }
    }
}
