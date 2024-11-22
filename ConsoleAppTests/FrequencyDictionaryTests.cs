using ConsoleApp.Abstracts;
using ConsoleApp.Services;
using FluentAssertions;
using Moq;
using System.Text;

namespace ConsoleAppTests
{
    public class FrequencyDictionaryTests
    {
        private readonly Mock<IInputFileProcessor> _mockInputFileProcessor;
        private readonly Mock<IOutputFileWriter> _mockOutputFileWriter;
        private readonly InputFileProcessor _inputFileProcessor;
        private readonly OutputFileWriter _outputFileWriter;
        public FrequencyDictionaryTests()
        {
            _mockInputFileProcessor = new Mock<IInputFileProcessor>();
            _mockOutputFileWriter = new Mock<IOutputFileWriter>();
            _inputFileProcessor = new InputFileProcessor();
            _outputFileWriter = new OutputFileWriter();
        }

        /// <summary>
        /// This test shows how a file is processed when it contains valid words.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProcessFileAsync_ValidFile_ReturnsCorrectWordFrequencies()
        {
            // Arrange
            string testFilePath = "testfile.txt";
            string fileContent = "Hello world! Hello again.";
            var expectedFrequencies = new Dictionary<string, int>
            {
                { "hello", 2 },
                { "world", 1 },
                { "again", 1 }
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var fileStream = new FileStream(testFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream, Encoding.GetEncoding("Windows-1252")))
            {
                await writer.WriteAsync(fileContent);
            }

            try
            {
                // Act
                var result = await _inputFileProcessor.ProcessFileAsync(testFilePath);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(expectedFrequencies);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testFilePath))
                {
                    File.Delete(testFilePath);
                }
            }
        }

        /// <summary>
        /// This test show how file is processed when it contains no words.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProcessFileAsync_FileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            string nonExistentFilePath = "nonexistentfile.txt";
            var processor = new InputFileProcessor();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(
                () => processor.ProcessFileAsync(nonExistentFilePath));
            exception.Message.Should().Contain("Input file not found");
        }

        /// <summary>
        /// This test shows how a file is process with invalid chracters and the invalid words are ignored.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProcessFileAsync_InvalidCharacters_IgnoresInvalidWords()
        {
            // Arrange
            string testFilePath = "invalidcharsfile.txt";
            string fileContent = "Hello!!! 123 world... #$@! Hello";
            var expectedFrequencies = new Dictionary<string, int>
            {
                { "hello", 2 },
                { "world", 1 }
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var fileStream = new FileStream(testFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream, Encoding.GetEncoding("Windows-1252")))
            {
                await writer.WriteAsync(fileContent);
            }

            try
            {
                // Act
                var result = await _inputFileProcessor.ProcessFileAsync(testFilePath);

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(expectedFrequencies);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testFilePath))
                {
                    File.Delete(testFilePath);
                }
            }
        }

        /// <summary>
        /// Thus test demonstartes writing to a file when inputs are correct
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task WriteFrequenciesAsync_ValidInput_WritesToFileSuccessfully()
        {
            // Arrange
            string testFilePath = "output_test.txt";
            var testFrequencies = new Dictionary<string, int>
                {
                    { "hello", 2 },
                    { "world", 1 }
                };

            var expectedContent = "hello,2\nworld,1\n";

            try
            {
                // Act
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                await _outputFileWriter.WriteFrequenciesAsync(testFilePath, testFrequencies);

                // Assert
                File.Exists(testFilePath).Should().BeTrue("The output file was not created.");

                using (var fileStream = new FileStream(testFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(fileStream, Encoding.GetEncoding("Windows-1252")))
                {
                    var actualContent = await reader.ReadToEndAsync();

                    // Normalize line endings for comparison
                    var normalizedActualContent = actualContent.Replace("\r\n", "\n");
                    normalizedActualContent.Should().Be(expectedContent);
                }
            }
            finally
            {
                // Cleanup
                if (File.Exists(testFilePath))
                {
                    File.Delete(testFilePath);
                }
            }
        }

        /// <summary>
        /// This test demonstrates writing to a file that is read-only and throws exception as expected.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task WriteFrequenciesAsync_UnauthorizedAccess_ThrowsIOException()
        {
            // Arrange
            string restrictedFilePath = "restricted_output_test.txt";
            var testFrequencies = new Dictionary<string, int> { { "test", 1 } };

            try
            {
                // Create file and make it read-only
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                File.WriteAllText(restrictedFilePath, string.Empty);
                File.SetAttributes(restrictedFilePath, FileAttributes.ReadOnly);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<IOException>(() =>
                    _outputFileWriter.WriteFrequenciesAsync(restrictedFilePath, testFrequencies));

                exception.Message.Should().Contain("Cannot write to file");
            }
            finally
            {
                // Cleanup
                if (File.Exists(restrictedFilePath))
                {
                    File.SetAttributes(restrictedFilePath, FileAttributes.Normal); // Remove read-only attribute
                    File.Delete(restrictedFilePath);
                }
            }
        }

        /// <summary>
        /// This test demonstrates how to use Moq to mock the dependencies of the Main method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Main_ValidInputs_ProcessesSuccessfully()
        {
            //Arrange
            var wordFrequencies = new Dictionary<string, int>
            {
                { "hello", 2 },
                { "world", 1 }
            };

            _mockInputFileProcessor
                .Setup(processor => processor.ProcessFileAsync("input.txt"))
                .ReturnsAsync(wordFrequencies);

            // Act
            var frequencies = await _mockInputFileProcessor.Object.ProcessFileAsync("input.txt");
            await _mockOutputFileWriter.Object.WriteFrequenciesAsync("output.txt", frequencies);

            // Assert
            _mockInputFileProcessor.Verify(processor => processor.ProcessFileAsync("input.txt"), Times.Once);
            _mockOutputFileWriter.Verify(writer => writer.WriteFrequenciesAsync("output.txt", wordFrequencies), Times.Once);
        }
    }
}