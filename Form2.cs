using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinPsychTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public void UpdateTextFromJson(dynamic jsonData)
        {
            textBox1.Text = jsonData.name.ToString();
            textBox2.Text = jsonData.screen_height_px.ToString() + "x" + jsonData.screen_width_px.ToString();
        }

        public void UpdateTextFromJson(String table)
        {
            richTextBox1.Text = table;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
