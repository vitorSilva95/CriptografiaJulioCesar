using CriptografiaJulioCesar.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CriptografiaJulioCesar
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            HttpResponseMessage response = await client.GetAsync("https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token=bd1e3e4ab9cb3fa0cad7f35bb694466e267c6f5e");
            response.EnsureSuccessStatusCode();
            var payloadCriptografiaJulioCesar = JsonConvert.DeserializeObject<PayloadCriptografiaJulioCesar>(await response.Content.ReadAsStringAsync());

            payloadCriptografiaJulioCesar.Decifrar();

            payloadCriptografiaJulioCesar.ResumoCriptografico = CalculateSHA1(payloadCriptografiaJulioCesar.Decifrado);

            var textContent = JsonConvert.SerializeObject(payloadCriptografiaJulioCesar);
            var uploadResult = await UploadMultipart(textContent);

            Console.WriteLine(uploadResult);
        }

        public static string CalculateSHA1(string textContent)
        {
            try
            {

                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(textContent))).Replace("-", string.Empty).ToLower();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static async Task<string> UploadMultipart(string textContent)
        {
            string fileName = "answer.json";

            byte[] file = await SaveAndGetFile(textContent, fileName);

            string url = "https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=bd1e3e4ab9cb3fa0cad7f35bb694466e267c6f5e";

            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(file);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{4}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, fileName, "multipart/form-data", fileData, fileName.Split(".")[0]);

            var nfile = webClient.Encoding.GetBytes(package);

            try
            {
                return Encoding.ASCII.GetString(webClient.UploadData(url, "POST", nfile));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static async Task<byte[]> SaveAndGetFile(string textContent, string fileName)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                await outputFile.WriteAsync(textContent);
            }

            var file = await File.ReadAllBytesAsync(filePath);
            return file;
        }
    }
}
