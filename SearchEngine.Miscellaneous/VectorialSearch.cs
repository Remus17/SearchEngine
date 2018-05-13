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
      var modules = GetDocumentModules(GetDocumentSet(storedWords));
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

      //dictionary with location as key and list of word data as value
      var documents = new Dictionary<string, List<WordData>>();
      foreach (var item in storedWords)
      {
        AddToDictionary(documents, item.Locations.Select(x =>
          new KeyValuePair<string, WordData>(x.Location,
            new WordData { Weight = x.FinalWeight, Word = item.Word })));
      }


      var vectorialTerms = GetVectorialSearchTerms(searchTerms, storedWords);

      return GetResults(vectorialTerms, documents, modules).OrderByDescending(x => x.Similarity);
    }


    private List<VectorialResult> GetResults(List<VectorialTerm> searched, Dictionary<string, List<WordData>> storedWords, List<ModuleDocument> modules)
    {
      var result = new List<VectorialResult>();
      var queryModule = ComputeVectorModule(searched.Select(x => x.Weight));
      foreach (var documentKey in storedWords.Keys)
      {
        var dbDocModule = modules.First(x => x.Location == documentKey).Module;
        result.Add(new VectorialResult()
        {
          Url = documentKey,
          Similarity = ComputeDocumentCosinus(searched, storedWords[documentKey], queryModule, dbDocModule)
        });
      }
      return result;
    }

    private float ComputeDocumentCosinus(List<VectorialTerm> searchedWords, List<WordData> storedWords, float queryModule, float dbDocModule)
    {
      float numerator = 0;
      foreach (var item in searchedWords)
      {
        float dbWordWeight = storedWords.FirstOrDefault(x => x.Word == item.Word)?.Weight ?? 0;
        numerator += item.Weight * dbWordWeight;
      }
      return numerator / (queryModule * dbDocModule);
    }
    //dictionary with location as key and list of word data as value
    private void AddToDictionary(Dictionary<string, List<WordData>> dictionary, IEnumerable<KeyValuePair<string, WordData>> items)
    {
      foreach (var item in items)
      {
        var newElement = new WordData() { Weight = item.Value.Weight, Word = item.Value.Word };
        if (dictionary.ContainsKey(item.Key))
        {
          dictionary[item.Key].Add(newElement);
        }
        else
        {
          dictionary.Add(item.Key, new List<WordData>() { newElement });
        }
      }
    }
    private List<VectorialTerm> GetVectorialSearchTerms(List<string> words, List<ReducedWordDocument> storedWords)
    {
      var totalFilesNumber = storedWords.SelectMany(x => x.Locations).Count();
      return words.Select(word => new VectorialTerm()
      {
        Word = word,
        Weight = (words.Where(y => y == word).Count() / (float)words.Count) * storedWords.FirstOrDefault(x => x.Word == word)?.InverseDocumentFrequency ?? 1
        //(float)Math.Log((totalFilesNumber / (float?)storedWords.FirstOrDefault(sw => sw.Word == x)?.Locations.Count ?? 0u) + 1)
      }).ToList();
    }



    private List<ReducedWordDocument> GetWordDocuments(List<string> words)
    {
      var collection = DbClient.Database.GetCollection<ReducedWordDocument>(DbClient.ReducedReverseIndexCollectionName);
      return collection.Find(x => words.Contains(x.Word)).ToList();
    }

    private List<ModuleDocument> GetDocumentModules(List<string> docs)
    {
      var collection = DbClient.Database.GetCollection<ModuleDocument>(DbClient.DocumentModulesCollectionName);
      return collection.Find(x => docs.Contains(x.Location)).ToList();
    }

    private float ComputeVectorModule(IEnumerable<float> numbers)
    {
      return (float)Math.Sqrt(numbers.Select(x => x * x).Sum());
    }
    private List<string> GetDocumentSet(List<ReducedWordDocument> storedWords)
    {
      return storedWords.SelectMany(x => x.Locations).Select(x => x.Location).Distinct().ToList();
    }

  }
}
