using ConsoleApp.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: FrequencyDictionary <inputFile> <outputFile>");
                    return;
                }

                string inputFile = args[0];
                string outputFile = args[1];

                #region Dependency Injection
                /// Set up the DI container
                var serviceProvider = DependencyInjection.ConfigureServices();

                // Resolve dependencies
                var inputReader = serviceProvider.GetRequiredService<IInputFileProcessor>();
                var outputWriter = serviceProvider.GetRequiredService<IOutputFileWriter>();

                #endregion
                Dictionary<string, int> frequencyDictionary = await inputReader.ProcessFileAsync(inputFile);
                await outputWriter.WriteFrequenciesAsync(outputFile, frequencyDictionary);

                Console.WriteLine($"Frequency dictionary created successfully. Output copy saved to output.txt...\n{JsonConvert.SerializeObject(frequencyDictionary, Newtonsoft.Json.Formatting.Indented)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
