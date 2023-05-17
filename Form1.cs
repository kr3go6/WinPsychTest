using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinPsychTest.Properties;

namespace WinPsychTest
{

    public partial class Form1 : Form
    {
        Thread newTh;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            newTh = new Thread(openMainForm);
            newTh.SetApartmentState(ApartmentState.STA);
            newTh.Start();
        }

        private void openMainForm()
        {
            Application.Run(new MainForm());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
