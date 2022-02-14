// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var process = new Process();
process.StartInfo.FileName = "https";
process.StartInfo.Arguments = "httpie.io/hello";
process.Start();

process.WaitForExit();