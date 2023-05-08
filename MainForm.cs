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
using Newtonsoft.Json.Linq;

namespace WinPsychTest
{
    public partial class MainForm : Form
    {
        static string apiToken;
        private static List<string> files = new List<string>();
        private static List<string> prettyFiles = new List<string>();
        SQLiteFactory factory;
        string baseName = "testResults.db";

        void UpdateDB()
        {
            Task<string> task = Task.Run(GetFilesList);
            task.Wait();

            richTextBox1.Text = task.GetAwaiter()
                                .GetResult();

            List<KeyValuePair<string, String>> notAddedYet = new List<KeyValuePair<String, String>>();

            int i = 0;

            foreach (var item in prettyFiles)
            {
                using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
                {
                    connection.ConnectionString = "Data Source = " + baseName;
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT id FROM files WHERE name == \"" + item + "\";";

                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                notAddedYet.Add(new KeyValuePair<string, string>(item, files[i]));
                            }

                            i += 1;
                        }
                    }
                }
            }

            using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + baseName;
                connection.Open();

                foreach (var item in notAddedYet)
                {
                    using (var command = new SQLiteCommand("INSERT INTO files (name, url_path) VALUES (@value1, @value2)", connection))
                    {
                        command.Parameters.AddWithValue("@value1", item.Key);
                        command.Parameters.AddWithValue("@value2", "/data/data/com.example.testapplication/cache/" + item.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        static async Task<string> GetFilesList()
        {
            using (var dbx = new DropboxClient(apiToken))
            {
                files.Clear();
                prettyFiles.Clear();
                var folder = "/data/data/com.example.testapplication/cache";
                var list = await dbx.Files.ListFolderAsync(folder);

                string files_str = "";

                foreach (var item in list.Entries.Reverse().Where(i => i.IsFile))
                {
                    if (item.Name.LastIndexOf("_") != -1)
                    {
                        files_str += string.Format("{0,19}      {1}\r\n", item.AsFile.ClientModified.ToString(),
                            item.Name.Substring(0, item.Name.LastIndexOf("_")));
                        files.Add(item.Name);
                        prettyFiles.Add(String.Format("{0,19}      {1}", item.AsFile.ClientModified.ToString(), 
                            item.Name.Substring(0, item.Name.LastIndexOf("_"))));
                    }
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

        static string PrettyPrinter(dynamic jsonData)
        {
            String res = "успех |  id |  цвет | радиус | центр круга | касание\n";

            for (int i = 0; i < ((JArray)jsonData.touch_data).Count; i++)
            {
                res += String.Format("{0,5} |{1,4} | {2,5} |{3,7} | ({4,4},{5,4}) | ({6,4},{7,4}) \n",
                    ((bool)jsonData.touch_data[i].is_successful) ? "✔" : "X",
                    jsonData.touch_data[i].id.ToString(),
                    jsonData.touch_data[i].color,
                    jsonData.touch_data[i].button_radius,
                    jsonData.touch_data[i].x_button_center,
                    jsonData.touch_data[i].y_button_center,
                    jsonData.touch_data[i].x,
                    jsonData.touch_data[i].y
                    );
            }

            return res;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        string getWordAtIndex(RichTextBox RTB, int index)
        {
            string wordSeparators = "\r\n";
            int c = index;
            int cp0 = index;
            int cp2 = RTB.Find(wordSeparators.ToCharArray(), index);

            for (; c > 0; c--)
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

            if (c == 0) cp0 = c;

            int l = cp2 - cp0;
            if (l > 0) return RTB.Text.Substring(cp0, l); else return "";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SQLiteConnection.CreateFile(baseName);

            factory = (SQLiteFactory) DbProviderFactories.GetFactory("System.Data.SQLite");

            using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + baseName;
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS [files] (
                    [id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [name] CHAR(100) NOT NULL,
                    [url_path] CHAR(100) NOT NULL
                    );";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

            apiToken = Form1.apiToken;

            UpdateDB();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateDB();
        }

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            string word = getWordAtIndex(richTextBox1, richTextBox1.SelectionStart);

            Console.WriteLine(word);
            
            if (prettyFiles.Contains(word))
            {
                string url;

                using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
                {
                    connection.ConnectionString = "Data Source = " + baseName;
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT url_path FROM files WHERE name == \"" + word + "\";";

                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();

                            url = reader.GetString(0);
                        }
                    }
                }

                var formPopup = new Form2();
                formPopup.Show(this);

                Task<Object> task = Task.Run(() => GetJsonContent(url));
                task.Wait();
                var jsonData = task.GetAwaiter().GetResult();

                Task<String> prettyTask = Task.Run(() => PrettyPrinter(jsonData));

                formPopup.UpdateTextFromJson(jsonData);

                prettyTask.Wait();
                formPopup.UpdateTextFromJson(prettyTask.GetAwaiter().GetResult());
            }
        }
    }
}
