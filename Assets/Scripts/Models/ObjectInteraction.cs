using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    private Vector2 lastTouchPosition;
    private float scaleSpeed = 0.01f;
    private float rotationSpeed = 0.2f;

    void Update()
    {
        // Rotate object based on one-finger drag
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World); // Horizontal swipe rotates on Y-axis
                transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World); // Vertical swipe rotates on X-axis
            }

            lastTouchPosition = touch.position;
        }

        // Scale object based on pinch gesture
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the previous positions of each touch
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Calculate the previous and current distance between the touches
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentTouchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in distances
            float deltaMagnitudeDiff = currentTouchDeltaMag - prevTouchDeltaMag;

            // Adjust the scale of the object based on the pinch gesture
            Vector3 newScale = transform.localScale + Vector3.one * deltaMagnitudeDiff * scaleSpeed;
            newScale = Vector3.Max(newScale, Vector3.one * 0.1f); // Minimum scale
            newScale = Vector3.Min(newScale, Vector3.one * 3f); // Maximum scale
            transform.localScale = newScale;
        }
    }
}
