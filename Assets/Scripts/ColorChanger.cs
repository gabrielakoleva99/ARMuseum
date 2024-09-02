using UnityEngine;
using PaintIn3D;
using CW.Common;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private CwPaintSphere paintSphere; // Reference to the paint brush

    // Method to change the brush color
    public void ChangePaintColor(Color newColor)
    {
        if (paintSphere != null)
        {
            paintSphere.Color = newColor;  // Set the new color
            Debug.Log($"Brush color changed to: {newColor}");
        }
    }

    // Example: Change color to red
    public void ChangeToRed()
    {
        ChangePaintColor(Color.red);
    }

    // Example: Change color to blue
    public void ChangeToBlue()
    {
        ChangePaintColor(Color.blue);
        Debug.Log($"Brush color changed to: {paintSphere.Color}");

    }

    // Example: Change color to a custom color
    public void ChangeToCustomColor()
    {
        ChangePaintColor(new Color(0.5f, 0.8f, 0.2f)); // Example custom color
    }
}
