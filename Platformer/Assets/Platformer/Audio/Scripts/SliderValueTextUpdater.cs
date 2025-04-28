using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Platformer
{
    public class SliderValueTextUpdater : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider slider; // Reference to the Slider
        [SerializeField] private TMP_Text valueText; // Reference to the TextMeshPro text to display the slider value

        private void Start()
        {
            // Initialize the text with the current slider value
            UpdateValueText(slider.value);

            // Add a listener to update the text whenever the slider value changes
            slider.onValueChanged.AddListener(UpdateValueText);
        }

        private void UpdateValueText(float value)
        {
            // Convert the slider value to an integer and update the text
            valueText.text = Mathf.RoundToInt(value).ToString();
        }

        private void OnDestroy()
        {
            // Remove the listener to prevent memory leaks
            slider.onValueChanged.RemoveListener(UpdateValueText);
        }

    }
}