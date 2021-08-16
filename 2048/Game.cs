using System;
using System.IO;
using System.Windows.Forms;

namespace _2048
{
    public class GameInstance
    {
        public delegate void UpdDispDel(int x, int y, int value);
        public event UpdDispDel UpdDisp;

        public delegate void UpdSDel(int score);
        public event UpdSDel UpdScore;

        public delegate void UpdHSDel(int score);
        public event UpdHSDel UpdHS;

        public int Score;
        public int ScorePrev;

        public int[,] State = new int[4, 4];
        public int[,] StatePrev = new int[4, 4];

        public void Reset()
        {

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    State[i, j] = 0;
                    UpdDisp?.Invoke(i, j, State[i,j]);
                }
            }

            Movement.Score += Scoring;
            Scoring(0, true);
            UpdHS?.Invoke(HighScore);
            Generate();
        }
        
        private void Generate()
        {
            bool ok = false;
            for (int i = 0; i < 4 && !ok; i++)
            {
                for (int j = 0; j < 4 && !ok; j++)
                {
                    if (State[i, j] == 0)
                        ok = true;
                }
            }

            if (!ok)
            {
                MessageBox.Show("Game over! You ran out of free spaces.", "Game over", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Random ran = new();
            bool work = true;
            int x = -1;
            int y = -1;
            while (work)
            {
                x = ran.Next(0, 4);
                y = ran.Next(0, 4);
                work = State[x, y] != 0;
            }

            if (ran.Next(0, 10) == 0)
                State[x, y] = 4;
            else
                State[x, y] = 2;

            UpdDisp?.Invoke(x, y, State[x,y]);
        }

        public void Move(Keys key)
        {
            Backup();
            switch (key)
            {
                case Keys.NumPad0:
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            State[i, j] = (int)Math.Pow(2, t);
                            UpdDisp?.Invoke(i, j, State[i,j]);
                            t++;
                        }
                    }

                    State[0, 0] = 0;
                    UpdDisp?.Invoke(0, 0, 0);
                    break;
                case Keys.Left:
                    State = Movement.MvLu(0, 1, State);
                    Updates();
                    Generate();
                    break;
                case Keys.Up:
                    State = Movement.MvLu(1, 0, State);
                    Updates();
                    Generate();
                    break;
                case Keys.Down:
                    State = Movement.MvRd(1, 0, State);
                    Updates();
                    Generate();
                    break;
                case Keys.Right:
                    State = Movement.MvRd(0, 1, State);
                    Updates();
                    Generate();
                    break;
            }
        }

        private void Updates()
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (State[x, y] != StatePrev[x,y])
                        UpdDisp?.Invoke(x, y, State[x,y]);
                }
            }
        }

        private void Backup()
        {
            ScorePrev = Score;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    StatePrev[i, j] = State[i, j];
                }
            }
        }

        public void Restore()
        {
            Scoring(ScorePrev, true);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    State[i, j] = StatePrev[i, j];
                    UpdDisp?.Invoke(i, j, State[i,j]);
                }
            }
        }

        private void Scoring(int points, bool hard)
        {
            if (hard)
                Score = points;
            else
                Score += points;
            UpdScore?.Invoke(Score);

            if (Score > HighScore)
                HighScore = Score;
        }

        private static readonly string WritePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string DirPath = Path.Combine(WritePath, "zsotroav", "2048");
        private static readonly string MainPath = Path.Combine(DirPath, "HighScore.bin");

        public int HighScore
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
                UpdHS?.Invoke(value);
                using StreamWriter sw = new(MainPath);
                sw.WriteLine(value);
                sw.Close();
            }
        }
    }
}
