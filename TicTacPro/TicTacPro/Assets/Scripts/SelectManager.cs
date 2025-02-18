using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public Button xButton;
    public Button oButton;
    public Button resetButton;
    public Button[] gridButtons; // Array to store the 9 buttons
    public LineRenderer lineRenderer; // LineRenderer to draw the line
    public TextMeshProUGUI resultText; // Text to display the result (X wins, O wins, or Tie)
    public AudioSource audioSource;  // Reference to the AudioSource
    public AudioClip moveSound;      // Sound for placing "X" or "O"
    public AudioClip winSound;       // Sound for winning
    public AudioClip tieSound;       // Sound for a tie
    public AudioClip clickSound;     // Sound for clicking reset button

    private Color originalXColor;
    private Color originalOColor;

    private enum Selection { None, X, O }
    private Selection currentSelection = Selection.None;
    private bool gameWon = false;

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

        resultText.text = ""; // Initially, no result is displayed
    }

    private void ToggleSelection(Selection selection)
    {
        if (gameWon) return; // Prevent any action if the game is won

        // Toggle selection between X, O, or None
        if (currentSelection == selection)
        {
            currentSelection = Selection.None;
        }
        else
        {
            currentSelection = selection;
        }

        // Update button colors to reflect current selection
        UpdateButtonColors();
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void UpdateButtonColors()
    {
        // Darken the color of the selected button
        xButton.GetComponent<Image>().color = (currentSelection == Selection.X) ? DarkenColor(originalXColor) : originalXColor;
        oButton.GetComponent<Image>().color = (currentSelection == Selection.O) ? DarkenColor(originalOColor) : originalOColor;
    }

    private void ChangeGridButtonText(Button button)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (buttonText != null && string.IsNullOrEmpty(buttonText.text) && !gameWon) // Only set text if it's empty and the game isn't won
        {
            if (currentSelection == Selection.X)
            {
                buttonText.text = "X";
                PlaySound(moveSound);

                buttonText.color = Color.red; // Set X to red
            }
            else if (currentSelection == Selection.O)
            {
                buttonText.text = "O";
                PlaySound(moveSound);

                buttonText.color = Color.blue; // Set O to blue
            }

            // Check if there's a winner after each move
            CheckForWinner();
        }
    }

    private void CheckForWinner()
    {
        // Winning combinations (index positions of gridButtons)
        int[][] winningPatterns = new int[][]
        {
            new int[] { 0, 1, 2 }, // Row 1
            new int[] { 3, 4, 5 }, // Row 2
            new int[] { 6, 7, 8 }, // Row 3
            new int[] { 0, 3, 6 }, // Column 1
            new int[] { 1, 4, 7 }, // Column 2
            new int[] { 2, 5, 8 }, // Column 3
            new int[] { 0, 4, 8 }, // Diagonal 1
            new int[] { 2, 4, 6 }  // Diagonal 2
        };

        foreach (var pattern in winningPatterns)
        {
            TMP_Text text1 = gridButtons[pattern[0]].GetComponentInChildren<TMP_Text>();
            TMP_Text text2 = gridButtons[pattern[1]].GetComponentInChildren<TMP_Text>();
            TMP_Text text3 = gridButtons[pattern[2]].GetComponentInChildren<TMP_Text>();

            // Check if the three cells have the same text (either "X" or "O")
            if (text1.text == text2.text && text2.text == text3.text && !string.IsNullOrEmpty(text1.text))
            {
                // Winner found
                gameWon = true;
                PlaySound(winSound);

                // Draw the line over the winning pattern
                DrawWinningLine(pattern);
                DisableGridButtons(); // Disable buttons after win
                DisplayWinner(text1.text); // Display the winner
                return;
            }
        }

        // If there is no winner, check if it's a tie (all buttons filled)
        bool allFilled = true;
        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (string.IsNullOrEmpty(buttonText.text))
            {
                allFilled = false;
                break;
            }
        }

        if (allFilled)
        {
            gameWon = true;
            resultText.text = "It's a Tie!";
            PlaySound(tieSound);

        }
    }

    private void DisplayWinner(string winner)
    {
        if (winner == "X")
        {
            resultText.text = "X Wins!";
        }
        else if (winner == "O")
        {
            resultText.text = "O Wins!";
        }
    }

    private void DrawWinningLine(int[] winningPattern)
    {
        lineRenderer.enabled = true;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;

        // Set the line's width
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        Vector3 startPos = gridButtons[winningPattern[0]].transform.position;
        Vector3 endPos = gridButtons[winningPattern[2]].transform.position;

        Vector3 direction = (endPos - startPos).normalized;
        float extension = 0.5f;

        startPos -= direction * extension;
        endPos += direction * extension;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    private void DisableGridButtons()
    {
        foreach (Button button in gridButtons)
        {
            button.interactable = false; // Disable all grid buttons
        }
    }

    private void EnableGridButtons()
    {
        foreach (Button button in gridButtons)
        {
            button.interactable = true; // Re-enable grid buttons
        }
    }

    private void ResetGame()
    {
        currentSelection = Selection.None;
        gameWon = false;
        PlaySound(clickSound);

        resultText.text = ""; // Clear the result text

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

        // Hide the winning line
        lineRenderer.enabled = false;

        // Re-enable all grid buttons
        EnableGridButtons();
    }

    private Color DarkenColor(Color color)
    {
        return new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, color.a);
    }
}
