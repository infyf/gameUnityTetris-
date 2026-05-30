using System.Collections.Generic;
using UnityEngine;
using Models;
using Core;

namespace Managers
{
    public class PieceManager
    {
        private BoardManager board;

        public List<Grid> currentPiece;
        public List<GameObject> pieceObjects;

        private GameObject prefab;

        public PieceManager(BoardManager boardManager, GameObject gamePiece)
        {
            board = boardManager;

            currentPiece = new List<Grid>();
            pieceObjects = new List<GameObject>();

            prefab = gamePiece;
        }

        public void SpawnPiece()
        {
            currentPiece.Clear();

            currentPiece.Add(new Grid() { x = 7, y = 0 });
            currentPiece.Add(new Grid() { x = 8, y = 0 });
            currentPiece.Add(new Grid() { x = 7, y = 1 });
            currentPiece.Add(new Grid() { x = 8, y = 1 });

            CreateVisualObjects();
        }

        private void CreateVisualObjects()
        {
            pieceObjects.Clear();

            foreach (var block in currentPiece)
            {
                GameObject obj = Object.Instantiate(prefab);

                obj.transform.position =
                    new Vector3(block.x, -block.y, -1);

                pieceObjects.Add(obj);
            }
        }

        public void Move(int dx, int dy)
        {
            for (int i = 0; i < currentPiece.Count; i++)
            {
                currentPiece[i] = new Grid()
                {
                    x = currentPiece[i].x + dx,
                    y = currentPiece[i].y + dy
                };
            }

            UpdateVisual();
        }

        public void Rotate()
        {
            Grid center = currentPiece[1];

            for (int i = 0; i < currentPiece.Count; i++)
            {
                int x = currentPiece[i].y - center.y;
                int y = currentPiece[i].x - center.x;

                currentPiece[i] = new Grid()
                {
                    x = center.x - x,
                    y = center.y + y
                };
            }

            UpdateVisual();
        }

        public void UpdateVisual()
        {
            for (int i = 0; i < pieceObjects.Count; i++)
            {
                pieceObjects[i].transform.position =
                    new Vector3(
                        currentPiece[i].x,
                        -currentPiece[i].y,
                        -1
                    );
            }
        }

        public bool CheckValidPosition()
        {
            foreach (var block in currentPiece)
            {
                if (block.x < 0 || block.x >= 16)
                    return false;

                if (block.y >= 22)
                    return false;

                if (block.y >= 0 &&
                    board.IsFilled(block.x, block.y))
                    return false;
            }

            return true;
        }

        public void SettlePiece()
        {
            foreach (var block in currentPiece)
            {
                board.SetCell(block.x, block.y, true);
            }
        }

        public void DestroyPiece()
        {
            foreach (var obj in pieceObjects)
            {
                Object.Destroy(obj);
            }

            pieceObjects.Clear();
        }
    }
}
