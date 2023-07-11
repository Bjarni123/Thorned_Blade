using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class strawman_test : MonoBehaviour
{
    /* screen shake */
    public float shakeDuration = 0.5f; // Duration of the shake in seconds
    public float shakeIntensity = 0.1f; // Intensity of the shake

    private Vector3 initialPosition;
    private float currentShakeDuration;
    

    private void Start()
    {
        // Store the initial position of the object
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (currentShakeDuration > 0)
        {
            // Generate a random offset within the shake intensity
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;

            // Apply the offset to the object's position
            transform.position = initialPosition + randomOffset;

            // Reduce the shake duration
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            // Reset the object's position
            transform.position = initialPosition;
        }
    }

    public void ShakeObject()
    {
        // Start the shake with the specified duration
        currentShakeDuration = shakeDuration;
    }
}
