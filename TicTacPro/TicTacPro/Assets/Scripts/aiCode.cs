using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class aiCode : MonoBehaviour
{
    public Button resetButton;
    public Button[] gridButtons;
    public LineRenderer lineRenderer;
    public TMP_Text resultText;  // Reference to the result text

    private bool gameOver = false;
    private bool playerTurn = true;  // Player starts first

    void Start()
    {
        resetButton.onClick.AddListener(ResetGame);

        foreach (Button button in gridButtons)
        {
            button.onClick.AddListener(() => PlayerMove(button));
        }
    }

    private void PlayerMove(Button button)
    {
        if (gameOver || !playerTurn) return; // Prevent move if game is over or AI's turn

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
        {
            buttonText.text = "X";
            buttonText.color = Color.red;
            playerTurn = false; // Disable further moves

            if (CheckWinCondition("X"))
            {
                gameOver = true;
                resultText.text = "X Wins!";
                return;
            }

            StartCoroutine(AIMove()); // AI makes a move
        }
    }

    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1f); // Simulate AI thinking time

        if (gameOver) yield break;

        int bestMove = FindBestMove();
        if (bestMove != -1)
        {
            TMP_Text buttonText = gridButtons[bestMove].GetComponentInChildren<TMP_Text>();
            buttonText.text = "O";
            buttonText.color = Color.blue;

            if (CheckWinCondition("O"))
            {
                gameOver = true;
                resultText.text = "O Wins!";
                yield break;
            }
        }

        if (IsBoardFull())  // If the board is full and no one has won, it's a tie
        {
            gameOver = true;
            resultText.text = "It's a Tie!";
        }
        else
        {
            playerTurn = true; // Allow player to move again
        }
    }

    private int FindBestMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < gridButtons.Length; i++)
        {
            TMP_Text buttonText = gridButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
            {
                buttonText.text = "O";
                int score = Minimax(false);
                buttonText.text = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        return bestMove;
    }

    private int Minimax(bool isMaximizing)
    {
        string winner = GetWinner();
        if (winner == "X") return -1;
        if (winner == "O") return 1;
        if (IsBoardFull()) return 0;

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        for (int i = 0; i < gridButtons.Length; i++)
        {
            TMP_Text buttonText = gridButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
            {
                // Make the move
                buttonText.text = isMaximizing ? "O" : "X";  // AI or Player move
                int score = Minimax(!isMaximizing);  // Recurse
                buttonText.text = "";  // Undo the move

                // Update the best score
                bestScore = isMaximizing ? Mathf.Max(score, bestScore) : Mathf.Min(score, bestScore);
            }
        }
        return bestScore;
    }

    private string GetWinner()
    {
        int[,] winPatterns = new int[,]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 },
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },
            { 0, 4, 8 }, { 2, 4, 6 }
        };

        foreach (int i in new int[] { 0, 1 })
        {
            string symbol = (i == 0) ? "X" : "O";
            for (int j = 0; j < winPatterns.GetLength(0); j++)
            {
                int a = winPatterns[j, 0], b = winPatterns[j, 1], c = winPatterns[j, 2];
                if (gridButtons[a].GetComponentInChildren<TMP_Text>().text == symbol &&
                    gridButtons[b].GetComponentInChildren<TMP_Text>().text == symbol &&
                    gridButtons[c].GetComponentInChildren<TMP_Text>().text == symbol)
                {
                    return symbol;
                }
            }
        }

        return null;
    }

    private bool CheckWinCondition(string symbol)
    {
        if (GetWinner() == symbol)
        {
            HighlightWinningLine(symbol);
            return true;
        }
        return false;
    }

    private void HighlightWinningLine(string symbol)
    {
        int[,] winPatterns = new int[,]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 },
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },
            { 0, 4, 8 }, { 2, 4, 6 }
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            int a = winPatterns[i, 0], b = winPatterns[i, 1], c = winPatterns[i, 2];

            if (gridButtons[a].GetComponentInChildren<TMP_Text>().text == symbol &&
                gridButtons[b].GetComponentInChildren<TMP_Text>().text == symbol &&
                gridButtons[c].GetComponentInChildren<TMP_Text>().text == symbol)
            {
                lineRenderer.enabled = true;
                lineRenderer.startColor = Color.black;
                lineRenderer.endColor = Color.black;
                lineRenderer.startWidth = 0.15f;
                lineRenderer.endWidth = 0.15f;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, gridButtons[a].transform.position);
                lineRenderer.SetPosition(1, gridButtons[c].transform.position);
                return;
            }
        }
    }

    private bool IsBoardFull()
    {
        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
            {
                return false;
            }
        }
        return true;
    }

    private void ResetGame()
    {
        gameOver = false;
        playerTurn = true;  // Ensure player starts first

        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "";
            }
        }

        lineRenderer.enabled = false;
        resultText.text = "";  // Clear result text
    }
}
