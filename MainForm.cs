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
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Common;
using Newtonsoft.Json;

namespace WinPsychTest
{
    public partial class MainForm : Form
    {
        static string apiToken;
        private static List<string> files = new List<string>();

        static async Task<string> GetFilesList()
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

        static async Task<dynamic> GetJsonContent(String path)
        {
            using (var dbx = new DropboxClient(apiToken))
            {
                // Download file contents
                var response = await dbx.Files.DownloadAsync(path);

                // Read file contents as byte string
                var content = await response.GetContentAsByteArrayAsync();

                // Decode byte string into JSON object
                dynamic json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(content));

                return json;
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
            string baseName = "testResults.db";

            SQLiteConnection.CreateFile(baseName);

            SQLiteFactory factory = (SQLiteFactory) DbProviderFactories.GetFactory("System.Data.SQLite");

            using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + baseName;
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS [files] (
                    [id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [name] CHAR(100) NOT NULL,
                    [time] DATETIME,
                    [downloadPath] CHAR(100) NOT NULL
                    );";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

            apiToken = Form1.apiToken;

            Task<string> task = Task.Run(GetFilesList);
            task.Wait();

            richTextBox1.Text = task.GetAwaiter()
                                .GetResult();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task<string> task = Task.Run(GetFilesList);
            task.Wait();

            richTextBox1.Text = task.GetAwaiter()
                                .GetResult();
        }

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            string word = getWordAtIndex(richTextBox1, richTextBox1.SelectionStart);
            
            if (files.Contains(word))
            {
                Task<Object> task = Task.Run(() => GetJsonContent("/data/data/com.example.testapplication/cache/" + word));
                task.Wait();

                var jsonData = task.GetAwaiter().GetResult();

                var formPopup = new Form2();
                formPopup.UpdateTextFromJson(jsonData);
                formPopup.Show(this);
            }
        }
    }
}
