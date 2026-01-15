using System;
using System.Drawing;
using System.IO;

namespace Tetris;

public class GameBoard
{
    public int[,] Board { get; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public bool GameOver { get; private set; }
    public Tetromino CurrentPiece { get; private set; } = null!;
    public Tetromino NextPiece { get; private set; } = null!;
    public Tetromino? HoldPiece { get; private set; }
    private readonly Random random = new();
    private bool canHold = true; 
    private const string HighScoreFile = "highscore.txt";

    public const int Width = 10;
    public const int Height = 20;

    public int TotalHardDrops { get; private set; }
    public int LastHardDropDistance { get; private set; }

    public GameBoard()
    {
        Board = new int[Height, Width];
        Score = 0;
        HighScore = LoadHighScore();
        GameOver = false;
        NextPiece = new Tetromino(random.Next(7));
        SpawnNewPiece();
    }

    private int LoadHighScore()
    {
        try
        {
            if (File.Exists(HighScoreFile))
            {
                string content = File.ReadAllText(HighScoreFile);
                if (int.TryParse(content, out int highScore))
                {
                    return highScore;
                }
            }
        }
        catch (Exception)
        {

        }
        return 0;
    }

    private void SaveHighScore()
    {
        try
        {
            File.WriteAllText(HighScoreFile, HighScore.ToString());
        }
        catch (Exception)
        {
            
        }
    }

    public void SpawnNewPiece()
    {
        CurrentPiece = NextPiece;
        NextPiece = new Tetromino(random.Next(7));
        canHold = true; 
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

    
    public bool Hold()
    {
        if (!canHold || GameOver) return false;

        if (HoldPiece == null)
        {
            HoldPiece = CurrentPiece;
            HoldPiece.Position = new Point(3, 0);
            SpawnNewPiece();
        }
        else
        {
            var temp = HoldPiece;
            HoldPiece = CurrentPiece;
            HoldPiece.Position = new Point(3, 0);
            CurrentPiece = temp;
            CurrentPiece.Position = new Point(3, 0);

            if (IsCollision())
            {
                CurrentPiece = HoldPiece;
                HoldPiece = temp;
                HoldPiece.Position = new Point(3, 0);
                return false;
            }
        }

        canHold = false; 
        return true;
    }

    public int HardDrop()
    {
        if (GameOver) return 0;

        int distance = 0;

        while (MoveDown())
        {
            distance++;
        }

        if (distance > 0)
        {
            int bonus = distance * 2; 
            Score += bonus;

            TotalHardDrops++;
            LastHardDropDistance = distance;

            if (Score > HighScore)
            {
                HighScore = Score;
                SaveHighScore();
            }
        }

        return distance;
    }

    public bool SoftDrop()
    {
        if (GameOver) return false;

        CurrentPiece.MoveDown();
        if (IsCollision())
        {
            CurrentPiece.Position = new Point(CurrentPiece.Position.X, CurrentPiece.Position.Y - 1);
            MergePiece();
            ClearLines();
            SpawnNewPiece();
            return false;
        }
        else
        {
            Score += 1;

            if (Score > HighScore)
            {
                HighScore = Score;
                SaveHighScore();
            }

            return true;
        }
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
            SaveHighScore();
        }
    }

    public void Restart()
    {
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                Board[i, j] = 0;

        Score = 0;
        TotalHardDrops = 0;
        LastHardDropDistance = 0;
        GameOver = false;
        HoldPiece = null;
        canHold = true;

        NextPiece = new Tetromino(random.Next(7));
        SpawnNewPiece();
    }
}
