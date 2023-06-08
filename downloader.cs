using System;
using System.IO;

namespace Downloader
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var url = "http://localhost/File.zip";
            var nomeDoArquivo = "File.zip";
    
            await BaixarArquivo(url, nomeDoArquivo);
        }

        private static async Task BaixarArquivo(string url, string nomeDoArquivo)
        {
            const int bufferSize = 8192; //1MB de buffer
            using var client = new HttpClient();

            try
            {
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using var streamToReadFrom = await response.Content.ReadAsStreamAsync();
                await using var outputFileStream = File.Create(nomeDoArquivo, bufferSize);

                var buffer = new byte[bufferSize];
                long totalBytesRead = 0;
                int bytesRead;

                do
                {
                    bytesRead = await streamToReadFrom.ReadAsync(buffer.AsMemory(0, bufferSize));
                    await outputFileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                    totalBytesRead += bytesRead;
                    Console.WriteLine($"Baixou {totalBytesRead / 1024.0f / 1024.0f} MB");
                } while (bytesRead > 0);

                Console.WriteLine("Downloaded finalizado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro ao realizar o download: {ex.Message}");
            }
        }
    }
}