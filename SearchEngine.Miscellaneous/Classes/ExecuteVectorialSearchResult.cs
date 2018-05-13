using System.Collections.Generic;

namespace SearchEngine.Miscellaneous.Classes
{
  public class ExecuteVectorialSearchResult
  {
    public List<VectorialSearchDocumentResult> Documents { get; set; }
    public int DocumentsCount { get; set; }
    public double SearchElapsedMilliseconds { get; set; }

    public ExecuteVectorialSearchResult()
    {
      Documents = new List<VectorialSearchDocumentResult>();
    }
  }

  public class VectorialSearchDocumentResult
  {
    public double CosineSimilarity { get; set; }
    public string Url { get; set; }

    public VectorialSearchDocumentResult(double cosineSimilarity, string url)
    {
      CosineSimilarity = cosineSimilarity;
      Url = url;
    }
  }
}
