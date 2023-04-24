using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinPsychTest
{
    public partial class MainForm : Form
    {
        static string apiToken;

        static async Task<string> Run()
        {
            using (var dbx = new DropboxClient(apiToken))
            {
                var folder = "/data/data/com.example.testapplication/cache";
                var list = await dbx.Files.ListFolderAsync(folder);

                string files = "";

                foreach (var item in list.Entries.Reverse().Where(i => i.IsFile))
                {
                    files += string.Format("{0,19}   {1}\r\n", item.AsFile.ClientModified.ToString(), item.Name);
                }

                return files;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            apiToken = Form1.apiToken;

            Task<string> task = Task.Run(Run);
            task.Wait();

            richTextBox1.Text = task.GetAwaiter()
                                .GetResult();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task<string> task = Task.Run(Run);
            task.Wait();

            richTextBox1.Text = task.GetAwaiter()
                                .GetResult();
        }
    }
}
