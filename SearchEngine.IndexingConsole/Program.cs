using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SearchEngine.Database;
using SearchEngine.IndexingConsole.Indexing;

namespace SearchEngine.IndexingConsole
{
  class Program
  {
    static void Main(string[] args)
    {

      //var serviceProvider = DependencyResolver.BuildDi();
      //var indexer = serviceProvider.GetService<IIndexer>();

      InitializeSettings();

      var timer = Stopwatch.StartNew();
      Console.WriteLine("\n\n--- Starting direct index creator ---\n\n");
      DirectIndexCreator.Start();


      Console.WriteLine("\n\n--- Starting the mapping process of reverse index collection from direct index collection  ---\n\n");
      ReverseIndexMapper.Start();


      Console.WriteLine("\n\n--- Starting the reduction of reverse index collection ---\n\n");
      ReverseIndexReducer.Start();
      timer.Stop();
      Console.WriteLine($"Indexing elapsed seconds: {timer.Elapsed.TotalMilliseconds / (float)1000}");
    }

    public static void InitializeSettings()
    {
      DbClient.ClearDatabase();

      var projectDir = Environment.CurrentDirectory.Contains("Debug\\netcoreapp") ?
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\")) : 
        Environment.CurrentDirectory;

      var siteDir = Path.Combine(projectDir, "riwdataset");
      RunSettings.DatasetPath = siteDir;


      RunSettings.LogType = LogType.Default;
      RunSettings.DocumentBatchSize = 5;
      RunSettings.GroupedWordsBatchSize = 300; //used for calculating idf
    }


  }
}
