using MongoDB.Driver;
using System;
using System.Threading;

namespace SearchEngine.Database
{
  public class DbClient
  {
    private static MongoClient _client;
    private static IMongoDatabase _database;
    public static MongoClient Client
    {
      get
      {
        if (_client == null)
        {
          try
          {
            _client = new MongoClient("mongodb+srv://remus:remus123$%^@cluster0-tgjbo.mongodb.net/test");
            Console.WriteLine("Db connection successfully initialized");
          }
          catch (DnsClient.DnsResponseException e)
          {
            Console.WriteLine($"Exception caught when trying to connect to MongoDB. {e.Message}");
            Console.WriteLine("Retrying...");
            Thread.Sleep(1000);
            var retry = Client;
          }
        }
        return _client;
      }
    }
    public static IMongoDatabase Database
    {
      get
      {
        if (_database == null)
        {
          _database = Client.GetDatabase("MapReduce");
        }
        return _database;
      }
    }
    public static string DirectIndexCollectionName { get; } = "direct_index";
    public static string MappedReverseIndexCollectionName { get; } = "mapped_reverse_index";
    public static string ReducedReverseIndexCollectionName { get; } = "reduced_reverse_index";
    public static string DocumentModulesCollectionName { get; } = "document_modules";

    public static void ClearDatabase()
    {
      Console.WriteLine("Cleaning up the db collections");
      Database.DropCollection(DirectIndexCollectionName);
      Database.CreateCollection(DirectIndexCollectionName);


      Database.DropCollection(MappedReverseIndexCollectionName);
      Database.CreateCollection(MappedReverseIndexCollectionName);

      Database.DropCollection(ReducedReverseIndexCollectionName);
      Database.CreateCollection(ReducedReverseIndexCollectionName);

      Database.DropCollection(DocumentModulesCollectionName);
      Database.CreateCollection(DocumentModulesCollectionName);

    }
    public static void Init()
    {
      var clientInit = Client;
      var dbInit = Database;
    }



  }
}
