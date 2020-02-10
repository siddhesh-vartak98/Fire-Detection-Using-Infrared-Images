using cam_aforge1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cam_aforge1
{
    public partial class LOAD : Form
    {
        public LOAD()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 fs = new Form1(); 
            fs.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormIR ir = new FormIR();
            ir.Show();
            //Form2 wc = new Form2();
            //wc.Show();
        }
    }
}
