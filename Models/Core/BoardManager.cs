using UnityEngine;

namespace Core
{
    public class BoardManager
    {
        public bool[,] fill;

        public int width = 16;
        public int height = 22;

        public BoardManager()
        {
            fill = new bool[16, 22];
        }

        public void InitializeBoard()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fill[x, y] = false;
                }
            }
        }

        public bool IsFilled(int x, int y)
        {
            return fill[x, y];
        }

        public void SetCell(int x, int y, bool value)
        {
            fill[x, y] = value;
        }
    }
}
