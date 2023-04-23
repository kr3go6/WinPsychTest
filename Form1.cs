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

namespace WinPsychTest
{

    public partial class Form1 : Form
    {
        int counter = 0;

        async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }
        }

        static async Task<string> Run()
        {
            using (var dbx = new DropboxClient("sl.Bc_KM_hoHJEdf_iOZzc2YYFe0N0kR21k6rd1q03GdbQAsKuJO6Ovdfg_U2hPxiY9mjNxCZ8h8rVkbwpFrSBttHboH524y0R4Mx45b2eap4NXKyluQpuHeTlsHVC10dXP9hh6sCU"))
            {
                var folder = "/data/data/com.example.testapplication/cache";
                var full = await dbx.Users.GetCurrentAccountAsync();
                MessageBox.Show(string.Format("{0} - {1}", full.Name.DisplayName, full.Email));

                var list = await dbx.Files.ListFolderAsync(folder);

                string files = "";

                // show folders then files
                foreach (var item in list.Entries.Where(i => i.IsFolder))
                {
                    files += string.Format("D  {0}/\r\n", item.Name);
                }

                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    files += string.Format("F{0,8} {1}\r\n", item.AsFile.Size, item.Name);
                }

                return files;
            }
        }

        public Form1()
        {
            InitializeComponent();

            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, "XXXXXXXXXXXXXXX", (Uri)null);

            Console.WriteLine(authorizeUri);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            counter++;
            textBox1.Text = counter.ToString();

            Task<string> task = Task.Run((Func<Task<string>>)Form1.Run);
            task.Wait();

            textBox2.Text = task.GetAwaiter()
                                .GetResult();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
