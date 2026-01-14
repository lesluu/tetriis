using System;
using System.Windows.Forms;
using System.Drawing;

namespace Tetris;

public partial class Form1 : Form
{
    private readonly GameBoard gameBoard = new();
    private readonly System.Windows.Forms.Timer gameTimer = new();
    private const int BlockSize = 30;
    private const int PreviewBlockSize = 20;
    private static readonly Brush[] colors = {
        Brushes.Black,
        Brushes.Cyan,
        Brushes.Red,
        Brushes.Green,
        Brushes.Orange,
        Brushes.Blue,
        Brushes.Yellow,
        Brushes.Purple
    };
    private static readonly Font gameFont = new("Arial", 14);
    private static readonly Brush textBrush = Brushes.White;
    private static readonly Button restartButton = new()
    {
        Text = "Рестарт (R)",
        Location = new Point(320, 300),
        Size = new Size(100, 30),
        BackColor = Color.White,
        FlatStyle = FlatStyle.Flat
    };

    public Form1()
    {
        InitializeComponent();

        Text = "Тетрис";
        ClientSize = new Size(450, 600);
        BackColor = Color.Black;
        DoubleBuffered = true;
        KeyPreview = true; 

        gameTimer.Interval = 500;
        gameTimer.Tick += GameTimer_Tick;
        gameTimer.Start();

        Controls.Add(restartButton);
        restartButton.Click += (s, e) => RestartGame();

        Paint += Form1_Paint;
        KeyDown += Form1_KeyDown;
    }

    private void GameTimer_Tick(object sender, EventArgs e)
    {
        if (!gameBoard.GameOver)
        {
            gameBoard.MoveDown();
            Invalidate();
        }
    }

    private void Form1_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;

        for (int i = 0; i < GameBoard.Height; i++)
        {
            for (int j = 0; j < GameBoard.Width; j++)
            {
                if (gameBoard.Board[i, j] != 0)
                {
                    g.FillRectangle(Brushes.Cyan, j * BlockSize, i * BlockSize, BlockSize - 1, BlockSize - 1);
                }
                g.DrawRectangle(Pens.DarkGray, j * BlockSize, i * BlockSize, BlockSize, BlockSize);
            }
        }

        if (!gameBoard.GameOver)
        {
            var piece = gameBoard.CurrentPiece;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (piece.Shape[i, j] != 0)
                    {
                        int x = (piece.Position.X + j) * BlockSize;
                        int y = (piece.Position.Y + i) * BlockSize;
                        g.FillRectangle(new SolidBrush(piece.Color), x + 1, y + 1, BlockSize - 1, BlockSize - 1);
                    }
                }
            }
        }

        var nextPiece = gameBoard.NextPiece;
        g.DrawString("Следующая:", gameFont, textBrush, 320, 100);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (nextPiece.Shape[i, j] != 0)
                {
                    int x = 320 + j * PreviewBlockSize;
                    int y = 130 + i * PreviewBlockSize;
                    g.FillRectangle(new SolidBrush(nextPiece.Color), x + 1, y + 1, PreviewBlockSize - 1, PreviewBlockSize - 1);
                    g.DrawRectangle(Pens.DarkGray, x, y, PreviewBlockSize, PreviewBlockSize);
                }
            }
        }

        g.DrawString($"Счет: {gameBoard.Score}", gameFont, textBrush, 320, 30);
        g.DrawString($"Рекорд: {gameBoard.HighScore}", gameFont, textBrush, 320, 60);

        if (gameBoard.GameOver)
        {
            g.DrawString("ИГРА ОКОНЧЕНА", gameFont, textBrush, 320, 250);
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (!gameBoard.GameOver)
        {
            switch (keyData)
            {
                case Keys.Left:
                    gameBoard.MoveLeft();
                    break;
                case Keys.Right:
                    gameBoard.MoveRight();
                    break;
                case Keys.Down:
                    gameBoard.MoveDown();
                    break;
                case Keys.Up:
                    gameBoard.Rotate();
                    break;
            }
            Invalidate();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.R)
        {
            RestartGame();
            e.Handled = true;
        }
    }

    private void RestartGame()
    {
        gameBoard.Restart();
        gameTimer.Start();
        Invalidate();
    }
}