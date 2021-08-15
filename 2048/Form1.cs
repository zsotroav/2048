using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2048
{
    public partial class Form1 : Form
    {
        public readonly Dictionary<int,Color> Colors = new()
        {
            { 0, Color.FromArgb(204, 192, 178) },
            { 2, Color.FromArgb(238, 228, 218) },
            { 4, Color.FromArgb(237, 224, 200) },
            { 8, Color.FromArgb(242, 177, 121) },
            { 16, Color.FromArgb(245, 149, 99) },
            { 32, Color.FromArgb(246, 124, 95) },
            { 64, Color.FromArgb(246, 94, 59) },
            { 128, Color.FromArgb(237, 207, 114) },
            { 256, Color.FromArgb(237, 204, 97) },
            { 512, Color.FromArgb(237, 200, 80) },
            { 1024, Color.FromArgb(237, 197, 63) },
            { 2048, Color.FromArgb(237, 194, 46) },
            { 4096, Color.FromArgb(110, 204, 19) },
            { 8192, Color.FromArgb(100, 192, 11) },
            { 16384, Color.FromArgb(84, 168, 2) },
            { 32768, Color.FromArgb(72, 144, 2) }
        };
        public readonly Color ColorPlus = Color.FromArgb(89, 137, 247);

        public Panel[,] GameBoard;
        public Label[,] GameLabels;

        public int[,] GameState = new int[4, 4];
        public int[,] GameStatePrev = new int[4, 4];

        public bool[,] GameFree = new bool[4, 4];
        public bool[,] GameFreePrev = new bool[4, 4];

        public int Score = 0;
        
        public Form1()
        {
            InitializeComponent();
            Init(null, null);
        }

        private void Init(object sender, EventArgs e)
        {
            Score = 0;
            LScore.Text = "Score: 0";
            LHScore.Text = $"High score: {HighScore}";
            GameBoard = new Panel[,]
            {
                { PA1, PB1, PC1, PD1},
                { PA2, PB2, PC2, PD2},
                { PA3, PB3, PC3, PD3},
                { PA4, PB4, PC4, PD4}
            };
            GameLabels = new Label[,]
            {
                { LA1, LB1, LC1, LD1},
                { LA2, LB2, LC2, LD2},
                { LA3, LB3, LC3, LD3},
                { LA4, LB4, LC4, LD4}
            };
            
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameState[i, j] = 0;
                    UpdDisp(i, j);
                }
            }
            Generate();

        }

        private void Generate()
        {
            bool ok = false;
            for (int i = 0; i < 4 && !ok; i++)
            {
                for (int j = 0; j < 4 && !ok; j++)
                {
                    if (GameFree[i, j])
                        ok = true;
                }
            }

            if (!ok)
            {
                MessageBox.Show("Game over! You ran out of free spaces.", "Game over", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Random R = new();
            bool work = true;
            int x = -1;
            int y = -1;
            while (work)
            {
                x = R.Next(0, 4);
                y = R.Next(0, 4);
                work = !GameFree[x, y];
            }

            if (R.Next(0, 10) == 0)
                GameState[x, y] = 4;
            else
                GameState[x, y] = 2;

            UpdDisp(x, y);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void UpdDisp(int x, int y)
        {
            Color col;
            col = Colors.TryGetValue(GameState[x, y], out col) ? col : ColorPlus;
            GameBoard[x, y].BackColor = col;
            
            if (GameState[x, y] == 0)
            {
                GameLabels[x, y].Text = "";
                GameFree[x, y] = true;
            }
            else
            {
                GameLabels[x, y].Text = GameState[x, y].ToString();
                GameFree[x, y] = false;

                if (GameLabels[x, y].Text.Length > 4)
                    GameLabels[x, y].Font = new Font("Microsoft Sans Serif", 9F,
                        FontStyle.Bold, GraphicsUnit.Point);
                else
                    GameLabels[x, y].Font = new Font("Microsoft Sans Serif", 12F,
                        FontStyle.Bold, GraphicsUnit.Point);
            }
        }

        private void Backup()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameStatePrev[i,j] = GameState[i,j];
                    GameFreePrev[i,j] = GameFree[i,j];

                }
            }
        }

        private void Restore(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameState[i, j] = GameStatePrev[i, j];
                    GameFree[i, j] = GameFreePrev[i, j];
                    UpdDisp(i, j);
                }
            }
        }

        private void Scoring(int Points)
        {
            Score += Points;
            LScore.Text = $"Score: {Score}";
            if (Score > HighScore)
            {
                LHScore.Text = $"High score: {Score}";
                HighScore = Score;
            }
        }

        public static readonly string WritePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string DirPath = Path.Combine(WritePath, "zsotroav", "2048");
        public static readonly string MainPath = Path.Combine(DirPath, "HighScore.bin");
        private static int HighScore
        {
            get
            {
                if (File.Exists(MainPath))
                {
                    using StreamReader sr = new(MainPath);
                    int re = int.Parse(sr.ReadLine() ?? "0");
                    sr.Close();
                    return re;
                }
                else
                {
                    Directory.CreateDirectory(DirPath);
                    File.Create(MainPath).Close();
                    return 0;
                }
            }
            set
            {
                using StreamWriter sw = new(MainPath);
                sw.WriteLine(value);
                sw.Close();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Backup();
            switch (e.KeyData)
            {
                case Keys.NumPad0:
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            GameState[i, j] = (int)Math.Pow(2, t);
                            UpdDisp(i, j);
                            t++;
                        }
                    }

                    GameState[0, 0] = 0;
                    UpdDisp(0, 0);
                    break;
                case Keys.NumPad1:
                    GameState = new int[4, 4]
                    {
                        {0,32,32,32},
                        {0,0,32,32},
                        {32,32,32,32},
                        {0,0,0,32}
                    };
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            UpdDisp(i, j);
                        }
                    }
                    break;
                case Keys.Left:
                    MvLU(0, 1);
                    break;
                case Keys.Up:
                    MvLU(1, 0);
                    break;
                case Keys.Down:
                    MvRD(1, 0);
                    break;
                case Keys.Right:
                    MvRD(0, 1);
                    break;
            }
        }

        private void MvLU(int x, int y)
        {
            var helper = new bool[4, 4];
            for (int i = 3; i - x >= 0; i--)        // Row
            {
                for (int j = 3; j - y >= 0; j--)    // Column
                { 
                    if (GameState[i, j] != 0 && GameState[i - x, j - y] == 0)
                    {
                        GameState[i - x, j - y] = GameState[i, j];
                        UpdDisp(i - x, j - y);
                        GameState[i, j] = 0;
                        UpdDisp(i, j);
                    } else if (GameState[i, j] == GameState[i - x, j - y] && GameState[i, j] != 0 && !helper[i, j])
                    {
                        int x2 = x;
                        int y2 = y;
                        if (((i - 2 * x == 1 && x != 0) || (j - 2 * y == 1 && y != 0)) &&
                            GameState[i - 2 * x, j - 2 * y] == GameState[i, j] &&
                            GameState[i - 3 * x, j - 3 * y] != GameState[i, j])
                        {
                            x2 *= 2;
                            y2 *= 2;
                        }
                        else if (((i - 2 * x == 0 && x != 0) || (j - 2 * y == 0 && y != 0)) &&
                                 GameState[i - 2 * x, j - 2 * y] == GameState[i, j])
                        {
                            x2 *= 2;
                            y2 *= 2;
                        }

                        helper[i - x2, j - y2] = true;
                        GameState[i - x2, j - y2] *= 2;
                        Scoring(GameState[i - x2, j - y2]);
                        UpdDisp(i - x2, j - y2);
                        GameState[i, j] = 0;
                        UpdDisp(i, j);
                    }

                    if (x == 1 && i < 3 && GameState[i + x, j] != 0 && GameState[i, j] == 0)
                    {
                        GameState[i, j] = GameState[i + x, j];
                        UpdDisp(i,j);
                        GameState[i + x, j] = 0;
                        UpdDisp(i + x, j);
                    }
                    if (y == 1 && j < 3 && GameState[i, j + y] != 0 && GameState[i,j] == 0)
                    {
                        GameState[i, j] = GameState[i, j + y];
                        UpdDisp(i,j);
                        GameState[i, j + y] = 0;
                        UpdDisp(i, j + y);
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (x == 1 && GameState[3, i] != 0 && GameState[2, i] == 0) // UP
                {
                    GameState[2, i] = GameState[3, i];
                    UpdDisp(2, i);
                    GameState[3, i] = 0;
                    UpdDisp(3, i);
                }
                if (y == 1 && GameState[i, 3] != 0 && GameState[i, 2] == 0) // LEFT
                {
                    GameState[i, 2] = GameState[i, 3];
                    UpdDisp(i, 2);
                    GameState[i, 3] = 0;
                    UpdDisp(i, 3);
                }
            }

            Generate();
        }


        private void MvRD(int x, int y)
        {
            bool[,] helper = new bool[4, 4];
            for (int i = 0; i + x <= 3; i++)        // Row
            {
                for (int j = 0; j + y <= 3; j++)    // Column
                {
                    if (GameState[i, j] != 0 && GameState[i + x, j + y] == 0)
                    {
                        GameState[i + x, j + y] = GameState[i, j];
                        UpdDisp(i + x, j + y);
                        GameState[i, j] = 0;
                        UpdDisp(i, j);
                    }
                    else if (GameState[i, j] == GameState[i + x, j + y] && GameState[i, j] != 0 && !helper[i, j])
                    {
                        int x2 = x;
                        int y2 = y;
                        if (((i + 2 * x == 2 && x != 0) || (j + 2 * y == 2 && y != 0)) &&
                            GameState[i + 2 * x, j + 2 * y] == GameState[i, j] &&
                            GameState[i + 3 * x, j + 3 * y] != GameState[i, j])
                        {
                            x2 *= 2;
                            y2 *= 2;
                        }
                        else if (((i + 2 * x == 3 && x != 0) || (j + 2 * y == 3 && y != 0)) &&
                                 GameState[i + 2 * x, j + 2 * y] == GameState[i, j])
                        {
                            x2 *= 2;
                            y2 *= 2;
                        }

                        helper[i + x2, j + y2] = true;
                        GameState[i + x2, j + y2] *= 2;
                        Scoring(GameState[i + x2, j + y2]);
                        UpdDisp(i + x2, j + y2);
                        GameState[i, j] = 0;
                        UpdDisp(i, j);
                    }

                    if (x == 1 && i > 1 && GameState[i - x, j] != 0 && GameState[i,j] == 0)
                    {
                        GameState[i, j] = GameState[i - x, j];
                        UpdDisp(i, j);
                        GameState[i - x, j] = 0;
                        UpdDisp(i - x, j);
                    }
                    if (y == 1 && j > 1 && GameState[i, j - y] != 0 && GameState[i,j] == 0)
                    {
                        GameState[i, j] = GameState[i, j - y];
                        UpdDisp(i, j);
                        GameState[i, j - y] = 0;
                        UpdDisp(i, j - y);
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (x == 1 && GameState[0, i] != 0 && GameState[1, i] == 0) // DOWN
                {
                    GameState[1, i] = GameState[0, i];
                    UpdDisp(1, i);
                    GameState[0, i] = 0;
                    UpdDisp(0, i);
                }
                if (y == 1 && GameState[i, 0] != 0 && GameState[i, 1] == 0) // RIGHT
                {
                    GameState[i, 1] = GameState[i, 0];
                    UpdDisp(i, 1);
                    GameState[i, 0] = 0;
                    UpdDisp(i, 0);
                }
            }

            Generate();
        }
    }
}
