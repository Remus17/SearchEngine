using System;
using System.Diagnostics;
using System.Linq;
using SearchEngine.Database;
using SearchEngine.Miscellaneous.Classes;

namespace SearchEngine.Miscellaneous
{
  public class SearchService : ISearchService
  {
    private readonly VectorialSearch searchEngine;

    public SearchService()
    {
      searchEngine = new VectorialSearch();
      DbClient.Init();
    }

    public ExecuteVectorialSearchResult ExecuteVectorialSearch(string searchTerms)
    {
      if (string.IsNullOrWhiteSpace(searchTerms)) throw new ArgumentException(nameof(searchTerms));

      var stopWatch = Stopwatch.StartNew();
      var results = searchEngine.Search(searchTerms).ToList();
      stopWatch.Stop();
      if (!results.Any()) return null;

      var vectorialSearchResult = new ExecuteVectorialSearchResult
      {
        Documents = results.Select(x => new VectorialSearchDocumentResult(x.Similarity, x.Url)).ToList(),
        DocumentsCount = results.Count,
        SearchElapsedMilliseconds = stopWatch.Elapsed.TotalMilliseconds
      };
      return vectorialSearchResult;
    }
  }
}
