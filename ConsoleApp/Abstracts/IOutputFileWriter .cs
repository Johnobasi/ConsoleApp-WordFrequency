namespace ConsoleApp.Abstracts
{
    public interface IOutputFileWriter
    {
        Task WriteFrequenciesAsync(string path, Dictionary<string, int> frequenciesDictionary);
    }
}
