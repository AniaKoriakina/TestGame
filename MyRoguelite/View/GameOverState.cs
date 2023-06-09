using System;
using System.Drawing;
using System.Windows.Forms;

public class GameOverState
{
    private bool isGameOver;
    private Form gameOverForm;

    public bool IsGameOver => isGameOver;

    public GameOverState()
    {
        gameOverForm = new Form();
        gameOverForm.Text = "Game Over";
        gameOverForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        gameOverForm.StartPosition = FormStartPosition.CenterScreen;

        Image backgroundImage = Image.FromFile("imageGameOver.png");
        PictureBox pictureBox = new PictureBox();
        pictureBox.Image = backgroundImage;
        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox.Dock = DockStyle.Fill;

        Button exitButton = new Button();
        exitButton.Text = "Вы - умер";
        exitButton.Size = new Size(200, 40);
        exitButton.Top = (gameOverForm.Height - exitButton.Height) / 2 + 50;
        exitButton.Location = new Point((gameOverForm.Width - exitButton.Width) / 2, (gameOverForm.Height - exitButton.Height) / 2);
        gameOverForm.FormBorderStyle = FormBorderStyle.None;
        exitButton.Click += (sender, e) =>
        {
            Environment.Exit(0);
        };

        gameOverForm.Controls.Add(exitButton);
        gameOverForm.Controls.Add(pictureBox);
    }

    public void ShowGameOverForm()
    {
        if (!gameOverForm.Visible)
        {
            isGameOver = true;
            gameOverForm.Visible = true;
        }
    }

    public void HideGameOverForm()
    {
        if (gameOverForm.Visible)
        {
            isGameOver = false;
            gameOverForm.Visible = false;
        }
    }
}
