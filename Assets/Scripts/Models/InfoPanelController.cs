using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//handles the InformationPanel's behaviour
public class InfoPanelController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    // controlls panel position
    public RectTransform sideMenuRectTransform;

    // width of the screen
    private float screenWidth;

    // position before dragging
    private float startPositionX;

    // anchored X position of the panel at the start of the drag.
    private float startingAnchoredPositionX;

    /// Enum to determine which side of the screen the panel should appear on.
    public enum Side { left, right }

    /// <summary>
    /// The side of the screen where the panel is located (left or right).
    /// </summary>
    public Side side;

   
    // checks if menu is currently open or closed.
    private bool isMenuOpen = false;

    // initializes the screen width and sets the initial position of the panel
    void Start()
    {
        screenWidth = Screen.width;

        // place the panel off-screen on the right side.
            sideMenuRectTransform.anchoredPosition = new Vector2(screenWidth, sideMenuRectTransform.anchoredPosition.y);

        Debug.Log($"Start: Initial panel position: {sideMenuRectTransform.anchoredPosition}");

        // sets the panel to inactive, so it's not visible
        sideMenuRectTransform.gameObject.SetActive(false);
    }

    // handles the dragging of the panel
   // <param name="eventData">data related to the pointer event.</param>
    public void OnDrag(PointerEventData eventData)
    {
        if (isMenuOpen) // Only allow dragging if the menu is open
        {
            // calculates and sets the new position of the panel during the drag
            sideMenuRectTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(startingAnchoredPositionX - (startPositionX - eventData.position.x), GetMinPosition(), GetMaxPosition()),
                sideMenuRectTransform.anchoredPosition.y);

            Debug.Log($"OnDrag: Panel position during drag: {sideMenuRectTransform.anchoredPosition}");
        }
    }

    // Handles the pointer down event when the user touches the screen
    // <param name="eventData">data related to the pointer event.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMenuOpen) // Only respond to touch if the menu is open
        {
            StopAllCoroutines();
            startPositionX = eventData.position.x;
            startingAnchoredPositionX = sideMenuRectTransform.anchoredPosition.x;

            Debug.Log($"OnPointerDown: Starting drag. Panel position: {sideMenuRectTransform.anchoredPosition}");
        }
    }

    // determines if the panel should open or close
    // <param name="eventData">data related to the pointer event.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isMenuOpen) // Only respond to touch if the menu is open
        {
            // check if panel should be opened or closed
            bool shouldOpen = isAfterHalfPoint();
            StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, shouldOpen ? GetMinPosition() : GetMaxPosition()));

            Debug.Log($"OnPointerUp: Should open panel: {shouldOpen}. Panel position before slide: {sideMenuRectTransform.anchoredPosition}");

            isMenuOpen = shouldOpen;
        }
    }

    //determines if the panel's position is beyond its half to know whether to open or close it
    /// <returns>true if the panel should remain open, false otherwise.</returns>
    private bool isAfterHalfPoint()
    {
       
            Debug.Log($"In isAfterHalf {sideMenuRectTransform.anchoredPosition.x < screenWidth - sideMenuRectTransform.rect.width}");
            // if panel's position is more than half off-screen it should close
            return sideMenuRectTransform.anchoredPosition.x < screenWidth - sideMenuRectTransform.rect.width;
        
       
    }

    // calculates the minimum position that the panel can move to, ensuring it remains on-screen.
    // <returns>the minimum X position for the panel.</returns>
    private float GetMinPosition()
    {
        
            return (screenWidth - sideMenuRectTransform.rect.width) / 70; // Position to fully show the panel on the right
       
    }

    // calculates the maximum position that the panel can move to, ensuring it is fully off-screen.
    // <returns>the maximum X position for the panel.</returns>
    private float GetMaxPosition()
    {
        
            Debug.Log($"MaxPosition is {screenWidth}");
            return screenWidth; // Fully off-screen to the right
        
    }

    /// handles the sliding of the panel to the target position over a specified duration.
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

        // ensures the panel is set to the target position at the end
        sideMenuRectTransform.anchoredPosition = new Vector2(targetX, sideMenuRectTransform.anchoredPosition.y);

        Debug.Log($"HandleMenuSlide: Final panel position: {sideMenuRectTransform.anchoredPosition}");

        onComplete?.Invoke();
    }

    // Toggles the visibility of the side menu. When the menu is opened, it is set to active and slides into view.
    // When the menu is closed, it slides out of view and is then set to inactive.
    public void ToggleMenu()
    {
        // activates the panel only when opening
        if (!isMenuOpen)
        {
            sideMenuRectTransform.gameObject.SetActive(true);
        }

        if (isMenuOpen)
        {
            StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, GetMaxPosition(), () =>
            {
                // deactivate the panel when it's fully closed
                sideMenuRectTransform.gameObject.SetActive(false);
                Debug.Log($"ToggleMenu: Panel closed and deactivated. Final position: {sideMenuRectTransform.anchoredPosition}");
            }));
        }
        else
        {
            StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, GetMinPosition(), () =>
            {
                Debug.Log($"ToggleMenu: Panel opened. Final position: {sideMenuRectTransform.anchoredPosition}");
            }));
        }
        isMenuOpen = !isMenuOpen;
        Debug.Log($"ToggleMenu: isMenuOpen is now: {isMenuOpen}");
    }
}
