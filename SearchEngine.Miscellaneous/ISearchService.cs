using SearchEngine.Miscellaneous.Classes;

namespace SearchEngine.Miscellaneous
{
  public interface ISearchService
  {
    ExecuteVectorialSearchResult ExecuteVectorialSearch(string searchTerms);
  }
}
