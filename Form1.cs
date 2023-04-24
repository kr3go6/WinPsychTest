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
        Uri authorizeUri;
        public static string apiToken = "";
        static readonly string appKey = "XXX";
        static readonly string appSecret = "XXX";

        static async Task<string> Login(string code)
        {
            using (var client = new DropboxClient(appKey, appSecret))
            {
                var token = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, appKey, appSecret);
                return token.AccessToken;
            }
        }

        public Form1()
        {
            InitializeComponent();

            authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, appKey, (Uri)null);

            Console.WriteLine(authorizeUri);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                return;
            }

            Task<string> task = Task.Run(() => (Task<string>)Login(textBox1.Text));
            task.Wait();

            apiToken = task.GetAwaiter()
                                .GetResult();

            Form form2 = new MainForm();
            this.Hide();
            form2.ShowDialog();
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(authorizeUri.ToString());
        }
    }
}
