using System;
using System.Drawing;

namespace Tetris;

public class Tetromino
{
    public int[,] Shape { get; private set; }
    public Point Position { get; set; }
    public Color Color { get; }

    private static readonly int[][,] TetrominoShapes = new int[][,]
    {
        new int[,] { {1,1,1,1}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0} },
        new int[,] { {1,1,0}, {0,1,1}, {0,0,0} },
        new int[,] { {0,1,1}, {1,1,0}, {0,0,0} },
        new int[,] { {1,1,1}, {0,0,1}, {0,0,0} },
        new int[,] { {1,1,1}, {1,0,0}, {0,0,0} },
        new int[,] { {1,1}, {1,1} },
        new int[,] { {1,1,1}, {0,1,0}, {0,0,0} }
    };

    private static readonly Color[] TetrominoColors =
    {
        Color.Cyan,
        Color.Red,
        Color.Green,
        Color.Orange,
        Color.Blue,
        Color.Yellow,
        Color.Purple
    };

    public Tetromino(int type)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(type);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(type, TetrominoShapes.Length);

        Shape = new int[4, 4];
        var sourceShape = TetrominoShapes[type];
        int rows = sourceShape.GetLength(0);
        int cols = sourceShape.GetLength(1);

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                Shape[i, j] = sourceShape[i, j];

        Color = TetrominoColors[type];
        Position = new Point(3, 0);
    }

    public void Rotate()
    {
        int[,] newShape = new int[4, 4];
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                newShape[i, j] = Shape[3 - j, i];

        Shape = newShape;
    }

    public void MoveLeft() => Position = new Point(Position.X - 1, Position.Y);
    public void MoveRight() => Position = new Point(Position.X + 1, Position.Y);
    public void MoveDown() => Position = new Point(Position.X, Position.Y + 1);
}