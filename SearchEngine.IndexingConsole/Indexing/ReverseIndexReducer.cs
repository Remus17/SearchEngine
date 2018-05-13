using MongoDB.Driver;
using SearchEngine.Database;
using SearchEngine.Database.Models;
using SearchEngine.IndexingConsole.Indexing.Workers;
using SearchEngine.IndexingConsole.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.IndexingConsole.Indexing
{
  public class ReverseIndexReducer
  {
    public static int DocumentNumber { get; set; }

    public static void Start()
    {
      if(RunSettings.DocumentNumber==-1)
      {
        var files = FileTreeTools.GetFileLinkRelations(RunSettings.DatasetPath);
        RunSettings.DocumentNumber = files.Count;
      }

      var parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = RunSettings.MaxDegreeOfParallelism
      };

      Parallel.ForEach(Master.GetGroupedWordsBatch(), parallelOptions, Slave.BuildReducedReverseIndex);

      Console.WriteLine($"Building a database index on {DbClient.ReducedReverseIndexCollectionName} collection");
      CreateDbIndex();

    }
    private static void CreateDbIndex()
    {
      var collection = DbClient.Database.GetCollection<ReducedWordDocument>(DbClient.ReducedReverseIndexCollectionName);
      collection.Indexes.CreateOne(Builders<ReducedWordDocument>.IndexKeys.Ascending(_ => _.Word));
    }
  }
}
