using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Transform objectToFollow; // Reference to the object that the health bar should follow
    public float yOffset = 1.0f; // Offset in the y-axis to position the health bar above the object
    public float healthBarWidth = 50f; // Width of the health bar
    public float healthBarHeight = 5f; // Height of the health bar
    public Color healthBarBackgroundColor = Color.red; // Background color of the health bar
    public Color healthBarFillColor = Color.green; // Fill color of the health bar

    private RectTransform healthBarBackground;
    private RectTransform healthBarFill;
    private Canvas canvas;

    private void Start()
    {
        // Create a new GameObject for the health bar background
        GameObject backgroundObj = new GameObject("HealthBarBackground");
        backgroundObj.transform.SetParent(canvas.transform);

        // Add an Image component to the background object
        Image backgroundImg = backgroundObj.AddComponent<Image>();
        backgroundImg.color = healthBarBackgroundColor;

        // Set the size of the background object
        healthBarBackground = backgroundObj.GetComponent<RectTransform>();
        healthBarBackground.sizeDelta = new Vector2(healthBarWidth, healthBarHeight);

        // Create a new GameObject for the health bar fill
        GameObject fillObj = new GameObject("HealthBarFill");
        fillObj.transform.SetParent(backgroundObj.transform);

        // Add an Image component to the fill object
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = healthBarFillColor;

        // Set the size and anchor position of the fill object
        healthBarFill = fillObj.GetComponent<RectTransform>();
        healthBarFill.anchorMin = new Vector2(0f, 0f);
        healthBarFill.anchorMax = new Vector2(1f, 0f);
        healthBarFill.sizeDelta = new Vector2(healthBarWidth, healthBarHeight);
    }

    private void LateUpdate()
    {
        // Position the health bar above the object's head in world space
        Vector3 objectPosition = objectToFollow.position;
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(objectPosition);
        Vector2 screenPosition = new Vector2(
            (viewportPosition.x * canvas.pixelRect.width) - (canvas.pixelRect.width * 0.5f),
            (viewportPosition.y * canvas.pixelRect.height) - (canvas.pixelRect.height * 0.5f)
        );
        Vector2 anchoredPosition = screenPosition + new Vector2(0f, yOffset * canvas.scaleFactor);

        // Set the position of the health bar in screen space
        healthBarBackground.anchoredPosition = anchoredPosition;
    }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }
}
