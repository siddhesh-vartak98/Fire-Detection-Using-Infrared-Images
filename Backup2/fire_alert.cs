using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace cam_aforge1
{
    public partial class fire_alert : Form
    {
      
        public fire_alert()
        {
            InitializeComponent();
        }

        private void fire_alert_Load(object sender, EventArgs e)
        {
            SoundPlayer simpleSound = new SoundPlayer(@"C:\voice.wav");
            simpleSound.Play();
            this.Text = "Fire Alert";
            this.Opacity = 0.75;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        

        
    }
}
