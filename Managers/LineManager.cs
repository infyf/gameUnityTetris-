using Core;

namespace Managers
{
    public class LineManager
    {
        private BoardManager board;

        public LineManager(BoardManager boardManager)
        {
            board = boardManager;
        }

        public void ClearLines()
        {
            for (int y = 21; y >= 0; y--)
            {
                bool full = true;

                for (int x = 0; x < 16; x++)
                {
                    if (!board.fill[x, y])
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    RemoveLine(y);
                    y++;
                }
            }
        }

        private void RemoveLine(int row)
        {
            for (int y = row; y > 0; y--)
            {
                for (int x = 0; x < 16; x++)
                {
                    board.fill[x, y] =
                        board.fill[x, y - 1];
                }
            }

            for (int x = 0; x < 16; x++)
            {
                board.fill[x, 0] = false;
            }
        }
    }
}
