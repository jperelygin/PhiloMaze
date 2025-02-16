using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{    
    [SerializeField] private Image keyImagePrefab;
    [SerializeField] private Sprite redKey;
    [SerializeField] private Sprite greenKey;
    [SerializeField] private Sprite blueKey;
    [SerializeField] private Sprite cyanKey;
    [SerializeField] private Sprite magentaKey;
    [SerializeField] private float spaceBetweenKeys = 100.0f;
    [SerializeField] private bool isOnScreenMapEnabled = false;

    [SerializeField] private GameObject player;

    Dictionary<int, Sprite> keyColors = new Dictionary<int, Sprite>();

    List<Image> images = new List<Image>(){ };

    Vector2 keyImageStartingPoint;

    private PhraseAppearance phraseAppearance;
    private GameObject upperHelpText;
    private GameObject lowerHelpText;

    private string UPPER_TEXT_RUS = "Собери все необходимые ключи и вернись к двери. ";
    private string LOWER_TEXT_RUS = "WASD для передвижения\r\nSPACE чтобы открыть карту";
    private string UPPER_TEXT_ENG = "Collect all needed keys and return to the door. ";
    private string LOWER_TEXT_ENG = "Use WASD to move\r\nPress SPACE to look at the map";

    public ScreenFade fade;

    public void OnKeyPicked(int color)
    {
        Vector2 point;
        if (images.Count == 0)
        {
            SetKeyImageStartingPoint();
            point = keyImageStartingPoint;
        }
        else 
        {
            point = images[^1].GetComponent<RectTransform>().anchoredPosition;
            point.x -= spaceBetweenKeys;
            
        }
        Image image = Instantiate(keyImagePrefab, GetComponent<Transform>());
        image.GetComponent<RectTransform>().anchoredPosition = point;
        
        Debug.Log($"image is on: {image.GetComponent<RectTransform>().anchoredPosition}");

        image.sprite = keyColors[color];
        images.Add(image);
    }

    public void SetLanguage(string language)
    {
        string upperText = "";
        string lowerText = "";
        if (language == "rus")
        {
            upperText = UPPER_TEXT_RUS;
            lowerText = LOWER_TEXT_RUS;
        }
        else 
        {
            if (language != "eng")
            {
                Debug.LogWarning($"There is no such language as: \"{language}\". Setting ENG by default.");
            }
            upperText = UPPER_TEXT_ENG;
            lowerText = LOWER_TEXT_ENG;
        }
        upperHelpText.GetComponent<TextMeshProUGUI>().text = upperText;
        lowerHelpText.GetComponent<TextMeshProUGUI>().text = lowerText;
        phraseAppearance.SetLanguage(language);
    }

    private void SetKeyImageStartingPoint()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log($"{rectTransform.rect}/{rectTransform.rect.width}|{rectTransform.rect.height}");
            Debug.Log($"{rectTransform.rect}/{rectTransform.rect.width - spaceBetweenKeys}|{rectTransform.rect.height - spaceBetweenKeys}");
            keyImageStartingPoint = new Vector2(rectTransform.rect.width / 2 - spaceBetweenKeys / 2, rectTransform.rect.height / 2 - spaceBetweenKeys / 2);
            Debug.Log($"{keyImageStartingPoint}");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //RED: 541065216, GREEN: -507510784, BLUE: 270532608, CYAN: -299892736, MAGENTA: 799014912

        keyColors.Add(541065216, redKey);
        keyColors.Add(-507510784, greenKey);
        keyColors.Add(270532608, blueKey);
        keyColors.Add(-299892736, cyanKey);
        keyColors.Add(799014912, magentaKey);

        if (isOnScreenMapEnabled)
        {
            GameObject.Find("MapImage").GetComponent<RawImage>().enabled = true;
            player.layer = 3;
        }

        phraseAppearance = GameObject.Find("PhyloPhrase").GetComponent<PhraseAppearance>();
        upperHelpText = GameObject.Find("UpperText");
        lowerHelpText = GameObject.Find("LowerText");
        //GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        //GetComponent<Canvas>().worldCamera = player.GetComponent<Transform>().Find("Camera").GetComponent<Camera>();

        fade = GetComponent<ScreenFade>();
        StartCoroutine(fade.Fade(false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
