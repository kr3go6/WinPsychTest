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
            richTextBox1.Text = "успех |  id |  цвет | радиус | центр круга | касание\n";

            for (int i = 0; i < ((JArray) jsonData.touch_data).Count; i++)
            {
                Console.WriteLine("{0}, {1}", i, ((JArray) jsonData.touch_data).Count);

                richTextBox1.Text += String.Format("{0,5} |{1,4} | {2,5} |{3,7} | ({4,4},{5,4}) | ({6,4},{7,4}) \n",
                    ((bool) jsonData.touch_data[i].is_successful) ? "✔" : "X",
                    jsonData.touch_data[i].id.ToString(),
                    jsonData.touch_data[i].color,
                    jsonData.touch_data[i].button_radius,
                    jsonData.touch_data[i].x_button_center,
                    jsonData.touch_data[i].y_button_center,
                    jsonData.touch_data[i].x,
                    jsonData.touch_data[i].y
                    );
            }
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
