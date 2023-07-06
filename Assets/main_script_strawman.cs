using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main_script_strawman : MonoBehaviour
{
    //health
    public float health = 3;


    /* screen shake */
    public float shakeDuration = 0.5f; // Duration of the shake in seconds
    public float shakeIntensity = 0.1f; // Intensity of the shake

    private Vector3 initialPosition;
    private float currentShakeDuration;


    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position of the object
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
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


    /* public void ShakeObject()
    {
        // Start the shake with the specified duration
        currentShakeDuration = shakeDuration;
    } */


    public void i_have_been_hit()
    {
        // Start the shake with the specified duration
        currentShakeDuration = shakeDuration;

        health -= 1;
        shakeIntensity += 0.03f;

        if(health < 1)
        {
            Destroy(gameObject);
        }
    }
}
