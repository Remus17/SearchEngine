using SearchEngine.ConsoleUI.EngineType;
using SearchEngine.Database;
using System;
using System.Diagnostics;
using System.Linq;

namespace SearchEngine.ConsoleUI
{
  class Program
  {
    static void Main(string[] args)
    {
      string userSearchTerms = null;
      var searchengine = new VectorialSearch();

      DbClient.Init();

      Console.WriteLine("Enter search terms: ");
      userSearchTerms = Console.ReadLine();

      while (!string.IsNullOrEmpty(userSearchTerms))
      {
        var stopWatch = Stopwatch.StartNew();
        var results = searchengine.Search(userSearchTerms);
        stopWatch.Stop();
        if(!results.Any())
        {
          Console.WriteLine("No results found");
        }
        else
        {
          foreach (var item in results)
          {
            Console.WriteLine($"{item.Similarity} - {item.Url}");
          }
          Console.WriteLine($"Found {results.Count()} results.");
          Console.WriteLine($"Search elapsed time {stopWatch.Elapsed.TotalMilliseconds} ms");
        }


        Console.WriteLine("Enter search terms: ");
        userSearchTerms = Console.ReadLine();
      }
    }
  }
}
