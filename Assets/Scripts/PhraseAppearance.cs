using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhraseAppearance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float scaleFactor = 2f;
    [SerializeField] private float textAppearanceTimer = 5f;
    [SerializeField] private TextAsset phrasesJsonFile;
    
    private Language language = Language.Rus;
    private bool isTextAppearing = false;
    private List<string> phrases = new List<string>();


    public enum Language
    {
        Rus,
        Eng
    }

    [System.Serializable]
    public class LanguagePhrases
    {
        public List<string> rus;
        public List<string> eng;
    }

    public void SetLanguage(string lang)
    {
        if (lang == "rus")
        {
            language = Language.Rus;
        }
        else
        {
            if (lang != "eng")
            {
                Debug.LogWarning($"There is no language: \"{language}\". Setting ENG by default");
            }
            language = Language.Eng;
        }
    }

    private IEnumerator PlayText()
    {
        textMesh.transform.localScale = Vector3.one * 0.8f;

        yield return StartCoroutine(FadeAndScale(0f, 1f, 0.8f, scaleFactor, fadeInDuration));

        yield return new WaitForSeconds(holdDuration);

        yield return StartCoroutine(FadeAndScale(1f, 0f, scaleFactor, 0.8f, fadeOutDuration));
    }

    private IEnumerator FadeAndScale(float startAlpha, float endAlpha, float startScale, float endScale, float duration)
    {
        float elapsedTime = 0f;

        Color color = textMesh.color;
        Vector3 initialScale = Vector3.one * startScale;
        Vector3 targetScale = Vector3.one * endScale;

        textMesh.transform.localScale = initialScale;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            color.a = alpha;
            textMesh.color = color;

            textMesh.transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        textMesh.color = color;
        textMesh.transform.localScale = targetScale;
    
    }

    public void SetIsTextAppearing(bool isTextAppearing)
    {
        this.isTextAppearing = isTextAppearing;
    }

    private void ParseTextFile() 
    {
        if (phrasesJsonFile != null)
        {
            LanguagePhrases data = JsonUtility.FromJson<LanguagePhrases>(phrasesJsonFile.text);
            if (data != null && data.rus != null && data.eng != null)
            {
                foreach (var item in data.rus)
                {
                    Debug.Log(item);
                }
            }
            else
            {
                Debug.LogError("No data in json!");
            }

            // With adding new languages dont forget the Serialized class.
            switch (language)
            {
                case Language.Rus:
                    phrases = data.rus;
                    break;
                case Language.Eng:
                    phrases = data.eng;
                    break;
            }
        }
        else
        {
            Debug.LogError("JSON FILE IS NULL!");
        }
    }

    private string GetPhrase()
    {
        string phrase;
        if (phrases.Count == 0)
        {
            ParseTextFile();
        }
        phrase = phrases[Random.Range(0, phrases.Count)];
        phrases.Remove(phrase);
        return phrase;
    }

    private IEnumerator TextAppearanceTimer()
    {
        while (true)
        {
            if (isTextAppearing)
            {
                textMesh.text = GetPhrase();
                yield return new WaitForSeconds(textAppearanceTimer);
                StartCoroutine(PlayText());
                yield return new WaitForSeconds(fadeInDuration + fadeOutDuration + holdDuration);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    
    }

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log($"Size of phrasees: {phrases.Count}");
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }
        textMesh.alpha = 0;
        ParseTextFile();
        StartCoroutine(TextAppearanceTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
