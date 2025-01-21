using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public Button xButton;
    public Button oButton;
    public Button resetButton;
    public Button[] gridButtons; // Array to store the 9 buttons

    private Color originalXColor;
    private Color originalOColor;

    private enum Selection { None, X, O }
    private Selection currentSelection = Selection.None;

    void Start()
    {
        originalXColor = xButton.GetComponent<Image>().color;
        originalOColor = oButton.GetComponent<Image>().color;

        // Initialize button listeners
        xButton.onClick.AddListener(() => ToggleSelection(Selection.X));
        oButton.onClick.AddListener(() => ToggleSelection(Selection.O));
        resetButton.onClick.AddListener(ResetGame);

        // Set listeners for all grid buttons
        foreach (Button button in gridButtons)
        {
            button.onClick.AddListener(() => ChangeGridButtonText(button));
        }
    }

    private void ToggleSelection(Selection selection)
    {
        if (currentSelection == selection)
        {
            currentSelection = Selection.None;
        }
        else
        {
            currentSelection = selection;
        }

        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        xButton.GetComponent<Image>().color = (currentSelection == Selection.X) ? DarkenColor(originalXColor) : originalXColor;
        oButton.GetComponent<Image>().color = (currentSelection == Selection.O) ? DarkenColor(originalOColor) : originalOColor;
    }

    private void ChangeGridButtonText(Button button)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (buttonText != null && string.IsNullOrEmpty(buttonText.text)) // Only set text if it's empty
        {
            if (currentSelection == Selection.X)
            {
                buttonText.text = "X";
            }
            else if (currentSelection == Selection.O)
            {
                buttonText.text = "O";
            }
        }
    }

    private void ResetGame()
    {
        currentSelection = Selection.None;

        // Reset all grid buttons to empty text
        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = ""; // Clear text
            }
        }

        // Reset button colors
        UpdateButtonColors();
    }

    private Color DarkenColor(Color color)
    {
        return new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, color.a);
    }
}
