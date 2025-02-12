using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Light Source")]
    public Light directionalLight; // Assign your Directional Light here

    [Header("Cycle Settings")]
    public float dayLengthInMinutes = 1f; // Total duration of day-night cycle (in minutes)
    private float dayLengthInSeconds;     // Total duration of cycle in seconds
    private float rotationSpeed;         // Speed of rotation of the light
    
    [Header("Lighting Settings")]
    [Range(0f, 1f)]
    public float nightIntensity = 0.1f; // Intensity at night

    public Gradient lightColorGradient; // A gradient that changes the light color (for sunrise/sunset effects)

    private void Start()
    {
        if (directionalLight == null)
        {
            Debug.LogError("Please assign a directional light to the DayNightCycle script!");
            enabled = false;
            return;
        }

        // Calculate the total duration of a day in seconds
        dayLengthInSeconds = dayLengthInMinutes * 60f;

        // Calculate rotation speed (360 degrees in one day cycle)
        rotationSpeed = 360f / dayLengthInSeconds;
    }

    private void Update()
    {
        // Rotate the light to simulate the day-night cycle
        directionalLight.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        // Adjust the intensity based on the light's direction
        UpdateLightIntensity();

        // Update the color of the light
        UpdateLightColor();
    }

    private void UpdateLightIntensity()
    {
        // Calculate intensity based on dot product of light forward vector and up vector
        float dotProduct = Vector3.Dot(directionalLight.transform.forward, Vector3.down);
        float intensity = Mathf.Clamp01(dotProduct);

        // Apply intensity
        directionalLight.intensity = Mathf.Lerp(nightIntensity, 1f, intensity);
    }

    private void UpdateLightColor()
    {
        // Evaluate the light color from the gradient based on the time of day
        float dotProduct = Vector3.Dot(directionalLight.transform.forward, Vector3.down);
        float timeOfDayNormalized = Mathf.Clamp01(dotProduct);
        directionalLight.color = lightColorGradient.Evaluate(timeOfDayNormalized);
    }
}