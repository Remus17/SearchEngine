using MongoDB.Driver;
using SearchEngine.Database;
using SearchEngine.Database.Models;
using SearchEngine.IndexingConsole.Indexing.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.IndexingConsole.Indexing
{
  public class ReverseIndexMapper
  {

    public static void Start()
    {
      var parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = RunSettings.MaxDegreeOfParallelism
      };

      Parallel.ForEach(Master.GetDirectIndexBatch(), parallelOptions, Slave.BuildMappedReverseIndex);

      Console.WriteLine($"Building a database index on {DbClient.MappedReverseIndexCollectionName} collection");
      CreateDbIndex();
    }

    private static void CreateDbIndex()
    {
      var collection = DbClient.Database.GetCollection<MappedWordDocument>(DbClient.MappedReverseIndexCollectionName);
      collection.Indexes.CreateOne(Builders<MappedWordDocument>.IndexKeys.Ascending(_ => _.Word));
    }

  }
}
