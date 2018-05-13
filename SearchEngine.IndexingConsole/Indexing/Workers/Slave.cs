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
      var reducedWords = wordDocs.GroupBy(x => x.Word, (key, g) => new ReducedWordDocument
      {
        Word = key,
        Locations = g.Select(word => new TermLocation
        {
          Count = word.Count,
          FinalWeight = ((float)Math.Log(RunSettings.DocumentNumber / (float)g.Count())) *  word.TermFrequency, 
          Location = word.Url
        }).ToList()
      }).ToList();

      if (RunSettings.LogType == LogType.Debug)
      {
        Console.WriteLine($"Inserting into reduced reverse index collection ({reducedWords.Count} reduced words from {wordDocs.Count} mapped words)");
      }


      FlushToDatabase(reducedWords);

    };



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
  }
}
