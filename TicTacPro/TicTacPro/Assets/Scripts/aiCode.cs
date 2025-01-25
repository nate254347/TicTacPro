using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class aiCode : MonoBehaviour
{
    public Button xButton;
    public Button oButton;
    public Button resetButton;
    public Button[] gridButtons;
    public LineRenderer lineRenderer;

    private Color originalXColor;
    private Color originalOColor;
    private bool gameOver = false;

    private enum Selection { None, X, O }
    private Selection currentSelection = Selection.None;

    void Start()
    {
        originalXColor = xButton.GetComponent<Image>().color;
        originalOColor = oButton.GetComponent<Image>().color;

        xButton.onClick.AddListener(() => ToggleSelection(Selection.X));
        oButton.onClick.AddListener(() => ToggleSelection(Selection.O));
        resetButton.onClick.AddListener(ResetGame);

        foreach (Button button in gridButtons)
        {
            button.onClick.AddListener(() => PlayerMove(button));
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

    private void PlayerMove(Button button)
    {
        if (gameOver) return; // Stop if game is over

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
        {
            buttonText.text = "X";
            buttonText.color = Color.red;

            if (CheckWinCondition("X"))
            {
                gameOver = true;
                return;
            }

            StartCoroutine(AIMove()); // AI waits before making a move
        }
    }

    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1f); // Simulate AI thinking time

        if (gameOver) yield break; // Stop if game is already won

        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
            {
                buttonText.text = "O";
                buttonText.color = Color.blue;

                if (CheckWinCondition("O"))
                {
                    gameOver = true;
                }

                break; // AI moves only once
            }
        }
    }

    private bool CheckWinCondition(string symbol)
    {
        int[,] winPatterns = new int[,]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Rows
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Columns
            { 0, 4, 8 }, { 2, 4, 6 }              // Diagonals
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            int a = winPatterns[i, 0], b = winPatterns[i, 1], c = winPatterns[i, 2];

            if (gridButtons[a].GetComponentInChildren<TMP_Text>().text == symbol &&
                gridButtons[b].GetComponentInChildren<TMP_Text>().text == symbol &&
                gridButtons[c].GetComponentInChildren<TMP_Text>().text == symbol)
            {
                HighlightWinningLine(a, b, c);
                return true;
            }
        }

        return false;
    }

    private void HighlightWinningLine(int a, int b, int c)
    {
        lineRenderer.enabled = true;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, gridButtons[a].transform.position);
        lineRenderer.SetPosition(1, gridButtons[c].transform.position);
    }

    private void ResetGame()
    {
        gameOver = false;
        currentSelection = Selection.None;

        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "";
            }
        }

        UpdateButtonColors();
        lineRenderer.enabled = false;
    }

    private Color DarkenColor(Color color)
    {
        return new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, color.a);
    }
}
