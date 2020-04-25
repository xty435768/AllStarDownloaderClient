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
        public preview_form()
        {
            InitializeComponent();
        }
        public static byte[] bytes_image_adapter(Bitmap b)
        {
            MemoryStream ms = new MemoryStream();
            b.Save(ms, ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();
            return bytes;
        }

        public static Bitmap bytes_image_adapter(byte[] b)
        {
            using (MemoryStream ms = new MemoryStream(b))
            {
                Bitmap outputImg = new Bitmap(Image.FromStream(ms));
                return outputImg;
            }
        }

        public static byte[] GetBytes(string url)
        {
            try
            {
                return client.DownloadData(url);
            }
            catch
            {
                Console.WriteLine("Ignore " + url.Substring(url.LastIndexOf("/") + 1));
            }
            return null;
        }

        private delegate void set_image_delegate(Bitmap b);
        private void preview_form_Load(object sender, EventArgs e)
        {
            save_button.Enabled = false;
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
            bitmap = bytes_image_adapter(GetBytes(url));
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
    }
}
