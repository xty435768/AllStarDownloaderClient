using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AllStarDownloader_client
{
    class Common
    {
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

    }
}
