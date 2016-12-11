using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using JsonObject = System.Collections.Generic.Dictionary<string, object>;
using Demot.RandomOrgApi;
using System.IO;

namespace Snake
{
    public partial class GameForm : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        private int currentTime;

        public GameForm()
        {
            InitializeComponent();
            
            currentTime = 0;

            //Set settings to default
            new Settings();

            //testing speed
            int snakeSpeed = GamesSettingsForm.snakeSpeed;

            //Set game speed and start timer
            gameTimer.Interval = 1000 / snakeSpeed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            clockTimer.Interval = 1000; //Set for 1 second intervals
            clockTimer.Start(); //Start the timer

            //Start New game
            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;

            //Set settings to default
            new Settings();

            if (File.Exists("HighScore.txt"))
            {
                try {
                    StreamReader reader = new StreamReader("HighScore.txt");
                    int newHighScore = int.Parse(reader.ReadLine());
                    Settings.HighScore = newHighScore;
                    reader.Close();
                } catch (FileNotFoundException ex)
                {

                } catch (ArgumentNullException ex)
                {
                    Settings.HighScore = 0;
                }
                
            } else
            {
                try
                {
                    StreamWriter writer = File.CreateText("HighScore.txt");
                    writer.Close();
                } catch (Exception ex)
                {

                }
                
            }

            //Create new player object
            Snake.Clear();
            Circle head = new Circle {X = 10, Y = 5};
            Snake.Add(head);

            label2.Text = Settings.Time.ToString();

            lblScore.Text = Settings.Score.ToString();

            lblHighScore.Text = Settings.HighScore.ToString();
            GenerateFood();

            clockTimer.Interval = 1000;
            clockTimer.Start();

        }

        //Place random food object
        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle {X = random.Next(0, maxXPos), Y = random.Next(0, maxYPos)};
        }


        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check for Game Over
            if (Settings.GameOver)
            {
                //Check if Enter is pressed
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                MovePlayer();
            }

            pbCanvas.Invalidate();

        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            /*
            Color myRGBcolor = new Color();
            myRGBcolor = Color.FromArgb(0, 0, 255);
            SolidBrush brush = new SolidBrush( myRGBcolor );
            */
            if (!Settings.GameOver)
            {
                //Set color of snake
                SolidBrush brushColor;
                brushColor = GamesSettingsForm.brushRGBColor;

                //Draw snake
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColor;
                    if (i == 0)
                        snakeColor = Brushes.Black ;      //Draw head
                    else
                        snakeColor = brushColor;    //Rest of body

                    //Draw snake
                    canvas.FillEllipse(snakeColor,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));
                    

                    //Draw Food
                    canvas.FillEllipse(Brushes.Red,
                            new Rectangle(food.X * Settings.Width,
                             food.Y * Settings.Height, Settings.Width, Settings.Height));

                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: " + Settings.Score + "\nPress Enter to try again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
                clockTimer.Stop(); //Stop the clockTime
                if (Settings.Score > Settings.HighScore)
                    Settings.HighScore = Settings.Score;
                try
                {
                    StreamWriter writer = new StreamWriter("HighScore.txt");
                    writer.WriteLine(Settings.HighScore);
                    writer.Close();
                } catch (FileNotFoundException ex)
                {

                }
                
            }
        }


        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //Move head
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }


                    //Get maximum X and Y Pos
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

                    //Detect collission with game borders.
                    if (Snake[i].X < 0 || Snake[i].Y < 0
                        || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        Die();
                    }


                    //Detect collission with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                           Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    //Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }

                }
                else
                {
                    //Move body
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            //Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(circle);

            //Update Score
            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            Settings.Time += 1;
            label2.Text = Settings.Time.ToString();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GamesSettingsForm settingsForm = new GamesSettingsForm();
            settingsForm.Show();
            this.Hide();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("instrucitons",
                "Help Menu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
