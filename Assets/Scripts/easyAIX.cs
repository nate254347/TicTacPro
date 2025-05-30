using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class easyAIX : MonoBehaviour
{
    public AudioSource audioSource;  // Reference to the AudioSource
    public AudioClip moveSound;      // Sound for placing "X" or "O"
    public AudioClip winSound;       // Sound for winning
    public AudioClip tieSound;       // Sound for a tie
    public AudioClip clickSound;     // Sound for clicking reset button

    public Button resetButton;
    public Button[] gridButtons;
    public LineRenderer lineRenderer;
    public TMP_Text resultText;  // Reference to the result text

    private bool gameOver = false;
    private bool playerTurn = false;  // Player starts first

    void Start()
    {
        resetButton.onClick.AddListener(ResetGame);
        StartCoroutine(AIMove());
        foreach (Button button in gridButtons)
        {
            button.onClick.AddListener(() => PlayerMove(button));
        }
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
        if (gameOver || !playerTurn) return; // Prevent move if game is over or AI's turn

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
        {
            buttonText.text = "O";
            PlaySound(moveSound);

            buttonText.color = Color.blue;
            playerTurn = false; // Disable further moves

            if (CheckWinCondition("O"))
            {
                gameOver = true;
                resultText.text = "O Wins!";
                PlaySound(winSound);

                return;
            }

            StartCoroutine(AIMove()); // AI makes a move
        }
    }

    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1f); // Simulate AI thinking time

        if (gameOver) yield break;

        int randomMove = GetRandomMove();  // Get a random available move
        if (randomMove != -1)
        {
            TMP_Text buttonText = gridButtons[randomMove].GetComponentInChildren<TMP_Text>();
            buttonText.text = "X";
            PlaySound(moveSound);

            buttonText.color = Color.red;

            if (CheckWinCondition("X"))
            {
                gameOver = true;
                resultText.text = "X Wins!";
                PlaySound(winSound);

                yield break;
            }
        }

        if (IsBoardFull())  // If the board is full and no one has won, it's a tie
        {
            gameOver = true;
            resultText.text = "It's a Tie!";
            PlaySound(tieSound);

        }
        else
        {
            playerTurn = true; // Allow player to move again
        }
    }

    // Randomly select a valid move for the AI
    private int GetRandomMove()
    {
        // Get all the available empty spots
        System.Collections.Generic.List<int> availableMoves = new System.Collections.Generic.List<int>();

        for (int i = 0; i < gridButtons.Length; i++)
        {
            TMP_Text buttonText = gridButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null && string.IsNullOrEmpty(buttonText.text))
            {
                availableMoves.Add(i);
            }
        }

        // If there are available moves, select one randomly
        if (availableMoves.Count > 0)
        {
            int randomIndex = Random.Range(0, availableMoves.Count);
            return availableMoves[randomIndex];
        }

        return -1; // No moves available
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
        PlaySound(clickSound);

        playerTurn = false;  // Ensure player starts first

        foreach (Button button in gridButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "";
            }
        }

        lineRenderer.enabled = false;
        StartCoroutine(AIMove());
        resultText.text = "";  // Clear result text
    }
}
