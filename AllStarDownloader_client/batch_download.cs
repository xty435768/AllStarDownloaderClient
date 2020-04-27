using DevComponents.DotNetBar.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.IO;

namespace AllStarDownloader_client
{
    public partial class batch_download : Form
    {
        public List<Common.SynFileInfo> fileList;
        public bool is_downloading = false;
        List<WebClient> lwc = new List<WebClient>();
        public void set_datagridview(DataTable d)
        {
            dgv.DataSource = d;
            
            dgv.Columns["url"].Visible = false;
            dgv.Columns["path"].Visible = false;
            if (!dgv.Columns.Contains("Progress"))
            { dgv.Columns.Add(new DataGridViewProgressBarXColumn() { Name = "Progress" }); dgv.Columns["Progress"].Width = 200;            }
            dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["Speed"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.Columns["IsAwaken"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["Speed"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["IsAwaken"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        public batch_download()
        {
            InitializeComponent();
            
            label1.Text = "";
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ColumnHeadersHeight = 30;
            dgv.RowHeadersWidth = 10;
            dgv.Font = new Font("微软雅黑", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 134);
        }

        private void batch_download_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                if(string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show("Empty path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                dialog.SelectedPath += "\\";
                textBox1.Text = dialog.SelectedPath;
                Refresh();
            }
        }

        [DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        bool isConnected()
        {
            int I = 0;
            bool state = InternetGetConnectedState(out I, 0);
            return state;
        }

        private void start_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = fileList.Count;
            if(isConnected())
            {
                if(textBox1.Text != "")
                {
                    start.Enabled = false;
                    Stop.Enabled = true;
                    is_downloading = true;
                    ThreadPool.SetMaxThreads(5, 5);
                    foreach (Common.SynFileInfo s in fileList)
                    {
                        lwc.Add(new WebClient());
                        startDownload(s,lwc[fileList.IndexOf(s)]);
                    }
                }
                else
                {
                    MessageBox.Show("Please choose save path first!", "Path error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No available Internet connection!", "Network error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void startDownload(Common.SynFileInfo s,WebClient client)
        {
            s.LastTime = DateTime.Now;
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri(s.url), textBox1.Text+s.file_name, s);
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled) return;
            Common.SynFileInfo s = (Common.SynFileInfo)e.UserState;
            fileList.Remove(s);
            progressBar1.Value++;
            label1.Text = (progressBar1.Value * 100 / progressBar1.Maximum).ToString() + "%";
            dgv.Rows[s.dgv_index].Cells["Speed"].Value = "Done!";
            dgv.Rows[s.dgv_index].Cells["Progress"].Value = 100;
            if (fileList.Count<=0)
            {
                is_downloading = false;
                start.Enabled = true;
                Stop.Enabled = false;
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!IsHandleCreated) return;
            Common.SynFileInfo s = (Common.SynFileInfo)e.UserState;
            s.progress_percent = e.ProgressPercentage;
            double secondCount = (DateTime.Now - s.LastTime).TotalSeconds;
            s.speed= Common.FileOperate.GetAutoSizeString(Convert.ToDouble(e.BytesReceived / secondCount), 2) + "/s";

            dgv.Rows[s.dgv_index].Cells["Progress"].Value = s.progress_percent;
            dgv.Rows[s.dgv_index].Cells["Speed"].Value = s.speed;
        }

        private void batch_download_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Downloading is not complete! Are you sure to exit?", "Exit confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (WebClient s in lwc)
                {
                    s.CancelAsync();
                    s.Dispose();
                }
                fileList = null;
                Dispose();
            }
            else return;
            Close();
        }

        //public static async Task<bool> RunCore(Common.SynFileInfo s, string path, HttpClient hc)
        //{
        //    var task = DownloadImage(s.url, hc);
        //    return await task.ContinueWith(t => {
        //        try
        //        {
        //            var data = task.Result;
        //            File.WriteAllBytes(path, data);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error: " + ex.Message, "Writing files failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return false;
        //        }
        //    });
        //}
        //public static async Task<byte[]> DownloadImage(string url, HttpClient client)
        //{
        //    return await client.GetByteArrayAsync(url);
        //}

    }
}
