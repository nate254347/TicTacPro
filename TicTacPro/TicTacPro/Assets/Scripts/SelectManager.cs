using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public Button xButton;
    public Button oButton;

    private Color originalXColor;
    private Color originalOColor;

    private enum Selection { None, X, O }
    private Selection currentSelection = Selection.None;

    void Start()
    {
        // Store the original colors of the buttons
        originalXColor = xButton.GetComponent<Image>().color;
        originalOColor = oButton.GetComponent<Image>().color;

        // Ensure buttons are properly initialized
        xButton.onClick.AddListener(() => ToggleSelection(Selection.X));
        oButton.onClick.AddListener(() => ToggleSelection(Selection.O));
    }

    private void ToggleSelection(Selection selection)
    {
        if (currentSelection == selection)
        {
            // If already selected, deselect both
            currentSelection = Selection.None;
        }
        else
        {
            // Switch to the selected one
            currentSelection = selection;
        }

        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        // Darken the selected button, reset the other
        xButton.GetComponent<Image>().color = (currentSelection == Selection.X) ? DarkenColor(originalXColor) : originalXColor;
        oButton.GetComponent<Image>().color = (currentSelection == Selection.O) ? DarkenColor(originalOColor) : originalOColor;
    }

    private Color DarkenColor(Color color)
    {
        return new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, color.a);
    }
}
