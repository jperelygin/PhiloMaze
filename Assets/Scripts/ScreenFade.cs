using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField]
    private Image fadePanel;
    
    public float fadeSeconds;

    public IEnumerator Fade(bool toDark)
    {
        fadePanel.GetComponent<Image>().enabled = true;

        Color color = fadePanel.color;
        float time = 0f;

        while (time < fadeSeconds)
        {
            time += Time.deltaTime;
            if (toDark)
            {
                color.a = Mathf.Lerp(0, 1, time / fadeSeconds);
            }
            else 
            {
                color.a = Mathf.Lerp(1, 0, time / fadeSeconds);
            }
            fadePanel.color = color;
            yield return null;
        }

        if (!toDark) fadePanel.GetComponent<Image>().enabled = false;
    }

    private void Awake()
    {
        StartCoroutine(Fade(false));
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
