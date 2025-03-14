using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // Choose the axis of rotation in the Inspector.
    public float rotationSpeed = 10f;        // Adjust this value to control the speed of rotation.

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

}
