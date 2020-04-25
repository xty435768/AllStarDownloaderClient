using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllStarDownloader_client
{
    public partial class preview_form : Form
    {
        Bitmap bitmap;
        private static readonly WebClient client = new WebClient();
        public string url { get; set; }
        public string picture_name { get; set; }
        string previous_url;
        public string original_url { get; set; }
        public preview_form()
        {
            InitializeComponent();
        }

        private delegate void set_image_delegate(Bitmap b);
        private void preview_form_Load(object sender, EventArgs e)
        {
            save_button.Enabled = false;
            save_origin.Enabled = false;
            if(previous_url != url)
            {
                bitmap = null;
                pictureBox1.Image = null;
                Thread getimage_thread = new Thread(get_image);
                getimage_thread.IsBackground = true;
                getimage_thread.Start();
                previous_url = url;
            }
        }

        private void get_image()
        {
            bitmap = Common.bytes_image_adapter(Common.GetBytes(url,client));
            set_image(bitmap);
        }

        private void set_image(Bitmap b)
        {
            if (InvokeRequired)
            {
                set_image_delegate s = new set_image_delegate(set_image);
                Invoke(s, b);
            }
            else
            {
                pictureBox1.Image = b;
                save_button.Enabled = true;
                save_origin.Enabled = true;
                Refresh();
            }
        }

        private void preview_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Text = "Preview ";
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf1 = new SaveFileDialog() { Title = "Save preview picture", Filter = "JPEG|*.jpg|PNG|*.png", FileName = picture_name+" preview"};
            if(sf1.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(sf1.FileName, sf1.FileName.Substring(sf1.FileName.Length - 3) == "jpg" ? ImageFormat.Jpeg : ImageFormat.Png);
            }
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            Text = "Preview ";
            Close();
        }

        private void save_origin_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf1 = new SaveFileDialog() { Title = "Save original picture", Filter = "JPEG|*.jpg|PNG|*.png", FileName = picture_name + " original" };
            string path = "";
            if (sf1.ShowDialog() == DialogResult.OK)
            {
                path = sf1.FileName;
            }
            DownloadOriginalImage d = new DownloadOriginalImage(original_url, path);
            DialogResult result = d.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Abort)
            {
                d.Dispose();
                //if (result == DialogResult.Abort)
                //{
                //    File.Delete(path);
                //}
            }
        }
    }
}
