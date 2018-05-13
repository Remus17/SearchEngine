using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using SearchEngine.Database;
using SearchEngine.Database.Models;
using SearchEngine.Miscellaneous.Classes;
using SearchEngine.Tools;

namespace SearchEngine.Miscellaneous
{
  public class VectorialSearch
  {
    public PorterStemming Porter { get; set; }
    public VectorialSearch()
    {
      Porter = new PorterStemming();
    }
    public IEnumerable<VectorialResult> Search(string userSearchTerm)
    {
      var searchTerms = userSearchTerm.Split(' ').Select(x => Porter.Stem(x)).ToList();
      var exclusionStartIndex = searchTerms.IndexOf("not");
      var excludedTerms = new List<string>();
      if (exclusionStartIndex > 0 && searchTerms.Count > exclusionStartIndex + 1)
      {
        excludedTerms = searchTerms.GetRange(exclusionStartIndex + 1, searchTerms.Count - (exclusionStartIndex + 1));
        //searchTerms = searchTerms.GetRange(0, exclusionStartIndex);
      }
      var storedWords = GetWordDocuments(searchTerms);
      if (excludedTerms.Any())
      {
        var excludedWords = storedWords.Where(x => excludedTerms.Contains(x.Word)).ToList(); //GetWordDocuments(excludedTerms);
        storedWords = storedWords.Where(x => !excludedTerms.Contains(x.Word)).ToList();
        var excludedDocuments = excludedWords.SelectMany(x => x.Locations).Select(x => x.Location).ToList();
        foreach (var item in storedWords)
        {
          item.Locations = item.Locations.Where(x => !excludedDocuments.Contains(x.Location)).ToList();
        }
      }

      var documents = new Dictionary<string, Dictionary<string, float>>();
      foreach (var item in storedWords)
      {
        AddToDictionary(documents, item.Locations.Select(x =>
          new KeyValuePair<string, KeyValuePair<string, float>>(x.Location,
            new KeyValuePair<string, float>(item.Word, x.FinalWeight))));
      }


      var vectorialTerms = GetVectorialSearchTerms(searchTerms, storedWords);

      return GetResults(vectorialTerms, documents, ComputeVectorModule(vectorialTerms.Select(x => x.Weight))).OrderByDescending(x => x.Similarity);
    }

    private List<VectorialResult> GetResults(List<VectorialTerm> searched, Dictionary<string, Dictionary<string, float>> storedWords, float queryModule)
    {
      var result = new List<VectorialResult>();


      foreach (var documentKey in storedWords.Keys)
      {
        result.Add(new VectorialResult()
        {
          Url = documentKey,
          Similarity = ComputeDocumentCosinus(searched, storedWords[documentKey], queryModule)
        });
      }
      return result;
    }

    private float ComputeDocumentCosinus(List<VectorialTerm> searchedWords, Dictionary<string, float> storedWords, float queryModule)
    {
      float numerator = 0;
      foreach (var item in searchedWords)
      {
        float dbWordWeight = 0;
        storedWords.TryGetValue(item.Word, out dbWordWeight);
        numerator += item.Weight * dbWordWeight;
      }
      return numerator / (queryModule * ComputeVectorModule(storedWords.Values));
    }
    //dictionary with location as key and list of word data as value
    private void AddToDictionary(Dictionary<string, Dictionary<string, float>> dictionary, IEnumerable<KeyValuePair<string, KeyValuePair<string, float>>> items)
    {
      foreach (var item in items)
      {
        if (dictionary.ContainsKey(item.Key))
        {
          dictionary[item.Key].Add(item.Value.Key, item.Value.Value);
        }
        else
        {
          dictionary.Add(item.Key, new Dictionary<string, float>() { { item.Value.Key, item.Value.Value } });
        }
      }
    }
    private List<VectorialTerm> GetVectorialSearchTerms(List<string> words, List<ReducedWordDocument> storedWords)
    {
      var totalFilesNumber = storedWords.SelectMany(x => x.Locations).Count();
      return words.Select(x => new VectorialTerm()
      {
        Word = x,
        Weight = (1 / (float)words.Count) * (float)Math.Log((totalFilesNumber / (float?)storedWords.FirstOrDefault(sw => sw.Word == x)?.Locations.Count ?? 0u) + 1)
      }).ToList();
    }



    private List<ReducedWordDocument> GetWordDocuments(List<string> words)
    {
      var collection = DbClient.Database.GetCollection<ReducedWordDocument>(DbClient.ReducedReverseIndexCollectionName);
      return collection.Find(x => words.Contains(x.Word)).ToList();
    }

    private float ComputeVectorModule(IEnumerable<float> numbers)
    {
      return (float)Math.Sqrt(numbers.Select(x => x * x).Sum());
    }
  }
}
