using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This class controls the behavior of a side menu panel in a Unity application. The panel can be toggled open or closed,
/// and can be dragged horizontally within the screen bounds. The panel's position and visibility are managed based on user input.
/// </summary>
public class InfoPanelController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// The RectTransform of the side menu panel that will be controlled by this script.
    /// </summary>
    public RectTransform sideMenuRectTransform;

    /// <summary>
    /// The width of the screen, used to determine the boundaries for the panel's movement.
    /// </summary>
    private float screenWidth;

    /// <summary>
    /// The X position of the user's touch when the drag starts.
    /// </summary>
    private float startPositionX;

    /// <summary>
    /// The anchored X position of the panel at the start of the drag.
    /// </summary>
    private float startingAnchoredPositionX;

    /// <summary>
    /// Enum to determine which side of the screen the panel should appear on.
    /// </summary>
    public enum Side { left, right }

    /// <summary>
    /// The side of the screen where the panel is located (left or right).
    /// </summary>
    public Side side;

    /// <summary>
    /// Boolean to track whether the menu is currently open or closed.
    /// </summary>
    private bool isMenuOpen = false;

    /// <summary>
    /// Called before the first frame update. Initializes the screen width and sets the initial position of the panel
    /// off-screen based on the side it should appear on.
    /// </summary>
    void Start()
    {
        screenWidth = Screen.width;

        // Initially place the panel off-screen based on the specified side.
        if (side == Side.right)
            sideMenuRectTransform.anchoredPosition = new Vector2(screenWidth, sideMenuRectTransform.anchoredPosition.y); // Off-screen to the right
        else
            sideMenuRectTransform.anchoredPosition = new Vector2(-sideMenuRectTransform.rect.width, sideMenuRectTransform.anchoredPosition.y); // Off-screen to the left

        // Log the initial position
        Debug.Log($"Start: Initial panel position: {sideMenuRectTransform.anchoredPosition}");

        // Set the panel to inactive initially so it's not visible
        sideMenuRectTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles the dragging of the panel. The panel can only be dragged if it is open.
    /// </summary>
    /// <param name="eventData">Data related to the pointer event.</param>
    public void OnDrag(PointerEventData eventData)
    {
        if (isMenuOpen) // Only allow dragging if the menu is open
        {
            // Calculate and set the new position of the panel during the drag
            sideMenuRectTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(startingAnchoredPositionX - (startPositionX - eventData.position.x), GetMinPosition(), GetMaxPosition()),
                sideMenuRectTransform.anchoredPosition.y);

            // Log the position during dragging
            Debug.Log($"OnDrag: Panel position during drag: {sideMenuRectTransform.anchoredPosition}");
        }
    }

    /// <summary>
    /// Handles the pointer down event when the user starts touching the screen.
    /// Records the starting positions for the drag.
    /// </summary>
    /// <param name="eventData">Data related to the pointer event.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMenuOpen) // Only respond to touch if the menu is open
        {
            StopAllCoroutines();
            startPositionX = eventData.position.x;
            startingAnchoredPositionX = sideMenuRectTransform.anchoredPosition.x;

            // Log the position when dragging starts
            Debug.Log($"OnPointerDown: Starting drag. Panel position: {sideMenuRectTransform.anchoredPosition}");
        }
    }

    /// <summary>
    /// Handles the pointer up event when the user stops touching the screen.
    /// Determines whether the panel should remain open or close based on its position.
    /// </summary>
    /// <param name="eventData">Data related to the pointer event.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isMenuOpen) // Only respond to touch if the menu is open
        {
            // Determine whether to keep the panel open or close it
            bool shouldOpen = isAfterHalfPoint();
            StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, shouldOpen ? GetMinPosition() : GetMaxPosition()));

            // Log the decision on whether to open or close the panel
            Debug.Log($"OnPointerUp: Should open panel: {shouldOpen}. Panel position before slide: {sideMenuRectTransform.anchoredPosition}");

            isMenuOpen = shouldOpen; // Update the menu state based on drag position
        }
    }

    /// <summary>
    /// Determines whether the panel should remain open or close based on its position after dragging.
    /// </summary>
    /// <returns>True if the panel should remain open, false otherwise.</returns>
    private bool isAfterHalfPoint()
    {
        if (side == Side.right)
        {
            // If the panel's position is more than halfway off-screen to the right, it should close
            Debug.Log($"In isAfterHalf {sideMenuRectTransform.anchoredPosition.x < screenWidth - sideMenuRectTransform.rect.width}");

            return sideMenuRectTransform.anchoredPosition.x < screenWidth - sideMenuRectTransform.rect.width;
        }
        else
        {
            // If the panel's position is more than halfway off-screen to the left, it should close
            return sideMenuRectTransform.anchoredPosition.x > -sideMenuRectTransform.rect.width;
        }
    }

    /// <summary>
    /// Calculates the minimum position that the panel can move to, ensuring it remains on-screen.
    /// </summary>
    /// <returns>The minimum X position for the panel.</returns>
    private float GetMinPosition()
    {
        if (side == Side.right)
        {
            return (screenWidth - sideMenuRectTransform.rect.width) / 70; // Position to fully show the panel on the right
        }
        return 0; // Fully visible position for the left side
    }

    /// <summary>
    /// Calculates the maximum position that the panel can move to, ensuring it is fully off-screen.
    /// </summary>
    /// <returns>The maximum X position for the panel.</returns>
    private float GetMaxPosition()
    {
        if (side == Side.right)
        {
            Debug.Log($"MaxPosition is {screenWidth}");

            return screenWidth; // Fully off-screen to the right
        }
        return -sideMenuRectTransform.rect.width; // Fully off-screen to the left
    }

    /// <summary>
    /// Smoothly slides the panel to the target position over a specified duration.
    /// </summary>
    /// <param name="slideTime">The duration of the slide in seconds.</param>
    /// <param name="startingX">The starting X position of the slide.</param>
    /// <param name="targetX">The target X position of the slide.</param>
    /// <param name="onComplete">An optional callback to invoke after the slide is complete.</param>
    /// <returns>An IEnumerator to be used in a coroutine.</returns>
    private IEnumerator HandleMenuSlide(float slideTime, float startingX, float targetX, System.Action onComplete = null)
    {
        for (float i = 0; i <= slideTime; i += .025f)
        {
            sideMenuRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(startingX, targetX, i / slideTime), sideMenuRectTransform.anchoredPosition.y);
            yield return new WaitForSecondsRealtime(.025f);
        }

        // Ensure the panel is set to the target position at the end
        sideMenuRectTransform.anchoredPosition = new Vector2(targetX, sideMenuRectTransform.anchoredPosition.y);

        // Log the final position after the slide
        Debug.Log($"HandleMenuSlide: Final panel position: {sideMenuRectTransform.anchoredPosition}");

        // Invoke the onComplete callback if provided
        onComplete?.Invoke();
    }

    /// <summary>
    /// Toggles the visibility of the side menu. When the menu is opened, it is set to active and slides into view.
    /// When the menu is closed, it slides out of view and is then set to inactive.
    /// </summary>
    public void ToggleMenu()
    {
        // Activate the panel only when opening
        if (!isMenuOpen)
        {
            sideMenuRectTransform.gameObject.SetActive(true);
        }

        if (isMenuOpen)
        {
            StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, GetMaxPosition(), () =>
            {
                // Deactivate the panel when it's fully closed
                sideMenuRectTransform.gameObject.SetActive(false);

                // Log when the panel is closed and deactivated
                Debug.Log($"ToggleMenu: Panel closed and deactivated. Final position: {sideMenuRectTransform.anchoredPosition}");
            }));
        }
        else
        {
            StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, GetMinPosition(), () =>
            {
                // Log when the panel is fully opened
                Debug.Log($"ToggleMenu: Panel opened. Final position: {sideMenuRectTransform.anchoredPosition}");
            }));
        }
        isMenuOpen = !isMenuOpen;
        // Log the toggle state
        Debug.Log($"ToggleMenu: isMenuOpen is now: {isMenuOpen}");
    }
}
