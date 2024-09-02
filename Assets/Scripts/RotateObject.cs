using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed = 1.0f; // Speed of rotation

    private float previousRotationAngle;
    private float currentRotationAngle;

    void Update()
    {
        // Check if user is touching with 2 fingers and neither touch is over a UI element
        if (Input.touchCount == 2 && !IsPointerOverUIObject(Input.GetTouch(0)) && !IsPointerOverUIObject(Input.GetTouch(1)))
        {
            // Calculate the angle between the two touches
            currentRotationAngle = Mathf.Atan2(Input.GetTouch(0).position.y - Input.GetTouch(1).position.y,
                                               Input.GetTouch(0).position.x - Input.GetTouch(1).position.x) * Mathf.Rad2Deg;

            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
            {
                // Initialize the previous rotation angle when the touch begins
                previousRotationAngle = currentRotationAngle;
            }

            // Calculate the difference in angle between the current and previous frame
            float deltaRotation = currentRotationAngle - previousRotationAngle;

            // Rotate the object based on the delta rotation
            transform.Rotate(0, -deltaRotation * rotateSpeed, 0);

            // Update the previous rotation angle
            previousRotationAngle = currentRotationAngle;
        }
    }

    // Helper function to check if a touch is over a UI element
    bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
