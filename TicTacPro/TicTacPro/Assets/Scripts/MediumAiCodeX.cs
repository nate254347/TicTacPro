using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MediumAICodeX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip moveSound, winSound, tieSound, clickSound;

    public Button resetButton;
    public Button[] gridButtons;
    public LineRenderer lineRenderer;
    public TMP_Text resultText;

    private bool gameOver = false;
    private bool playerTurn = false; // AI goes first

    void Start()
    {
        resetButton.onClick.AddListener(ResetGame);

        foreach (Button button in gridButtons)
        {
            button.onClick.AddListener(() => PlayerMove(button));
        }

        StartCoroutine(AIMove()); // AI makes the first move
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayerMove(Button button)
    {
        if (gameOver || playerTurn) return; // Player moves second

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
        {
            buttonText.text = "O"; // Player is O
            buttonText.color = Color.blue;
            PlaySound(moveSound);
            playerTurn = true;

            if (CheckWinCondition("O"))
            {
                gameOver = true;
                resultText.text = "O Wins!";
                PlaySound(winSound);
                return;
            }

            StartCoroutine(AIMove());
        }
    }

    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1f);

        if (gameOver) yield break;

        int bestMove = -1;

        // Check for winning move first
        bestMove = FindWinningMove("X");
        if (bestMove != -1)
        {
            MakeMove(bestMove, "X");
            yield break;
        }

        // Check for blocking move
        bestMove = FindWinningMove("O");
        if (bestMove != -1)
        {
            MakeMove(bestMove, "X");
            yield break;
        }

        // If no winning or blocking move, make a random move excluding the center on the first turn
        bestMove = FindRandomMove();
        MakeMove(bestMove, "X");
    }

    private int FindWinningMove(string symbol)
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

            string textA = gridButtons[a].GetComponentInChildren<TMP_Text>().text;
            string textB = gridButtons[b].GetComponentInChildren<TMP_Text>().text;
            string textC = gridButtons[c].GetComponentInChildren<TMP_Text>().text;

            if (textA == symbol && textB == symbol && string.IsNullOrEmpty(textC)) return c;
            if (textA == symbol && string.IsNullOrEmpty(textB) && textC == symbol) return b;
            if (string.IsNullOrEmpty(textA) && textB == symbol && textC == symbol) return a;
        }

        return -1;
    }

    private int FindRandomMove()
    {
        // Make sure to avoid the center spot on the first move
        List<int> availableMoves = new List<int>();

        for (int i = 0; i < gridButtons.Length; i++)
        {
            TMP_Text buttonText = gridButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
            {
                // If the first move, avoid the middle
                if (i != 4)
                {
                    availableMoves.Add(i);
                }
            }
        }

        if (availableMoves.Count > 0)
        {
            return availableMoves[Random.Range(0, availableMoves.Count)];
        }

        return -1; // No valid move found
    }

    private void MakeMove(int index, string symbol)
    {
        TMP_Text buttonText = gridButtons[index].GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = symbol; // AI is X
            buttonText.color = Color.red;
            PlaySound(moveSound);

            if (CheckWinCondition(symbol))
            {
                gameOver = true;
                resultText.text = $"{symbol} Wins!";
                PlaySound(winSound);
            }
            else if (IsBoardFull())
            {
                gameOver = true;
                resultText.text = "It's a Tie!";
                PlaySound(tieSound);
            }
            else
            {
                playerTurn = false;
            }
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
                buttonText.text = "X";
                float score = EvaluateMove();
                buttonText.text = "";

                // Limit the AI's move choices
                if (score > bestScore && Random.value < 0.8f) // Introduce a randomness factor
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
            float symbolScore = (symbol == "X") ? 1f : -1f;

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
        playerTurn = false; // AI still goes first
        PlaySound(clickSound);

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

        StartCoroutine(AIMove()); // AI moves first on reset
    }
}
