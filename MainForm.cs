using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinPsychTest
{
    public partial class MainForm : Form
    {
        static string apiToken;
        private static List<string> files = new List<string>();

        static async Task<string> Run()
        {
            using (var dbx = new DropboxClient(apiToken))
            {
                files.Clear();
                var folder = "/data/data/com.example.testapplication/cache";
                var list = await dbx.Files.ListFolderAsync(folder);

                string files_str = "";

                foreach (var item in list.Entries.Reverse().Where(i => i.IsFile))
                {
                    files_str += string.Format("{0,19}   {1}\r\n", item.AsFile.ClientModified.ToString(), item.Name);
                    files.Add(item.Name);
                }

                return files_str;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        string getWordAtIndex(RichTextBox RTB, int index)
        {
            string wordSeparators = " \r\n";
            int cp0 = index;
            int cp2 = RTB.Find(wordSeparators.ToCharArray(), index);

            for (int c = index; c > 0; c--)
            { 
                try
                {
                    if (wordSeparators.Contains(RTB.Text[c]))
                    {
                        cp0 = c + 1; 
                        break; 
                    } 
                } catch
                {
                    return "";
                }
            }

            int l = cp2 - cp0;
            if (l > 0) return RTB.Text.Substring(cp0, l); else return "";
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

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            string word = getWordAtIndex(richTextBox1, richTextBox1.SelectionStart);
            if (files.Contains(word)) MessageBox.Show(word);
        }
    }
}
