using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using zip_server.src;
using static System.Net.Mime.MediaTypeNames;

namespace zip_server.src.Server
{
    internal class RequestHandler
    {
        private static readonly string fileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Files");

        public static void HandleRequest(object? obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj!;

            try
            {
                string? filespath = context.Request.Url?.AbsolutePath.TrimStart('/');
                Logger.Log("Primljen zahtev: " + filespath);
                if (string.IsNullOrEmpty(filespath))
                {
                    Logger.Log("Primljen zahtev bez parametara.");
                    SendText(context, "Nisu prosledjeni parametri zahtevu!");
                    return;
                }

                string[] reqFiles = filespath.Split('&', StringSplitOptions.RemoveEmptyEntries);
                var found = new List<string>();
                foreach (string filename in reqFiles)
                {
                    string safepath = Path.GetFileName(filename);
                    string filepath = Path.Combine(fileDir, safepath);
                    if (File.Exists(filepath))
                        found.Add(filepath);
                }
                if (found.Count == 0)
                {
                    Logger.Log("Zahtevani fajlovi ne postoje na serveru.");
                    SendText(context, "Ne postoje zahtevani fajlovi!");
                    return;
                }

                using MemoryStream ms = new MemoryStream();
                using (ZipOutputStream zips = new ZipOutputStream(ms))
                {
                    foreach (string filename in found)
                    {
                        byte[] data = File.ReadAllBytes(filename);
                        ZipEntry entry = new ZipEntry(Path.GetFileName(filename));
                        zips.PutNextEntry(entry);
                        zips.Write(data, 0, data.Length);
                        zips.CloseEntry();
                        Logger.Log("Zipovan fajl: " + filename);
                    }
                    zips.Close();
                }

                byte[] zipData = ms.ToArray();
                SendZip(context, zipData);
                Logger.Log("Zip fajl poslat.");
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message);
                SendText(context, "Error: " + ex.Message);
            }
            finally
            {
                context.Response.Close();
            }
        }

        static void SendZip(HttpListenerContext context, byte[] data)
        {
            context.Response.ContentType = "application/zip";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=files.zip");
            context.Response.ContentLength64 = data.Length;
            context.Response.OutputStream.Write(data, 0, data.Length);
        }

        static void SendText(HttpListenerContext context, string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
