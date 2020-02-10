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
    public partial class smoke_alert : Form
    {
        public smoke_alert()
        {
            InitializeComponent();
        }

        private void smoke_alert_Load(object sender, EventArgs e)
        {


            SoundPlayer simpleSound1 = new SoundPlayer(@"C:\voice alert smoke.wav");
            simpleSound1.Play();
            this.Text = "Smoke Alert";
            this.Opacity = 0.75;
        }

      

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();

        }
            }
}
