using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Grid { public int x, y; }

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gamePiece;

    private List<GameObject> pieceGameObjects;
    private Dictionary<Grid, GameObject> gridObjects;

    private List<Grid> currIndexes;
    private List<Grid> tmpIndexes;
    private float normalWaitTime = 0.3f;
    private float fastFallWaitTime = 0.1f;
    private float waitTime;

    private bool[,] fill = new bool[16, 22];
    private List<List<Grid>> pieces;

    private int direction;
    private bool canRotate;
    private bool gameOver;
    private bool isFastFalling;

    private void Awake()
    {
        InitializeGame();
        SpawnNewPiece();
        StartCoroutine(UpdateTurn());
    }

    private void InitializeGame()
    {
        currIndexes = new List<Grid>();
        tmpIndexes = new List<Grid>();
        pieceGameObjects = new List<GameObject>();
        direction = 0;
        canRotate = false;
        gameOver = false;
        waitTime = normalWaitTime;
        gridObjects = new Dictionary<Grid, GameObject>();
        isFastFalling = false;

        pieces = new List<List<Grid>>()
        {
            new List<Grid> { new Grid() { x = 1, y = 0 }, new Grid() { x = 1, y = 1 }, new Grid() { x = 1, y = 2 }, new Grid() { x = 1, y = 3 } },
            new List<Grid> { new Grid() { x = 0, y = 1 }, new Grid() { x = 0, y = 2 }, new Grid() { x = 1, y = 2 }, new Grid() { x = 1, y = 3 } },
            new List<Grid> { new Grid() { x = 1, y = 1 }, new Grid() { x = 1, y = 2 }, new Grid() { x = 0, y = 2 }, new Grid() { x = 0, y = 3 } },
            new List<Grid> { new Grid() { x = 1, y = 1 }, new Grid() { x = 1, y = 2 }, new Grid() { x = 0, y = 2 }, new Grid() { x = 1, y = 3 } },
            new List<Grid> { new Grid() { x = 0, y = 1 }, new Grid() { x = 1, y = 1 }, new Grid() { x = 1, y = 2 }, new Grid() { x = 1, y = 3 } },
            new List<Grid> { new Grid() { x = 1, y = 1 }, new Grid() { x = 1, y = 2 }, new Grid() { x = 1, y = 3 }, new Grid() { x = 0, y = 3 } },
            new List<Grid> { new Grid() { x = 0, y = 1 }, new Grid() { x = 1, y = 1 }, new Grid() { x = 0, y = 2 }, new Grid() { x = 1, y = 2 } }
        };

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 22; j++)
            {
                fill[i, j] = false;
            }
        }
    }

    private void Update()
    {
        if (gameOver) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) direction = -1;
        else if (Input.GetKeyDown(KeyCode.D)) direction = 1;
        if (Input.GetKeyDown(KeyCode.W)) canRotate = true;
        if (Input.GetKeyDown(KeyCode.S)) isFastFalling = true;
        if (Input.GetKeyUp(KeyCode.S)) isFastFalling = false;
    }

    private IEnumerator UpdateTurn()
    {
        waitTime = isFastFalling ? fastFallWaitTime : normalWaitTime;
        yield return new WaitForSeconds(waitTime);

        MovePiece(direction);
        if (canRotate) RotatePiece();
        MovePieceDown();

        if (!CheckValidPosition())
        {
            SettlePiece();
            if (gameOver)
            {
                ClearAllPieces();
                yield break;
            }
            SpawnNewPiece();
        }

        ClearFullRows();
        ResetTurnVariables();

        if (!gameOver)
        {
            StartCoroutine(UpdateTurn());
        }
    }

    private void MovePiece(int direction)
    {
        UpdateGridIndexes(direction, 0);
        if (!CheckValidPosition())
        {
            UpdateGridIndexes(-direction, 0);
        }
        else
        {
            UpdatePieceGameObjects();
        }
    }

    private void RotatePiece()
    {
        if (currIndexes.Count == 0) return;

        Grid centerRotation = currIndexes[1];

        tmpIndexes.Clear();
        tmpIndexes.AddRange(currIndexes);


        for (int i = 0; i < currIndexes.Count; i++)
        {
            int x = currIndexes[i].y - centerRotation.y;
            int y = currIndexes[i].x - centerRotation.x;
            currIndexes[i] = new Grid { x = centerRotation.x - x, y = centerRotation.y + y };
        }

        if (CheckValidPosition())
        {
            UpdatePieceGameObjects();
        }
        else
        {
            currIndexes.Clear();
            currIndexes.AddRange(tmpIndexes);
        }
    }

    private void MovePieceDown()
    {
        UpdateGridIndexes(0, 1);
        if (!CheckValidPosition())
        {
            currIndexes = new List<Grid>(tmpIndexes);
            if (!SettlePiece())
            {
                gameOver = true;
                ClearAllPieces();
                return;
            }

            SpawnNewPiece();
        }
        else
        {
            UpdatePieceGameObjects();
        }
    }

    private void UpdateGridIndexes(int deltaX, int deltaY)
    {
        for (int i = 0; i < currIndexes.Count; i++)
        {
            tmpIndexes[i] = currIndexes[i];
            currIndexes[i] = new Grid { x = currIndexes[i].x + deltaX, y = currIndexes[i].y + deltaY };
        }
    }

    private void UpdatePieceGameObjects()
    {
        for (int i = 0; i < pieceGameObjects.Count; i++)
        {
            pieceGameObjects[i].transform.position = new Vector3(currIndexes[i].x, -currIndexes[i].y, -1f);
        }
    }

    private bool CheckValidPosition()
    {
        foreach (var index in currIndexes)
        {
            if (index.x < 0 || index.x >= 16 || index.y >= 22 || fill[index.x, index.y])
            {
                return false;
            }
        }
        return true;
    }

    private bool SettlePiece()
    {
        for (int i = 0; i < currIndexes.Count; i++)
        {
            fill[currIndexes[i].x, currIndexes[i].y] = true;
            gridObjects[currIndexes[i]] = pieceGameObjects[i];
        }

        foreach (var index in currIndexes)
        {
            if (index.y < 0)
            {
                return false;
            }
        }
        return true;
    }

    private void SpawnNewPiece()
    {
        int n = Random.Range(0, pieces.Count);
        currIndexes.Clear();
        tmpIndexes.Clear();
        pieceGameObjects.Clear();

        for (int i = 0; i < pieces[n].Count; i++)
        {
            Grid tempGrid = pieces[n][i];
            tempGrid.x += 7;
            currIndexes.Add(tempGrid);
            tmpIndexes.Add(tempGrid);
        }

        for (int i = 0; i < currIndexes.Count; i++)
        {
            GameObject temp = Instantiate(gamePiece);
            temp.transform.position = new Vector3(currIndexes[i].x, -currIndexes[i].y, -1f);
            pieceGameObjects.Add(temp);
        }

        if (!CheckValidPosition())
        {
            gameOver = true;
            ClearAllPieces();
        }
    }

    private void ClearFullRows()
    {
        for (int i = 21; i > 0; i--)
        {
            if (IsRowFull(i))
            {
                ClearRow(i);
                i++;
            }
        }
    }

    private bool IsRowFull(int row)
    {
        for (int k = 0; k < 16; k++)
        {
            if (!fill[k, row]) return false;
        }
        return true;
    }

    private void ClearRow(int row)
    {
        for (int j = 0; j < 16; j++)
        {
            if (gridObjects.TryGetValue(new Grid() { x = j, y = row }, out GameObject tempObject))
            {
                tempObject.SetActive(false);
                gridObjects.Remove(new Grid() { x = j, y = row });
            }
        }

        ShiftRowsDown(row);
        UpdateGridObjectsAfterRowClear(row);
    }

    private void ShiftRowsDown(int fromRow)
    {
        for (int j = fromRow; j > 0; j--)
        {
            for (int k = 0; k < 16; k++)
            {
                fill[k, j] = fill[k, j - 1];
            }
        }

        for (int k = 0; k < 16; k++)
        {
            fill[k, 0] = false;
        }
    }

    private void UpdateGridObjectsAfterRowClear(int clearedRow)
    {
        Dictionary<Grid, GameObject> updatedGridObjects = new Dictionary<Grid, GameObject>();

        foreach (var pair in gridObjects)
        {
            Grid keyGrid = pair.Key;
            GameObject currentObject = pair.Value;
            if (keyGrid.y < clearedRow)
            {
                keyGrid.y += 1;
                currentObject.transform.position = new Vector3(keyGrid.x, -keyGrid.y, -1f);
            }
            updatedGridObjects[keyGrid] = currentObject;
        }

        gridObjects = updatedGridObjects;
    }

    private void ResetTurnVariables()
    {
        direction = 0;
        canRotate = false;
        waitTime = normalWaitTime;
    }

    private void ClearAllPieces()
    {
        foreach (var piece in pieceGameObjects)
        {
            Destroy(piece);
        }
        pieceGameObjects.Clear();

        foreach (var obj in gridObjects.Values)
        {
            Destroy(obj);
        }
        gridObjects.Clear();

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 22; j++)
            {
                fill[i, j] = false;
            }
        }
    }

    public void GameRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
