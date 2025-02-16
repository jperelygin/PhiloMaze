using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Camera cam;
    private float verticalRotation = 0f;

    [SerializeField] private Rigidbody rb;

    private AudioSource footStepsSource;
    private AudioSource mainThemeSource;
    [SerializeField] private float mainThemeDelaySeconds = 5.0f;

    private Animator animator;
    private bool isMapRaised = false;
    [SerializeField] private GameObject map;

    [SerializeField] private GameObject pauseMenu;
    private bool isGamePaused = false;

    private void PlayFootstepsIfMoving(float x, float z)
    {

        if (x != 0 | z != 0)
        {
            if (!footStepsSource.isPlaying)
            {
                footStepsSource.Play();
            }
        }
        else if (footStepsSource.isPlaying)
        {
            footStepsSource.Pause();
        }
    }

    private void PlayMainTheme()
    {
        mainThemeSource.Play();
    }

    private void MapControls()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!isMapRaised)
            {
                map.SetActive(true);
                isMapRaised = true;
            }
            else
            {
                map.SetActive(false);
                isMapRaised = false;
            }
        }
    }

    private void PauseControls()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isGamePaused = !isGamePaused;
            if (isGamePaused)
            {
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0f;
                gameController.StopTimer();
            }
            if (!isGamePaused)
            {
                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
                gameController.StartTimer();
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        pauseMenu = GameObject.Find("PauseMenu");

        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        // Sounds
        footStepsSource = GameObject.Find("Footsteps").GetComponent<AudioSource>();
        if (footStepsSource == null) Debug.LogError("Footsteps source is not found!");

        mainThemeSource = GameObject.Find("MainTheme").GetComponent<AudioSource>();
        if (mainThemeSource == null) Debug.LogError("Main theme music source is not found!");
        Invoke("PlayMainTheme", mainThemeDelaySeconds);

        // Animation
        animator = GetComponent<Animator>();
        //map = GameObject.Find("Map");
        map.SetActive(false);

        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMapRaised)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ);
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1);

            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
            
            // Walking sound
            PlayFootstepsIfMoving(moveX, moveZ);
        }

        if (!isGamePaused)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            cam.transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
        }

        // Map controls
        MapControls();

        // Pause menu
        PauseControls();
    }
}
