namespace Snake
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    public class Settings
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static int Speed { get; set; }
        public static int HighScore { get; set; }
        public static int Score { get; set; }
        public static int Points { get; set; }
        public static int Time { get; set; }
        public static bool GameOver { get; set; }
        public static Direction direction { get; set; }

        public Settings()
        {
            Width = 16;
            Height = 16;
            Speed = 16;
            HighScore = 0;
            Score = 0;
            Points = 100;
            Time = 0;
            GameOver = false;
            direction = Direction.Down;
        }

        public void setSpeed(int newSpeed)
        {
            Speed = newSpeed;
        }
    }
}
