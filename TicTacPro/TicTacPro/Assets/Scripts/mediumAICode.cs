using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class mediumAICode : MonoBehaviour
{
    public Button resetButton;
    public Button[] gridButtons;
    public LineRenderer lineRenderer;
    public TMP_Text resultText;

    private bool gameOver = false;
    private bool playerTurn = true;

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
        if (gameOver || !playerTurn) return;

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
        {
            buttonText.text = "X";
            buttonText.color = Color.red;
            playerTurn = false;

            if (CheckWinCondition("X"))
            {
                gameOver = true;
                resultText.text = "X Wins!";
                return;
            }

            StartCoroutine(AIMove());
        }
    }

    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1f);

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

        if (IsBoardFull())
        {
            gameOver = true;
            resultText.text = "It's a Tie!";
        }
        else
        {
            playerTurn = true;
        }
    }

    private int FindBestMove()
    {
        int bestMove = -1;
        float bestScore = float.NegativeInfinity;

        for (int i = 0; i < gridButtons.Length; i++)
        {
            TMP_Text buttonText = gridButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
            {
                buttonText.text = "O";
                float score = EvaluateMove();
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

    private float EvaluateMove()
    {
        float score = 0f;
        int[,] winPatterns = new int[,]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 },
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },
            { 0, 4, 8 }, { 2, 4, 6 }
        };

        foreach (int i in new int[] { 0, 1 })
        {
            string symbol = (i == 0) ? "X" : "O";
            float symbolScore = (symbol == "O") ? 1f : -1f;

            for (int j = 0; j < winPatterns.GetLength(0); j++)
            {
                int a = winPatterns[j, 0], b = winPatterns[j, 1], c = winPatterns[j, 2];

                string textA = gridButtons[a].GetComponentInChildren<TMP_Text>().text;
                string textB = gridButtons[b].GetComponentInChildren<TMP_Text>().text;
                string textC = gridButtons[c].GetComponentInChildren<TMP_Text>().text;

                int count = 0;
                if (textA == symbol) count++;
                if (textB == symbol) count++;
                if (textC == symbol) count++;

                if (count == 3) return symbolScore * 100f;
                if (count == 2) score += symbolScore * 10f;
                if (count == 1) score += symbolScore;
            }
        }

        return score;
    }

    private bool CheckWinCondition(string symbol)
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
                HighlightWinningLine(a, c);
                return true;
            }
        }
        return false;
    }

    private void HighlightWinningLine(int start, int end)
    {
        lineRenderer.enabled = true;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, gridButtons[start].transform.position);
        lineRenderer.SetPosition(1, gridButtons[end].transform.position);
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
        playerTurn = true;

        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "";
            }
        }

        lineRenderer.enabled = false;
        resultText.text = "";
    }
}
