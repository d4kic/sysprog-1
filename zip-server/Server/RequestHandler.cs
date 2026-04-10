using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace zip_server.Server
{
    internal class RequestHandler
    {
        private static readonly string fileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Files");

        public static void HandleRequest(object obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj;
            Console.WriteLine(fileDir);

            try
            {
                string filespath = context.Request.Url.AbsolutePath.TrimStart('/');
                if (string.IsNullOrEmpty(filespath))
                {
                    SendText(context, "Ne postoji folder!");
                    return;
                }

                string[] reqFiles = filespath.Split('&');
                var found = new List<string>();
                foreach (string filename in reqFiles)
                {
                    string filepath = Path.Combine(fileDir, filename);
                    if (File.Exists(filepath))
                        found.Add(filepath);
                }
                if (found.Count == 0)
                {
                    SendText(context, "Ne postoje zahtevani fajlovi!");
                    return;
                }

                MemoryStream ms = new MemoryStream();
                ZipOutputStream zips = new ZipOutputStream(ms);
                foreach (string filename in found)
                {
                    byte[] data = File.ReadAllBytes(filename);
                    ZipEntry entry = new ZipEntry(Path.GetFileName(filename));
                    zips.PutNextEntry(entry);
                    zips.Write(data, 0, data.Length);
                    zips.CloseEntry();
                }
                zips.Close();
                byte[] zipData = ms.ToArray();
                SendZip(context, zipData);
            }
            catch (Exception ex)
            {
                SendText(context, "Error: " + ex.Message);
            }
        }

        static void SendZip(HttpListenerContext context, byte[] data)
        {
            context.Response.AddHeader("Content-Disposition", "attachment; filename=files.zip");
            context.Response.ContentLength64 = data.Length;
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.OutputStream.Close();
        }

        static void SendText(HttpListenerContext context, string text)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}
