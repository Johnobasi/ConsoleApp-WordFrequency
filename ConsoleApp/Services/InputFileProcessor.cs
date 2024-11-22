using ConsoleApp.Abstracts;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp.Services
{
    public class InputFileProcessor : IInputFileProcessor
    {
        public InputFileProcessor()
        {

        }

        //
        /// <summary>
        /// Process the input file and return a dictionary of word frequencies
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        public async Task<Dictionary<string, int>> ProcessFileAsync(string path)
        {
            var wordFrequencies = new Dictionary<string, int>();
            var wordPattern = @"\b\p{L}+\b";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Input file not found: {path}");
            }
            try
            {
                // Register the encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var streamReader = new StreamReader(path, Encoding.GetEncoding("Windows-1252"));
                while (!streamReader.EndOfStream)
                {
                    try
                    {
                        var line = await streamReader.ReadLineAsync();

                        var words = Regex.Matches(line!, wordPattern) // Extract words using regex
                            .Select(match => match.Value.Trim().ToLowerInvariant());

                        foreach (var word in words)
                        {
                            if (wordFrequencies.ContainsKey(word))
                                wordFrequencies[word]++;
                            else
                                wordFrequencies.Add(word, 1);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                return wordFrequencies
                    .OrderByDescending(kvp => kvp.Value)
                    .ThenBy(kvp => kvp.Key)
                    .ToDictionary();
            }
            catch (Exception ex)
            {
                throw new IOException($"Error reading file: {path}. Details: {ex.Message}");
            }
        }
    }
}
