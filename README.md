# ConsoleApp

###FrequencyDictionary Application Setup and Usage Guide
This readme file provides instructions for setting up and running FrequencyDictionary application which also included packages and configurations.

##System Setup
#1. Set the Startup Project
 - Open visual studio
 - Ensure .Net 8 is installed

#2. Add Command-Line Arguments
 - The application requires two command-line arguments required arguments during debugging:
 - The input file path (input.txt).
 - The output file path (output.txt).

##Steps i used to add Arguments:
 - I right-click on your project in 'Solution Explorer' and select 'Properties'.
 - I navigate to the 'Debug' tab.
 - In the 'Application Arguments' (or 'Command-Line Arguments') field, i entered:
input.txt output.txt

##Required Packages
dotnet add package System.Text.Encoding.CodePages

##Purpose:
 - I added below package dotnet add package System.Text.Encoding.CodePages
 - Reason is to enables handling of encodings Windows-1252 as speocfied in the requirement.
 
 ###Explanation of the Classes and escription of the Architecture

#IInputFileProcessor Interface:

The IInputFileProcessor interface defines a method, ProcessFileAsync(), which processes a file at speocific path and returns a dictionary.

#Implementation:

InputFileProcessor reads a file line by line and supports multiple encodings.

##3. IOutputFileWriter Interface: Defines the contract for writing word frequencies to a file in key value pair format.

#Implementation: OutputFileWriter:

 - The InputFileProcessor class implements IInputFileProcessor, providing an asynchronous method ProcessFileAsync to read a file, 
 - Extract words using regex
 - And return their frequencies in a sorted dictionary.

##4. Program

#Purpose:

Serve as entry point for the application.
Dependency injection (DI) to resolve IFrequencyProcessor and IOutputFileWriter implementations.
Reads file chunks.
Processes word frequencies.
Writes results to the output file.
Initializes dependency injection.
###Design Principles used

Separation of Concerns : Logic for read and write and seperated
##Future Expansion

Support for Additional Input and output format
Enhanced Processing
UI
