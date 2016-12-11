using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using JsonObject = System.Collections.Generic.Dictionary<string, object>;
using Demot.RandomOrgApi;
using Demot.RandomOrgJsonRPC;

namespace Snake
{
    public partial class GamesSettingsForm : Form
    {
        String apiKey;

        public GamesSettingsForm()
        {
            apiKey = "051346c7-c218-49bf-a081-a7bb7fa95910";
            InitializeComponent();
        }

        //variables to pass to the game
        public static SolidBrush brushRGBColor;
        public static int snakeSpeed;

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            //setting up teh game
            snakeSpeed = chooseDifficulty();

            GameForm snakeGame = new GameForm();
            snakeGame.Show();
            this.Hide();
        }

        private void btnRandSnakeColor_Click(object sender, EventArgs e)
        {
            RandomOrgApiClient randOrgClient = new RandomOrgApiClient(apiKey);
            int randomNumberTest = Generate.GetRandom() % 255;
          
            Response randomOrgNums = randOrgClient.GenerateIntegers(3, 0, 255);
            txtR.Text = randomOrgNums.Integers[0].ToString();
            txtG.Text = randomOrgNums.Integers[1].ToString();
            txtB.Text = randomOrgNums.Integers[2].ToString();
        }

        private void btnPreviewSnakeColor_Click(object sender, EventArgs e)
        {
            if(validateInput() == false)
            {
                //do nothing, everything good
                return;
            }

            Color myRGBcolor = new Color();
            myRGBcolor = Color.FromArgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
            brushRGBColor = new SolidBrush(myRGBcolor);

            btnPreviewSnakeColor.BackColor = myRGBcolor;
        }

        private int chooseDifficulty()
        {
            int difficulty = 0;
            if (cboDifficulty.SelectedIndex == 0)
            {
                //easy
                difficulty = 10;
            }
            else if(cboDifficulty.SelectedIndex == 1)
            {
                //medium
                difficulty = 16;
            }
            else if (cboDifficulty.SelectedIndex == 2)
            {
                //hard
                difficulty = 25;
            }
            else if (cboDifficulty.SelectedIndex == 3)
            {
                //extreme
                difficulty = 35;
            }
            else
            {
                MessageBox.Show("ERROR", "ERROR!!!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            
            return difficulty;
        }

        private bool validateInput()
        {
            int valueR = 0;
            int valueG = 0;
            int valueB = 0;

            if (txtR.Text == "" || txtG.Text == "" || txtB.Text == "")
            {
                DisplayMessage("Enter your color values, fool!");
                txtR.Focus();
                return false;
            }
            if (int.TryParse(txtR.Text, out valueR) == false)
            {
                DisplayMessage("Use digits, you fool!");
                txtR.Focus();
                return false;
            }
            if (int.TryParse(txtG.Text, out valueG) == false)
            {
                DisplayMessage("Use digits, you fool!");
                txtG.Focus();
                return false;
            }
            if (int.TryParse(txtB.Text, out valueG) == false)
            {
                DisplayMessage("Use digits, you fool!");
                txtB.Focus();
                return false;
            }
            if (valueR > 255 || valueR < 0)
            {
                DisplayMessage("The digits need to be between 0 and 255, fool!");
                txtR.Focus();
                return false;

            }
            if (valueG > 255 || valueG < 0)
            {
                DisplayMessage("The digits need to be between 0 and 255, fool!");
                txtG.Focus();
                return false;
            }
            if (valueB > 255 || valueB < 0)
            {
                DisplayMessage("The digits need to be between 0 and 255, fool!");
                txtB.Focus();
                return false;
            }
            return true;
        }

        private void DisplayMessage(string message)
        {
            MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        /*************
        Createst instance of RandomOrgApiClient
        **************/

        public static class Generate //Used to generate Random Numbers
        {
            private static Random rnd = new Random();
            private static RandomOrgApiClient nacho = new RandomOrgApiClient("051346c7-c218-49bf-a081-a7bb7fa95910"); //Random.Org API code. Can only be used 1000x a day.

            public static int GetRandom()
            {
                try // first try the API. Since we are limited in # of uses, it will eventually stop working so you need to use a regular random number generator
                {
                    //Response r = nacho.GenerateIntegers(1, 0, 1000, true, false);
                    //return r.Integers[0];
                    return rnd.Next();
                }
                catch //use built in random number generator if there's no internet connection, or the API doesn't work.
                {
                    return rnd.Next();
                }
            }
            public static int GetRandom(int start, int finish)
            {
                try // first try the API. Since we are limited in # of uses, it will eventually stop working so you need to use a regular random number generator
                {
                    Response r = nacho.GenerateIntegers(1, start, finish, true, false);
                    return r.Integers[0];
                }
                catch //use built in random number generator if there's no internet connection, or the API doesn't work.
                {
                    return rnd.Next(start, finish);
                }
            }
        }

    }
}
