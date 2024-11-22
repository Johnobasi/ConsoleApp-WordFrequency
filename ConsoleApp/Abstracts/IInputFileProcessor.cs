namespace ConsoleApp.Abstracts
{
    public interface IInputFileProcessor
    {
        Task<Dictionary<string, int>> ProcessFileAsync(string path);
    }
}
