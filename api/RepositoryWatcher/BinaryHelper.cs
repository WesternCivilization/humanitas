using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepositoryWatcher
{
    static public class BinaryHelper
    {

        static public string FromImageToString(string filePath)
        {
            var attempts = 0;
            while(true)
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                        return Convert.ToBase64String(bin);
                    }
                }
                catch (IOException)
                {
                    attempts++;
                    if (attempts <= 5) Thread.Sleep(1000);
                    else throw;
                }
            }
        }

        static public float GetFileSize(string filePath)
        {
            if (File.Exists(filePath))
            {
                var info = new FileInfo(filePath);
                return ((float)info.Length) / 1024;
            }
            return 0;
        }

        static public string FromStreamToString(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(stream.Length));
                return Convert.ToBase64String(bin);
            }
        }

        static public Stream FromStringToStream(string contents)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        static public byte[] FromStringToBytes(string contents)
        {
            return Encoding.UTF8.GetBytes(contents);
        }

        static public string FromStringToBase64(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(text);
            return System.Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        static public string FromBase64ToString(string sbase64)
        {
            if (string.IsNullOrEmpty(sbase64)) return sbase64;
            byte[] bytes = System.Convert.FromBase64String(sbase64);
            var encoding = new System.Text.UTF8Encoding();
            return encoding.GetString(bytes, 0, bytes.Length);
        }


        public static void SaveBase64StringToFile(string path, string content)
        {
            File.WriteAllBytes(path, Convert.FromBase64String(content));
        }

        public static System.Drawing.Image FromUriToImage(string filePath)
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(filePath);
                MemoryStream ms = new MemoryStream(bytes);
                if (bytes.Length > 505)
                {
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    return img;
                }
                else
                {
                    return null;
                }
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

    }
}
