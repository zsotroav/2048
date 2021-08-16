using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _2048
{
    public partial class Form1 : Form
    {
        public GameInstance Game = new();

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

        public readonly Dictionary<Keys, GameControls> ControlsMap = new()
        {
            {Keys.Up, GameControls.Up},
            {Keys.Down, GameControls.Down},
            {Keys.Left, GameControls.Left},
            {Keys.Right, GameControls.Right},
            {Keys.NumPad0, GameControls.Debug}
        };

        public readonly Color ColorPlus = Color.FromArgb(89, 137, 247);

        public Panel[,] GameBoard;
        public Label[,] GameLabels;

        public Form1()
        {
            InitializeComponent();

            Game.UpdDisp += UpdDisp;
            Game.UpdScore += Score;
            Game.UpdHS += HScore;
            Game.GenErr += GenErr;

            Init();
        }
        
        private void Init()
        {
            GameBoard = new[,]
            {
                { PA1, PB1, PC1, PD1},
                { PA2, PB2, PC2, PD2},
                { PA3, PB3, PC3, PD3},
                { PA4, PB4, PC4, PD4}
            };
            GameLabels = new[,]
            {
                { LA1, LB1, LC1, LD1},
                { LA2, LB2, LC2, LD2},
                { LA3, LB3, LC3, LD3},
                { LA4, LB4, LC4, LD4}
            };
            
            Game.Reset();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Score(int score)
        {
            LScore.Text = $@"Score: {score}";
        }

        private void HScore(int score)
        {
            LHScore.Text = $@"High score: {score}";
        }

        private void UpdDisp(int x, int y, int value)
        {
            Color col;
            col = Colors.TryGetValue(value, out col) ? col : ColorPlus;
            GameBoard[x, y].BackColor = col;
            
            if (value == 0)
            {
                GameLabels[x, y].Text = "";
            }
            else
            {
                GameLabels[x, y].Text = value.ToString();

                if (GameLabels[x, y].Text.Length > 4)
                    GameLabels[x, y].Font = new Font("Microsoft Sans Serif", 9F,
                        FontStyle.Bold, GraphicsUnit.Point);
                else
                    GameLabels[x, y].Font = new Font("Microsoft Sans Serif", 12F,
                        FontStyle.Bold, GraphicsUnit.Point);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            GameControls cont;
            cont = ControlsMap.TryGetValue(e.KeyData, out cont) ? cont : GameControls.Invalid;
            Game.Move(cont);
        }

        private void Undo(object sender, EventArgs e)
        {
            Game.Restore();
        }

        private void NewGame(object sender, EventArgs e)
        {
            Init();
        }

        private static void GenErr()
        {
            MessageBox.Show(@"Game over! You ran out of free spaces.", @"Game over", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
