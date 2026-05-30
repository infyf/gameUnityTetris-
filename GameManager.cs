using System.Collections;
using UnityEngine;

using Core;
using Managers;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gamePiece;

    private BoardManager boardManager;
    private PieceManager pieceManager;
    private InputManager inputManager;
    private LineManager lineManager;

    private bool gameOver;

    private float normalSpeed = 0.3f;
    private float fastSpeed = 0.1f;

    private void Awake()
    {
        boardManager = new BoardManager();

        pieceManager =
            new PieceManager(boardManager, gamePiece);

        inputManager = new InputManager();

        lineManager =
            new LineManager(boardManager);

        StartGame();
    }

    private void Update()
    {
        if (gameOver)
            return;

        inputManager.HandleInput();
    }

    private void StartGame()
    {
        boardManager.InitializeBoard();

        pieceManager.SpawnPiece();

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (!gameOver)
        {
            float speed =
                inputManager.fastFall
                ? fastSpeed
                : normalSpeed;

            yield return new WaitForSeconds(speed);

            HandleMovement();

            pieceManager.Move(0, 1);

            if (!pieceManager.CheckValidPosition())
            {
                pieceManager.Move(0, -1);

                pieceManager.SettlePiece();

                lineManager.ClearLines();

                pieceManager.SpawnPiece();

                if (!pieceManager.CheckValidPosition())
                {
                    gameOver = true;
                }
            }
        }
    }

    private void HandleMovement()
    {
        if (inputManager.direction != 0)
        {
            pieceManager.Move(inputManager.direction, 0);

            if (!pieceManager.CheckValidPosition())
            {
                pieceManager.Move(
                    -inputManager.direction,
                    0
                );
            }
        }

        if (inputManager.rotate)
        {
            pieceManager.Rotate();

            if (!pieceManager.CheckValidPosition())
            {
                pieceManager.Rotate();
                pieceManager.Rotate();
                pieceManager.Rotate();
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
