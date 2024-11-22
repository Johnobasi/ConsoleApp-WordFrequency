using ConsoleApp.Abstracts;
using System.Text;

namespace ConsoleApp.Services
{
    /// <summary>
    /// This class writes word frequencies to an output file in a key value pair format
    /// </summary>
    public class OutputFileWriter : IOutputFileWriter
    {
        public async Task WriteFrequenciesAsync(string path, Dictionary<string, int> frequenciesDictionary)
        {

            try
            {
                using var writer = new StreamWriter(path, false, Encoding.GetEncoding("Windows-1252"));
                foreach (var kvp in frequenciesDictionary)
                {
                    await writer.WriteLineAsync($"{kvp.Key},{kvp.Value}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new IOException($"Cannot write to file: {path}. Check user's file permissions.");
            }
        }
    }
}
