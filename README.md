# CSVtoJSON

This project is a console application written in C#. It reads a CSV file, validates each row based on a defined set of rules, and outputs all valid rows as formatted JSON. Completing this challenge was a great learning experience and an opportunity to explore more of what C# has to offer. Thank you for the opportunity and I hope to continue in the interview process.

## Building the Project

The repository includes:

- Program.cs — full source code

- CSVtoJSON.csproj — project file

- sample_customers.csv — sample data used for testing

- Precompiled CSVtoJSON.exe

### Build Instructions

1. Install the .NET 9 SDK

2. Navigate to the project folder in a terminal

3. Build the application: `dotnet build -c Release`

4. Run the application: `dotnet run <input-file> <threads> <output-file>`

### Arguments

input-file — required, path to the CSV file

threads — optional, max thread count (defaults to 3)

output-file — optional, output filename (defaults to output.json *must provide all arguments*)

### Example:

`dotnet run test.csv 4 cleaned.json`

### Running the Precompiled Executable

A precompiled Windows executable is included: `.\CSVtoJSON.exe <input-file> <threads> <output-file>`

## Solution Design

The project uses built-in .NET libraries for:

- Regular expressions (System.Text.RegularExpressions)

- JSON serialization (System.Text.Json)

- Parallel processing (System.Collections.Concurrent)

### Structure

*Main*

input validation

file loading

selecting single-threaded or parallel validation

JSON generation and output
--- 
*ValidateRow*

A series of small helper methods, each responsible for validating one field

Follows the Single Responsibility Principle
---
*Row Processing*

Single-threaded: a simple loop (findValid)

Parallel version: Parallel.For with configurable thread count

Uses ConcurrentBag<CustomerRecord> for thread-safe accumulation of results

The logic for parsing valid rows is shared between both modes, ensuring consistent behavior regardless of thread count.