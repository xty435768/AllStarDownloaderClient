using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllStarDownloader_client
{
    public class Common
    {
        public static byte[] bytes_image_adapter(Bitmap b)
        {
            if (b == null) return null;
            MemoryStream ms = new MemoryStream();
            b.Save(ms, ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();
            return bytes;
        }

        public static Bitmap bytes_image_adapter(byte[] b)
        {
            if (b == null) return null;
            using (MemoryStream ms = new MemoryStream(b))
            {
                Bitmap outputImg = new Bitmap(Image.FromStream(ms));
                return outputImg;
            }
        }

        public static byte[] GetBytes(string url, WebClient client)
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

        public class SynFileInfo
        {
            public int id { get; set; }
            public string file_name { get; set; }
            public string speed { get; set; } 
            public int progress_percent { get; set; }
            public string url { get; set; }
            public string path { get; set; }
            public int dgv_index { get; set; }
            public DateTime LastTime { get; set; }
            public SynFileInfo(int i,string f,string u,int di)
            {
                id = i;
                file_name = f;
                url = u;
                dgv_index = di;
            }
        }

        public class FileOperate
        {
            private const double KBCount = 1024;
            private const double MBCount = KBCount * 1024;
            private const double GBCount = MBCount * 1024;
            private const double TBCount = GBCount * 1024;
            public static string GetAutoSizeString(double size, int roundCount)
            {
                if (KBCount > size) return Math.Round(size, roundCount) + "B";
                else if (MBCount > size) return Math.Round(size / KBCount, roundCount) + "KB";
                else if (GBCount > size) return Math.Round(size / MBCount, roundCount) + "MB";
                else if (TBCount > size) return Math.Round(size / GBCount, roundCount) + "GB";
                else return Math.Round(size / TBCount, roundCount) + "TB";
            }

        }

    }
}
