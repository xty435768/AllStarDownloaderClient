using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllStarDownloader_client
{
    public partial class DownloadOriginalImage : Form
    {
        string url;
        string path;
        WebClient client = new WebClient();
        public bool done = false;
        public DownloadOriginalImage(string u,string p)
        {
            InitializeComponent();
            url = u;
            path = p;
        }

        private bool cancel_confirm()
        {
            if (progressBar1.Value == progressBar1.Maximum)
            {
                DialogResult = DialogResult.OK;
                return true;
            }
            if(MessageBox.Show("Cancel download process?","Cancel",MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                client.CancelAsync();
                DialogResult = DialogResult.Abort;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DownloadOriginalImage_Load(object sender, EventArgs e)
        {
            DownloadFile(url, path, ProgressBar_Value, null);
        }

        private void ProgressBar_Value(int obj)
        {
            if (DialogResult == DialogResult.OK || DialogResult == DialogResult.Abort) return;
            progressBar1.Value = obj;
            label1.Text = obj.ToString() + "%";
            if(obj == 100)
            {
                DialogResult = DialogResult.OK;
                MessageBox.Show("Done!");
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DownloadOriginalImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK || DialogResult == DialogResult.Abort) { return; }
            if (!cancel_confirm()) e.Cancel = true;
        }

        private void DownloadFile(string url, string savefile, Action<int> downloadProgressChanged, Action downloadFileCompleted)
        {
            if (downloadFileCompleted != null)
            {
                client.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                {
                    if (e.Cancelled) { client.Dispose();  return; }
                    Invoke(downloadFileCompleted);
                };
            }
            
            if (downloadProgressChanged != null)
            {
                client.DownloadProgressChanged += delegate (object sender, System.Net.DownloadProgressChangedEventArgs e)
                {
                    if (IsHandleCreated)
                        Invoke(downloadProgressChanged, e.ProgressPercentage);
                };
            }
            client.DownloadFileAsync(new Uri(url), savefile);
        }
        delegate void Action(); 
    }
}
