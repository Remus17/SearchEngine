using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Database.Models
{
  public class MappedWordDocument
  {
    public ObjectId Id { get; set; }
    public int Count { get; set; }
    public string Url { get; set; }
    public string Word { get; set; }
    public float TermFrequency { get; set; }
  }
}
