using MongoDB.Bson;
using MongoDB.Driver;
using SearchEngine.Database;
using SearchEngine.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.IndexingConsole.Indexing.Workers
{
  public class Master
  {


    public static IEnumerable<IEnumerable<DirectIndexDocument>> GetDirectIndexBatch()
    {
      var collection = DbClient.Database.GetCollection<DirectIndexDocument>(DbClient.DirectIndexCollectionName);
      var options = new FindOptions
      {
        BatchSize = RunSettings.DocumentBatchSize,
        NoCursorTimeout = false
      };
      using (var cursor = collection.Find(new FilterDefinitionBuilder<DirectIndexDocument>().Empty, options).ToCursor())
      {
        while (cursor.MoveNext())
        {

          yield return cursor.Current;
        }
      }
    }

    public static IEnumerable<IEnumerable<BsonDocument>> GetGroupedWordsBatch()
    {
      var collection = DbClient.Database.GetCollection<MappedWordDocument>(DbClient.MappedReverseIndexCollectionName);
      var grouped = new BsonDocument
      {
        { "_id",
          "$Word"
        }
      };
      var options = new AggregateOptions
      {
        BatchSize = RunSettings.GroupedWordsBatchSize
      };
      using (var cursor = collection.Aggregate(options).Group(grouped).ToCursor())
      {
        while (cursor.MoveNext())
        {

          yield return cursor.Current;
        }
      }

    }
  }
}
