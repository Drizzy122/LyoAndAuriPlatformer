using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Platformer
{
    public class MenuManager : MonoBehaviour
    {
        public List<RectTransform> buttons; // Assign Button RectTransforms in the Inspector
    public float moveDuration = 0.5f;   // Duration for the button's movement
    public float buttonOffset = 100f;  // Vertical space between buttons
    private int activeIndex = 0;       // Keeps track of which button is currently active

    private List<Vector2> originalPositions; // Stores the original anchored positions of buttons

    public float activeAlpha = 1f;         // Fully visible for active buttons
    public float neighborAlpha = 0.7f;     // For buttons next to the active one
    public float hiddenAlpha = 0.2f;       // For buttons out of view

    void Start()
    {
        // Save the original positions of all UI buttons
        originalPositions = new List<Vector2>();
        foreach (var button in buttons)
        {
            originalPositions.Add(button.anchoredPosition);

            // Ensure each button has a Canvas Group
            if (button.GetComponent<CanvasGroup>() == null)
            {
                button.gameObject.AddComponent<CanvasGroup>();
            }
        }

        RefreshMenu(); // Set up the menu for the starting active button
    }

    void Update()
    {
        // Handle input for navigation
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CycleDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CycleUp();
        }
    }

    private void CycleDown()
    {
        // Move to the next index, looping around when necessary
        activeIndex = (activeIndex + 1) % buttons.Count;

        RefreshMenu(); // Refresh button positions and states
    }

    private void CycleUp()
    {
        // Move to the previous index, looping around when necessary
        activeIndex = (activeIndex - 1 + buttons.Count) % buttons.Count;

        RefreshMenu(); // Refresh button positions and states
    }

    private void RefreshMenu()
    {
        // Iterate through all buttons to adjust their positions and states
        for (int i = 0; i < buttons.Count; i++)
        {
            // Calculate offset based on the active index
            int offset = i - activeIndex;
            if (offset < -1) offset += buttons.Count; // Handle wrap-around down
            if (offset > 1) offset -= buttons.Count;  // Handle wrap-around up

            // Use the original position as a reference
            Vector2 targetPosition = originalPositions[i] + new Vector2(0, -buttonOffset * offset);

            // Animate the buttons to their correct positions
            buttons[i].DOAnchorPos(targetPosition, moveDuration);

            // Update the Canvas Group for active, neighboring, and hidden buttons
            CanvasGroup canvasGroup = buttons[i].GetComponent<CanvasGroup>();
            if (offset == 0) // Centered/Active button
            {
                // Highlight or "activate" this button
                canvasGroup.alpha = activeAlpha;     // Fully visible
                canvasGroup.interactable = true;    // Make it interactable
                canvasGroup.blocksRaycasts = true;  // Allow clicks and interaction
            }
            else if (Mathf.Abs(offset) == 1) // Buttons directly above and below (neighbors)
            {
                canvasGroup.alpha = neighborAlpha;  // Dim slightly, but visible
                canvasGroup.interactable = false;   // Not interactable
                canvasGroup.blocksRaycasts = false; // No clicks and interaction
            }
            else // Buttons farther away
            {
                canvasGroup.alpha = hiddenAlpha;    // Almost invisible
                canvasGroup.interactable = false;   // Not interactable
                canvasGroup.blocksRaycasts = false; // No clicks and interaction
            }
        }
    }

    }
}