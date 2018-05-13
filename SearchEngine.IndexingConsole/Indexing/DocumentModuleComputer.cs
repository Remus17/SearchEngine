using MongoDB.Driver;
using SearchEngine.Database;
using SearchEngine.Database.Models;
using SearchEngine.IndexingConsole.Indexing.Workers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.IndexingConsole.Indexing
{
  public class DocumentModuleComputer
  {
    public static void Start()
    {
      var locations = RunSettings.FilePaths.Select(x => x.Uri);

      var parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = RunSettings.MaxDegreeOfParallelism
      };
      //Partitioner.Create()
      Parallel.ForEach(locations, parallelOptions, Slave.ComputeDocumentsModule);
      //Parallel.ForEach(Master.Batch(locations, RunSettings.DocumentBatchSize), parallelOptions, Slave.ComputeDocumentsModule);
      Console.WriteLine("Creating an index on document modules collection");
      CreateDbIndex();
    }

    private static void CreateDbIndex()
    {
      var collection = DbClient.Database.GetCollection<ModuleDocument>(DbClient.DocumentModulesCollectionName);
      collection.Indexes.CreateOne(Builders<ModuleDocument>.IndexKeys.Ascending(_ => _.Location));
    }
  }
}
