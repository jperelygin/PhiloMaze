using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject upperText;
    [SerializeField] private GameObject lowerText;
    [SerializeField] private PhraseAppearance phraseAppearance;
    [SerializeField] private GameController gameController;

    private bool alreadyExited = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameController != null)
        {
            if (other.GetComponent<PlayerController>() != null && gameController.isAllKeysCollected())
            {
                gameController.StopTimer();
                Debug.Log(gameController.GetTimerValue());
            }
            if (other.GetComponent<PlayerController>())
            {
                Debug.Log("Player entered the area");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited trigger!");
        if (other.GetComponent<PlayerController>() != null && !alreadyExited)
        {
            upperText.SetActive(false);
            lowerText.SetActive(false);
            phraseAppearance.SetIsTextAppearing(true);
            gameController.StartTimer();
            alreadyExited = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        upperText = GameObject.Find("UpperText");
        lowerText = GameObject.Find("LowerText");
        phraseAppearance = GameObject.Find("PhyloPhrase").GetComponent<PhraseAppearance>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
