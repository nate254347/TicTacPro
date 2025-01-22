using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public Button xButton;
    public Button oButton;
    public Button resetButton;
    public Button[] gridButtons; // Array for the 9 grid buttons

    private Color originalXColor;
    private Color originalOColor;
    private Color originalGridColor;
    private bool gameWon = false; // Prevent further inputs after a win

    private enum Selection { None, X, O }
    private Selection currentSelection = Selection.None;

    void Start()
    {
        originalXColor = xButton.GetComponent<Image>().color;
        originalOColor = oButton.GetComponent<Image>().color;
        originalGridColor = gridButtons[0].GetComponent<Image>().color; // Store default grid button color

        xButton.onClick.AddListener(() => ToggleSelection(Selection.X));
        oButton.onClick.AddListener(() => ToggleSelection(Selection.O));
        resetButton.onClick.AddListener(ResetGame);

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
        if (gameWon) return; // Prevent changing text after a win

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
        {
            if (currentSelection == Selection.X)
            {
                buttonText.text = "X";
            }
            else if (currentSelection == Selection.O)
            {
                buttonText.text = "O";
            }

            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        int[,] winPatterns = new int[,]
        {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Rows
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columns
            {0, 4, 8}, {2, 4, 6}             // Diagonals
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            int a = winPatterns[i, 0];
            int b = winPatterns[i, 1];
            int c = winPatterns[i, 2];

            TMP_Text textA = gridButtons[a].GetComponentInChildren<TMP_Text>();
            TMP_Text textB = gridButtons[b].GetComponentInChildren<TMP_Text>();
            TMP_Text textC = gridButtons[c].GetComponentInChildren<TMP_Text>();

            if (textA.text != "" && textA.text == textB.text && textA.text == textC.text)
            {
                gameWon = true;
                HighlightWinningButtons(a, b, c);
                return;
            }
        }
    }

    private void HighlightWinningButtons(int a, int b, int c)
    {
        gridButtons[a].GetComponent<Image>().color = Color.blue;
        gridButtons[b].GetComponent<Image>().color = Color.blue;
        gridButtons[c].GetComponent<Image>().color = Color.blue;
    }

    private void ResetGame()
    {
        currentSelection = Selection.None;
        gameWon = false;

        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "";
            }
            button.GetComponent<Image>().color = originalGridColor; // Reset button color
        }

        UpdateButtonColors();
    }

    private Color DarkenColor(Color color)
    {
        return new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, color.a);
    }
}
