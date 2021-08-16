using System;
using System.IO;

namespace _2048
{
    public class GameInstance
    {
        // Update Display ~ Render a cell with a given value
        public delegate void UpdDispDel(int x, int y, int value);
        public event UpdDispDel UpdDisp;

        // Update Score ~ Update the displayed score
        public delegate void UpdSDel(int score);
        public event UpdSDel UpdScore;

        // Update High Score
        public delegate void UpdHSDel(int score);
        public event UpdHSDel UpdHS;

        // Generate Error ~ Show an error message when generating a new cell fails
        public delegate void GenErrDel();
        public event GenErrDel GenErr;

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

            // Subscribe to scoring from the movement team
            Movement.Score += Scoring;

            // Hard reset current score and get high score
            Scoring(0, true);
            UpdHS?.Invoke(HighScore);

            // Generate start cell
            Generate();
        }
        
        private void Generate()
        {
            // Check if we still have open cells
            bool ok = false;
            for (int i = 0; i < 4 && !ok; i++)
            {
                for (int j = 0; j < 4 && !ok; j++)
                {
                    if (State[i, j] == 0)
                        ok = true;
                }
            }

            // Invoke error display from renderer
            if (!ok)
            {
                GenErr?.Invoke();
                return;
            }

            // Get a random empty cell
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

            // 10% chance of a 4 cell generating
            if (ran.Next(0, 10) == 0)
                State[x, y] = 4;
            else
                State[x, y] = 2;

            // Invoke render update on cell
            UpdDisp?.Invoke(x, y, State[x,y]);
        }

        public void Move(GameControls key)
        {
            Backup();
            switch (key)
            {
                // Debug displays all possible cells
                case GameControls.Debug:
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
                case GameControls.Left:
                    State = Movement.MvLu(0, 1, State);
                    Updates();
                    Generate();
                    break;
                case GameControls.Up:
                    State = Movement.MvLu(1, 0, State);
                    Updates();
                    Generate();
                    break;
                case GameControls.Down:
                    State = Movement.MvRd(1, 0, State);
                    Updates();
                    Generate();
                    break;
                case GameControls.Right:
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
                    // Update cells that don't match the previous state
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
                    // Save every cell
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
                    // Reload previous state
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

        // %AppData%/zsotroav/2048/HighScore.bin
        private static readonly string WritePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string DirPath = Path.Combine(WritePath, "zsotroav", "2048");
        private static readonly string MainPath = Path.Combine(DirPath, "HighScore.bin");

        public int HighScore
        {
            get
            {
                if (File.Exists(MainPath))
                {
                    // Read score from file
                    using StreamReader sr = new(MainPath);
                    int re = int.Parse(sr.ReadLine() ?? "0");
                    sr.Close();
                    return re;
                }
                else
                {
                    // Create dir and file if not found
                    Directory.CreateDirectory(DirPath);
                    File.Create(MainPath).Close();
                    return 0;
                }
            }
            set
            {
                // Update HS Display on UI
                UpdHS?.Invoke(value);

                //Write to HS file
                using StreamWriter sw = new(MainPath);
                sw.WriteLine(value);
                sw.Close();
            }
        }
    }
}
