using MongoDB.Bson;
using MongoDB.Driver;
using SearchEngine.Database;
using SearchEngine.Database.Models;
using SearchEngine.IndexingConsole.Infrastructure;
using SearchEngine.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchEngine.IndexingConsole.Indexing.Workers
{
  public class Slave
  {

    public static Action<LinkToFileRelation> BuildDirectIndex = linkToFile =>
    {
      var porter = new PorterStemming();
      var wordFrequencies = WordUtils.GetWordFrequency(linkToFile.FilePath,porter);
      if (RunSettings.LogType == LogType.Debug)
      {
        Console.WriteLine($"Inserting into direct index collection one file with ({wordFrequencies.Count} words)");
      }
      var collection = DbClient.Database.GetCollection<DirectIndexDocument>(DbClient.DirectIndexCollectionName);
      var document = new DirectIndexDocument()
      {
        Url = linkToFile.Uri,
        WordFrequencies = CalculateTermFrequencies(wordFrequencies)
      };

      collection.InsertOne(document);
    };

    public static Action<IEnumerable<DirectIndexDocument>> BuildMappedReverseIndex = documents =>
    {
      var mappedWords =
            documents.SelectMany(doc => doc.WordFrequencies.Select(term => new MappedWordDocument()
            {
              Word = term.Word,
              Count = term.Count,
              TermFrequency = term.Frequency,
              Url = doc.Url
            }))
                    .OrderBy(x => x.Word).ToList();

      if (RunSettings.LogType == LogType.Debug)
      {
        Console.WriteLine($"Inserting into mapped reverse index collection ({mappedWords.Count} words)");
      }
      var collection = DbClient.Database.GetCollection<MappedWordDocument>(DbClient.MappedReverseIndexCollectionName);
      collection.InsertMany(mappedWords);
    };

    public static Action<IEnumerable<BsonDocument>> BuildReducedReverseIndex = words =>
    {
      var groupedWords = words.Select(word => word["_id"].ToString()).ToList();
      //get all word documents which contain the grouped words
      var wordDocs = GetMappedWords(groupedWords);

      //group mapped words documents into single word document
      var reducedWords = new List<ReducedWordDocument>();
      var wordsWithLocations = wordDocs.GroupBy(x => x.Word);
      foreach (var item in wordsWithLocations)
      {
        var groupCount = item.Count();
        var idf = (float)Math.Log(RunSettings.DocumentNumber / (float)groupCount);
        reducedWords.Add(new ReducedWordDocument
        {
          Word = item.Key,
          Locations = item.Select(word => new TermLocation
          {
            Count = word.Count,
            FinalWeight = idf * word.TermFrequency,
            Location=word.Url
          }).ToList(),
          InverseDocumentFrequency = idf
        });
      }    

      if (RunSettings.LogType == LogType.Debug)
      {
        Console.WriteLine($"Inserting into reduced reverse index collection ({reducedWords.Count} reduced words from {wordDocs.Count} mapped words)");
      }


      FlushToDatabase(reducedWords);

    };

    public static Action<IEnumerable<string>> ComputeDocumentsModule = documents =>
    {
      var modules = new List<ModuleDocument>();
      foreach (var doc in documents)
      {
        var documentWords = GetDocumentReducedWords(doc);
        var wordWeights = documentWords.Select(x => x.Locations.First().FinalWeight);
        var module = new ModuleDocument
        {
          Location = doc,
          Module = (float)Math.Sqrt(wordWeights.Select(x => x * x).Sum())
        };
        modules.Add(module);
        if(RunSettings.LogType == LogType.Debug)
        {
          Console.WriteLine($"Computed document module:{module.Module} for {doc}");
        }

      }
      FlushToDatabase(modules);
    };

    private static List<ReducedWordDocument> GetDocumentReducedWords(string doc)
    {
      var collection = DbClient.Database.GetCollection<ReducedWordDocument>(DbClient.ReducedReverseIndexCollectionName);
      var findOptions = Builders<ReducedWordDocument>.Filter.Eq("Locations.Location", doc);
      var projectionOptions = Builders<ReducedWordDocument>.Projection.Include(x => x.Locations[-1]).Include(x => x.Word);
      return collection.Find(findOptions).Project<ReducedWordDocument>(projectionOptions).ToList();
    }

    private static List<Term> CalculateTermFrequencies(Dictionary<string, int> wordFrequencies)
    {
      return wordFrequencies.Select(x => new Term()
      {
        Word = x.Key,
        Count = x.Value,
        Frequency = x.Value / (float)wordFrequencies.Count
      }).ToList();
    }

    private static List<MappedWordDocument> GetMappedWords(List<string> words)
    {
      var collection = DbClient.Database.GetCollection<MappedWordDocument>(DbClient.MappedReverseIndexCollectionName);
      return collection.Find(x => words.Contains(x.Word)).ToList();
    }
    private static void FlushToDatabase(List<ReducedWordDocument> words)
    {      
      var collection = DbClient.Database.GetCollection<ReducedWordDocument>(DbClient.ReducedReverseIndexCollectionName);
      collection.InsertMany(words);
    }
    private static void FlushToDatabase(List<ModuleDocument> modules)
    {
      var collection = DbClient.Database.GetCollection<ModuleDocument>(DbClient.DocumentModulesCollectionName);
      collection.InsertMany(modules);
    }
  }
}
