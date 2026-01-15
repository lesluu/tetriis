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
    private const int HoldBlockSize = 15;
    private static readonly Font gameFont = new("Arial", 14);
    private static readonly Font smallFont = new("Arial", 10);
    private static readonly Brush textBrush = Brushes.White;
    private static readonly Brush bonusBrush = Brushes.Gold;
    private static readonly Button restartButton = new()
    {
        Text = "Рестарт (R)",
        Location = new Point(320, 430), 
        Size = new Size(100, 30),
        BackColor = Color.White,
        FlatStyle = FlatStyle.Flat
    };

    private System.Windows.Forms.Timer bonusTimer = new();
    private int bonusDisplayTime = 0;
    private string lastBonusText = "";

    public Form1()
    {
        InitializeComponent();

        Text = "Тетрис";
        ClientSize = new Size(650, 600); 
        BackColor = Color.Black;
        DoubleBuffered = true;
        KeyPreview = true; 

        gameTimer.Interval = 500;
        gameTimer.Tick += GameTimer_Tick;
        gameTimer.Start();

        bonusTimer.Interval = 100;
        bonusTimer.Tick += BonusTimer_Tick;

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

    private void BonusTimer_Tick(object sender, EventArgs e)
    {
        if (bonusDisplayTime > 0)
        {
            bonusDisplayTime--;
            if (bonusDisplayTime <= 0)
            {
                lastBonusText = "";
                bonusTimer.Stop();
            }
            Invalidate();
        }
    }

    private void ShowBonus(string text)
    {
        lastBonusText = text;
        bonusDisplayTime = 50; 
        bonusTimer.Start();
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
        g.DrawString("Следующая:", gameFont, textBrush, 320, 30);
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (nextPiece.Shape[i, j] != 0)
                {
                    int x = 320 + j * PreviewBlockSize;
                    int y = 60 + i * PreviewBlockSize;
                    g.FillRectangle(new SolidBrush(nextPiece.Color), x + 1, y + 1, PreviewBlockSize - 1, PreviewBlockSize - 1);
                    g.DrawRectangle(Pens.DarkGray, x, y, PreviewBlockSize, PreviewBlockSize);
                }
            }
        }

        g.DrawString("Hold:", gameFont, textBrush, 320, 150);
        var holdPiece = gameBoard.HoldPiece;
        if (holdPiece != null)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (holdPiece.Shape[i, j] != 0)
                    {
                        int x = 320 + j * HoldBlockSize;
                        int y = 180 + i * HoldBlockSize;
                        g.FillRectangle(new SolidBrush(holdPiece.Color), x + 1, y + 1, HoldBlockSize - 1, HoldBlockSize - 1);
                        g.DrawRectangle(Pens.DarkGray, x, y, HoldBlockSize, HoldBlockSize);
                    }
                }
            }
        }
        else
        {
            g.DrawString("Пусто", smallFont, textBrush, 320, 180);
        }

        g.DrawString($"Счет: {gameBoard.Score}", gameFont, textBrush, 320, 250);
        g.DrawString($"Рекорд: {gameBoard.HighScore}", gameFont, textBrush, 320, 280);

        g.DrawString($"Жестких падений: {gameBoard.TotalHardDrops}", smallFont, textBrush, 320, 320);
        g.DrawString($"Последнее: {gameBoard.LastHardDropDistance} клеток", smallFont, textBrush, 320, 340);

        if (!string.IsNullOrEmpty(lastBonusText))
        {
            float alpha = Math.Min(1.0f, bonusDisplayTime / 20.0f); 
            using (var bonusBrush = new SolidBrush(Color.FromArgb((int)(alpha * 255), Color.Gold)))
            {
                g.DrawString(lastBonusText, new Font("Arial", 16, FontStyle.Bold),
                           bonusBrush, 320, 380);
            }
        }

        if (gameBoard.GameOver)
        {
            g.DrawString("ИГРА ОКОНЧЕНА", gameFont, textBrush, 320, 400);
        }

        g.DrawString("Управление:", smallFont, textBrush, 450, 30);
        g.DrawString("← → : Движение", smallFont, textBrush, 450, 50);
        g.DrawString("↑ : Поворот", smallFont, textBrush, 450, 65);
        g.DrawString("↓ : Мягкое падение", smallFont, textBrush, 450, 80);
        g.DrawString("Пробел : Жесткое падение", smallFont, textBrush, 450, 95);
        g.DrawString("C : Hold фигура", smallFont, textBrush, 450, 110);
        g.DrawString("R : Рестарт", smallFont, textBrush, 450, 125);
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
                    if (gameBoard.SoftDrop())
                    {
                        ShowBonus("+1");
                    }
                    break;
                case Keys.Up:
                    gameBoard.Rotate();
                    break;
                case Keys.Space:
                    int distance = gameBoard.HardDrop();
                    if (distance > 0)
                    {
                        ShowBonus($"+{distance * 2} за {distance} клеток!");
                    }
                    Invalidate();
                    return true;
                case Keys.C:
                    if (gameBoard.Hold())
                    {
                        ShowBonus("Фигура удержана");
                    }
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
        lastBonusText = "";
        bonusDisplayTime = 0;
        Invalidate();
    }
}
