using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject keyOnScene;
    [SerializeField]
    private float cameraSpeed = 20f;
    [SerializeField]
    private float distance = 50f;

    private float currentAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentAngle += cameraSpeed * Time.deltaTime;

        float x = keyOnScene.transform.position.x + distance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float z = keyOnScene.transform.position.z + distance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        Vector3 newPosition = new Vector3(x, transform.position.y, z);

        transform.position = newPosition;
        transform.LookAt(keyOnScene.transform);
    }
}
