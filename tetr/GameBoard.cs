using System;
using System.Drawing;

namespace Tetris;

public class GameBoard
{
    public int[,] Board { get; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public bool GameOver { get; private set; }
    public Tetromino CurrentPiece { get; private set; } = null!;
    public Tetromino NextPiece { get; private set; } = null!;
    private readonly Random random = new();

    public const int Width = 10;
    public const int Height = 20;

    public GameBoard()
    {
        Board = new int[Height, Width];
        Score = 0;
        HighScore = 0;
        GameOver = false;
        NextPiece = new Tetromino(random.Next(7));
        SpawnNewPiece();
    }

    public void SpawnNewPiece()
    {
        CurrentPiece = NextPiece;
        NextPiece = new Tetromino(random.Next(7));
        if (IsCollision())
        {
            GameOver = true;
        }
    }

    public bool MoveLeft()
    {
        CurrentPiece.MoveLeft();
        if (IsCollision())
        {
            CurrentPiece.MoveRight();
            return false;
        }
        return true;
    }

    public bool MoveRight()
    {
        CurrentPiece.MoveRight();
        if (IsCollision())
        {
            CurrentPiece.MoveLeft();
            return false;
        }
        return true;
    }

    public bool MoveDown()
    {
        CurrentPiece.MoveDown();
        if (IsCollision())
        {
            CurrentPiece.Position = new Point(CurrentPiece.Position.X, CurrentPiece.Position.Y - 1);
            MergePiece();
            ClearLines();
            SpawnNewPiece();
            return false;
        }
        return true;
    }

    public void Rotate()
    {
        CurrentPiece.Rotate();
        if (IsCollision())
        {
            for (int i = 0; i < 3; i++)
                CurrentPiece.Rotate();
        }
    }

    private bool IsCollision()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (CurrentPiece.Shape[i, j] != 0)
                {
                    int x = CurrentPiece.Position.X + j;
                    int y = CurrentPiece.Position.Y + i;

                    if (x < 0 || x >= Width || y >= Height)
                        return true;
                    if (y >= 0 && Board[y, x] != 0)
                        return true;
                }
            }
        }
        return false;
    }

    private void MergePiece()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (CurrentPiece.Shape[i, j] != 0)
                {
                    int x = CurrentPiece.Position.X + j;
                    int y = CurrentPiece.Position.Y + i;
                    if (y >= 0)
                        Board[y, x] = 1;
                }
            }
        }
    }

    private void ClearLines()
    {
        int linesCleared = 0;
        for (int i = Height - 1; i >= 0; i--)
        {
            bool isLineFull = true;
            for (int j = 0; j < Width; j++)
            {
                if (Board[i, j] == 0)
                {
                    isLineFull = false;
                    break;
                }
            }

            if (isLineFull)
            {
                for (int k = i; k > 0; k--)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Board[k, j] = Board[k - 1, j];
                    }
                }
                linesCleared++;
                i++;
            }
        }

        int points = linesCleared == 4 ? 800 : linesCleared * 100;
        Score += points;

        if (Score > HighScore)
        {
            HighScore = Score;
        }
    }

    public void Restart()
    {
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                Board[i, j] = 0;

        Score = 0;
        GameOver = false;

        NextPiece = new Tetromino(random.Next(7));
        SpawnNewPiece();
    }
}
