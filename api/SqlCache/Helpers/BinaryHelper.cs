using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace SqlCache
{
    internal static class BinaryHelper
    {

        internal static string FromImageToString(string filePath)
        {
            while(true)
            {
                try
                {
                    if (!File.Exists(filePath)) return string.Empty;
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                        return Convert.ToBase64String(bin);
                    }
                }
                catch (IOException)
                {
                    return string.Empty;
                }
            }
        }

        internal static float GetFileSize(string filePath)
        {
            if (File.Exists(filePath))
            {
                var info = new FileInfo(filePath);
                return ((float)info.Length) / 1024;
            }
            return 0;
        }

        internal static string FromStreamToString(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(stream.Length));
                return Convert.ToBase64String(bin);
            }
        }

        internal static Stream FromStringToStream(string contents)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        internal static byte[] FromStringToBytes(string contents)
        {
            return Encoding.UTF8.GetBytes(contents);
        }

        internal static string FromStringToBase64(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(text);
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        internal static string FromBase64ToString(string sbase64)
        {
            if (string.IsNullOrEmpty(sbase64)) return sbase64;
            byte[] bytes = Convert.FromBase64String(sbase64);
            var encoding = new UTF8Encoding();
            return encoding.GetString(bytes, 0, bytes.Length);
        }


        internal static void SaveBase64StringToFile(string path, string content)
        {
            File.WriteAllBytes(path, Convert.FromBase64String(content));
        }

        internal static Image FromUriToImage(string filePath)
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(filePath);
                MemoryStream ms = new MemoryStream(bytes);
                if (bytes.Length > 505)
                {
                    Image img = Image.FromStream(ms);
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
