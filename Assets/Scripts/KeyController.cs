using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private GameController gameController;
    private AudioSource keyCollectedAS;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play sound
            keyCollectedAS.Play();

            int keyColor = GetComponent<Transform>().Find("MapSphere").GetComponent<Renderer>().material.color.GetHashCode();
            gameController.onKeyCollected(keyColor);

            //Destroy(gameObject);
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<Animator>().enabled = false;
            GetComponent<Transform>().Find("philomaze_key").GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Transform>().Find("Highlight").GetComponent<Light>().enabled = false;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        keyCollectedAS = GetComponent<Transform>().Find("KeyCollectedSound").GetComponent<AudioSource>();
        if (keyCollectedAS == null) Debug.LogError("No audio source for collecting key!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
