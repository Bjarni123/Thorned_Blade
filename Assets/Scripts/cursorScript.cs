using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class cursorScript : MonoBehaviour
{
    private Vector3 cursorPosition;
    public Vector2 cursorOffset;

    [SerializeField] private Rigidbody2D playerTransform;
    [SerializeField] private Transform Cursor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        handleCursor();
    }

    private void handleCursor()
    {
        // cursor Position
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f;
        Vector2 cursorDir = ((cursorPosition - playerTransform.transform.position)).normalized;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, ((playerTransform.position + cursorDir) - cursorOffset), 50 * Time.deltaTime);
        Cursor.transform.position = smoothedPosition;

        // Cursor Rotation
        Vector3 lookAt = playerTransform.position;
        float AngleRad = Mathf.Atan2(lookAt.y - Cursor.position.y, lookAt.x - Cursor.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        Cursor.rotation = Quaternion.Euler(0f, 0f, AngleDeg + 90);
    }
}
