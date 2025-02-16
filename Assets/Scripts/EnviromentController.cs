using UnityEngine;

public class EnviromentController : MonoBehaviour
{
    [SerializeField] private bool isDirectionalLightOn = true;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private bool isFogOn = true;
    [SerializeField] private Color fogColor;
    [SerializeField] private float fogDensity;
    
    [SerializeField] private bool isSkyboxOn = true;


    private void TurnOffDirectionalLight()
    {
        if (directionalLight != null && !isDirectionalLightOn)
        {
            directionalLight.SetActive(false);
        }
    }

    private void SetUpFog()
    {
        RenderSettings.fog = isFogOn;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    private void TurnOffSkybox()
    {
        if (!isSkyboxOn)
        {
            RenderSettings.skybox = null;
            RenderSettings.ambientSkyColor = Color.black;
            RenderSettings.ambientLight = Color.black;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TurnOffDirectionalLight();
        SetUpFog();
        TurnOffSkybox();
        // at the end
        DynamicGI.UpdateEnvironment();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
