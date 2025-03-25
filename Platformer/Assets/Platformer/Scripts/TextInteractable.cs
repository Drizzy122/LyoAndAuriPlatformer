using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInteractable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text; // Assign a TextMeshProUGUI GameObject in the Inspector
  
    void Start()
    {
        if (text != null)
        {
            text.gameObject.SetActive(false); // Ensure the text starts as inactive
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && text != null)
        {
            text.gameObject.SetActive(true); // Enable the text when the player enters the trigger
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && text != null)
        {
            text.gameObject.SetActive(false); // Disable the text when the player exits the trigger
        }
    }
}